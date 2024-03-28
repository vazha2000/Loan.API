using Loan.API.Controllers;
using Loan.API.Models;
using Loan.API.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Loan.API.Models.DTOs.User;
using Loan.API.Services.IServices;
using Loan.API.Exceptions;

namespace Loan.API.Tests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task GetUserInfo_ValidUserId_ReturnsOkResult()
        {
            // arrange
            var userId = "exampleId";
            var userInfo = new UserInfoDto
            {
                FirstName = "mela",
                LastName = "meladze",
                UserName = "mela2000",
                Email = "mela@mail.com",
                Salary = 20001m,
                Age = 21
            };

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(m => m.GetUserInfoAsync(userId)).ReturnsAsync(userInfo);
            var controller = new UsersController(userServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            };

            // Act
            var result = await controller.GetUserInfo();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUserInfo = Assert.IsType<UserInfoDto>(okResult.Value);
            Assert.Equal(userInfo, returnedUserInfo);
        }

        [Fact]
        public async Task GetUserInfo_UserNotFound_ReturnsBadRequestResult()
        {
            // Arrange
            var userId = "exampleId";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(m => m.GetUserInfoAsync(userId)).ThrowsAsync(new NotFoundException($"User with id {userId} not found"));
            var controller = new UsersController(userServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act 
            var result = await controller.GetUserInfo();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}

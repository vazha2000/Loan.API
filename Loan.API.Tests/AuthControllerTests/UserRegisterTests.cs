using Loan.API.Controllers;
using Loan.API.Models.DTOs.Auth;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan.API.Tests.AuthControllerTests
{
    public class UserRegisterTests
    {
        [Fact]
        public async Task UserRegister_SuccessfulRegistration_ReturnsOk()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@mail.com",
                Age = 30,
                Salary = 50000,
                Password = "password"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.UserRegisterAsync(registerDto))
                           .Verifiable();

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.UserRegister(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", okResult.Value);
            mockAuthService.Verify();
        }

        [Fact]
        public async Task UserRegister_UserAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "existingUser",
                Email = "john@mail.com",
                Age = 30,
                Salary = 50000,
                Password = "password"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.UserRegisterAsync(registerDto))
                           .ThrowsAsync(new InvalidOperationException($"User with this User Name already exists"));

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.UserRegister(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("User with this User Name already exists", conflictResult.Value);
        }

    }
}

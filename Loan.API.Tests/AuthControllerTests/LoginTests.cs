using Loan.API.Controllers;
using Loan.API.Models.DTOs.Auth;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan.API.Tests.AuthControllerTests
{
    public class LoginTests
    {
        [Fact]
        public async Task Login_Successful_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "validUser",
                Password = "validPassword"
            };

            var expectedToken = "mockToken";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginAsync(loginDto))
                           .ReturnsAsync(expectedToken);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var tokenProperty = responseObject.GetType().GetProperty("Token");
            var token = tokenProperty.GetValue(responseObject).ToString();
            Assert.Equal(expectedToken, token);
        }



        [Fact]
        public async Task Login_IncorrectCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "invalidUser",
                Password = "invalidPassword"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginAsync(loginDto))
                           .ThrowsAsync(new ArgumentException("incorrect username of password"));

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("incorrect username of password", badRequestResult.Value);
        }

    }
}

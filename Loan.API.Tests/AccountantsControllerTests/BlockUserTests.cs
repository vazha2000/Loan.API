using Loan.API.Controllers;
using Loan.API.Exceptions;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan.API.Tests.AccountantsControllerTests
{
    public class BlockUserTests
    {
        [Fact]
        public async Task BlockUser_SuccessfullyBlocked_ReturnsOk()
        {
            // Arrange
            var userId = "user123";

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.BlockUserForUnlimitedTimeAsync(userId))
                                 .Verifiable();

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.BlockUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User blocked successfully", okResult.Value);
            mockAccountantService.Verify();
        }

        [Fact]
        public async Task BlockUser_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var userId = "wrongUserId";

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.BlockUserForUnlimitedTimeAsync(userId))
                                 .ThrowsAsync(new NotFoundException($"User with id {userId} not found"));

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.BlockUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }
    }
}

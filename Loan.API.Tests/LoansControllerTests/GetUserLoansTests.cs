using Loan.API.Controllers;
using Loan.API.Enums;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Loan.API.Tests.LoansControllerTests
{
    public class GetUserLoansTests
    {
        [Fact]
        public async Task GetUserLoans_ValidUserIdFromBody_ReturnsOkResult()
        {
            // Arrange
            var userIdFromBody = "validUserId";
            var loans = new List<LoanModel>
            {
                new LoanModel { Id = Guid.NewGuid(), Amount = 1000m, Currency = "USD", LoanType = LoanType.CarLoan, Status = Enums.LoanStatus.InProgress, Period = 12 }
            };
            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(m => m.GetUserLoansAsync(userIdFromBody)).ReturnsAsync(loans);
            var controller = new LoansController(loanServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "userId")
                    }))
                }
            };

            // Act
            var result = await controller.GetUserLoans(userIdFromBody);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLoans = Assert.IsAssignableFrom<List<LoanModel>>(okResult.Value);
            Assert.Equal(loans, returnedLoans);
        }

        [Fact]
        public async Task GetUserLoans_ValidUserIdFromClaims_ReturnsOkResult()
        {
            // Arrange
            var userId = "userIdFromClaims";
            var loans = new List<LoanModel>
            {
                new LoanModel { Id = Guid.NewGuid(), Amount = 1000m, Currency = "USD", LoanType = LoanType.CarLoan, Status = Enums.LoanStatus.InProgress, Period = 12 }
            };
            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(m => m.GetUserLoansAsync(userId)).ReturnsAsync(loans);
            var controller = new LoansController(loanServiceMock.Object);
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
            var result = await controller.GetUserLoans(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLoans = Assert.IsAssignableFrom<List<LoanModel>>(okResult.Value);
            Assert.Equal(loans, returnedLoans);
        }

        [Fact]
        public async Task GetUserLoans_UserNotFound_ReturnsBadRequestResult()
        {
            // Arrange
            var userId = "wrongUserId";
            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(m => m.GetUserLoansAsync(userId)).ThrowsAsync(new NotFoundException($"User with id {userId} not found"));
            var controller = new LoansController(loanServiceMock.Object);
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
            var result = await controller.GetUserLoans(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserLoans_NullUserId_ReturnsUnauthorizedResult()
        {
            // Arrange
            string userIdFromBody = null;
            var loanServiceMock = new Mock<ILoanService>();
            var controller = new LoansController(loanServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal()
                }
            };

            // Act
            var result = await controller.GetUserLoans(userIdFromBody);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not authenticated or user ID not found in claims.", unauthorizedResult.Value);
        }
    }
}

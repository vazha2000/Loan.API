using Loan.API.Controllers;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Loan.API.Exceptions;
using Loan.API.Enums;

namespace Loan.API.Tests.LoansTests
{
    public class ApplyForLoanTests
    {
        [Fact]
        public async Task ApplyForLoan_ValidLoanDto_ReturnsOkResult()
        {
            // Arrange
            var userId = "someUserId";
            var loanDto = new LoanDto
            {
                Amount = 1000m,
                Currency = "USD",
                LoanType = LoanType.CarLoan,
                Period = 12
            };

            var loanServiceMock = new Mock<ILoanService>();
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
            var result = await controller.ApplyForLoan(loanDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("You applied for a loan successfully", okResult.Value);
        }

        [Fact]
        public async Task ApplyForLoan_UserNotFound_ReturnsBadRequestResult()
        {
            var userId = "someId";
            var loanDto = new LoanDto
            {
                Amount = 1000m,
                Currency = "USD",
                LoanType = LoanType.CarLoan,
                Period = 12
            };

            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(x => x.ApplyForLoanAsync(It.IsAny<LoanDto>(), userId))
                .Throws(new NotFoundException($"User with id {userId} not found"));
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
            var result = await controller.ApplyForLoan(loanDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task ApplyForLoan_UserBlocked_ReturnsForbiddenResult()
        {
            // Arrange
            var userId = "someId";
            var loanDto = new LoanDto()
            {
                Amount = 1000m,
                Currency = "USD",
                LoanType = LoanType.CarLoan,
                Period = 12
            };
            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(m => m.ApplyForLoanAsync(It.IsAny<LoanDto>(), userId)).ThrowsAsync(new UserBlockedException($"User with id {userId} is blocked and cannot apply for a loan."));
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
            var result = await controller.ApplyForLoan(loanDto);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);
        }

        [Fact]
        public async Task ApplyForLoan_ExceedsMaxPendingLoanRequests_ReturnsForbiddenResult()
        {
            // Arrange
            var userId = "userIdWithMaxPendingLoans";
            var loanDto = new LoanDto
            {
                Amount = 1000m,
                Currency = "USD",
                LoanType = LoanType.CarLoan,
                Period = 12
            };
            var loanServiceMock = new Mock<ILoanService>();
            loanServiceMock.Setup(m => m.ApplyForLoanAsync(It.IsAny<LoanDto>(), userId)).ThrowsAsync(new ExceedsMaxPendingLoanRequestException($"User already has max pending loan requests"));
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
            var result = await controller.ApplyForLoan(loanDto);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);
        }
    }
}

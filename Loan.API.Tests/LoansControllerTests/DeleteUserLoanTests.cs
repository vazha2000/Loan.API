using Loan.API.Controllers;
using Loan.API.Data;
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
    public class DeleteUserLoanTests
    {
        [Fact]
        public async Task DeleteUserLoan_SuccessfulDeletion_ReturnsNoContent()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var userId = "user123";

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.DeleteLoanAsync(userId, loanId))
                           .Verifiable();

            var controller = new LoansController(mockLoanService.Object);
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
            var result = await controller.DeleteUserLoan(loanId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockLoanService.Verify();
        }

        [Fact]
        public async Task DeleteUserLoan_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var userId = "wrongUserId";

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.DeleteLoanAsync(userId, loanId))
                           .ThrowsAsync(new NotFoundException($"User with id {userId} not found"));

            var controller = new LoansController(mockLoanService.Object);
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
            var result = await controller.DeleteUserLoan(loanId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUserLoan_LoanNotFound_ReturnsBadRequest()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var userId = "user123";

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.DeleteLoanAsync(userId, loanId))
                           .ThrowsAsync(new NotFoundException($"Loan with id {loanId} not found"));

            var controller = new LoansController(mockLoanService.Object);
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
            var result = await controller.DeleteUserLoan(loanId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Loan with id {loanId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUserLoan_UserNotAuthorized_ReturnsForbidden()
        {
            // Arrange
            var loanId = Guid.NewGuid();
            var userId = "user123";

            var existingLoan = new LoanModel
            {
                Id = loanId,
                UserId = userId,
                Status = Enums.LoanStatus.Accepted 
            };

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.DeleteLoanAsync(userId, loanId))
                           .ThrowsAsync(new InvalidOperationException($"Loan with id {loanId} cannot be deleted."));

            var mockDbContext = new Mock<ApplicationDbContext>();

            var controller = new LoansController(mockLoanService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    })),
                }
            };

            // Act
            var result = await controller.DeleteUserLoan(loanId);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
            Assert.Equal($"Loan with id {loanId} cannot be deleted.", forbiddenResult.Value);
        }


    }
}

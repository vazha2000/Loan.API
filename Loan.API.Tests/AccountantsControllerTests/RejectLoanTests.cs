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
    public class RejectLoanTests
    {
        [Fact]
        public async Task RejectLoan_SuccessfullyRejected_ReturnsOk()
        {
            // Arrange
            var loanId = Guid.NewGuid();

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.RejectLoanAsync(loanId))
                                 .Verifiable();

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.RejectLoan(loanId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Loan rejected", okResult.Value);
            mockAccountantService.Verify(); 
        }

        [Fact]
        public async Task RejectLoan_LoanNotFound_ReturnsBadRequest()
        {
            // Arrange
            var loanId = Guid.NewGuid();

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.RejectLoanAsync(loanId))
                                 .ThrowsAsync(new NotFoundException($"Loan with id {loanId} not found"));

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.RejectLoan(loanId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Loan with id {loanId} not found", badRequestResult.Value);
        }

        [Fact]
        public async Task RejectLoan_LoanAlreadyRejected_ReturnsConflict()
        {
            // Arrange
            var loanId = Guid.NewGuid();

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.RejectLoanAsync(loanId))
                                 .ThrowsAsync(new InvalidOperationException($"Loan with id {loanId} is already rejected"));

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.RejectLoan(loanId);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal($"Loan with id {loanId} is already rejected", conflictResult.Value);
        }

    }
}

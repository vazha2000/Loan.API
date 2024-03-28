using Loan.API.Controllers;
using Loan.API.Data;
using Loan.API.Enums;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class UpdateUserLoanTests
    {
        [Fact]
        public async Task UpdateUserLoan_SuccessfulUpdate_ReturnsOk()
        {
            // Arrange
            var loanDto = new LoanDto
            {
                Amount = 1000,
                Currency = "USD",
                Period = 12,
                LoanType = Enums.LoanType.CarLoan
            };

            var loanId = Guid.NewGuid();
            var userId = "user123";

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.UpdateLoanAsync(loanDto, userId, loanId))
                           .ReturnsAsync(loanDto);

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
            var result = await controller.UpdateUserLoan(loanDto, loanId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<LoanDto>(okResult.Value);
            Assert.Equal(loanDto.Amount, model.Amount);
            Assert.Equal(loanDto.Currency, model.Currency);
            Assert.Equal(loanDto.Period, model.Period);
            Assert.Equal(loanDto.LoanType, model.LoanType);
        }

        [Fact]
        public async Task UpdateUserLoan_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var loanDto = new LoanDto
            {
                Amount = 1000,
                Currency = "USD",
                Period = 12,
                LoanType = Enums.LoanType.CarLoan
            };

            var loanId = Guid.NewGuid();
            var userId = "wrongUserId";

            var mockLoanService = new Mock<ILoanService>();
            mockLoanService.Setup(service => service.UpdateLoanAsync(loanDto, userId, loanId))
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
            var result = await controller.UpdateUserLoan(loanDto, loanId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"User with id {userId} not found", badRequestResult.Value);
        }

        
    }
}


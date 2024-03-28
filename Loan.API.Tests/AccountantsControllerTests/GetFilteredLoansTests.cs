using Loan.API.Enums;
using Loan.API.Models.Loan;
using Loan.API.Models;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loan.API.Controllers;
using Loan.API.Validation;

namespace Loan.API.Tests.AccountantsControllerTests
{
    public class GetFilteredLoansTests
    {
        [Fact]
        public async Task GetFilteredLoans_ReturnsFilteredLoans()
        {
            // Arrange
            var filterOptions = new LoanFilterOptions
            {
                Status = LoanStatus.Accepted,
                Currency = "USD",
                MinAmount = 1000,
                MaxAmount = 2000,
                LoanType = LoanType.CarLoan
            };

            var expectedLoans = new List<LoanModel>
            {
                new LoanModel { Id = Guid.NewGuid(), Status = LoanStatus.Accepted, Currency = "USD", Amount = 1500, LoanType = LoanType.CarLoan },
                new LoanModel { Id = Guid.NewGuid(), Status = LoanStatus.Accepted, Currency = "USD", Amount = 1800, LoanType = LoanType.CarLoan }
            };

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.GetFilteredLoansAsync(filterOptions))
                                 .ReturnsAsync(expectedLoans);

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.GetFilteredLoans(filterOptions);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualLoans = Assert.IsAssignableFrom<List<LoanModel>>(okResult.Value);
            Assert.Equal(expectedLoans.Count, actualLoans.Count);
        }
    }
}

using Loan.API.Controllers;
using Loan.API.Models;
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
    public class GetAllLoansListTests
    {
        [Fact]
        public async Task GetAllLoansList_ReturnsListOfLoans()
        {
            // Arrange
            var expectedLoans = new List<LoanModel>
            {
                new LoanModel { Id = Guid.NewGuid(), Amount = 1000, Currency = "USD" },
                new LoanModel { Id = Guid.NewGuid(), Amount = 2000, Currency = "GEL" }
            };

            var mockAccountantService = new Mock<IAccountantService>();
            mockAccountantService.Setup(service => service.GetAllLoansAsync())
                                 .ReturnsAsync(expectedLoans);

            var controller = new AccountantsController(mockAccountantService.Object);

            // Act
            var result = await controller.GetAllLoansList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualLoans = Assert.IsAssignableFrom<List<LoanModel>>(okResult.Value);
            Assert.Equal(expectedLoans.Count, actualLoans.Count);
        }
    }
}

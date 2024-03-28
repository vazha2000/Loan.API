using Loan.API.Enums;
using Loan.API.Exceptions;
using Loan.API.Models.DTOs.Auth;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Loan.API.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Accountant")]
    public class AccountantsController : ControllerBase
    {
        private readonly IAccountantService _accountantService;

        public AccountantsController(IAccountantService accountantService)
        {
            _accountantService = accountantService;
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUser(string userId)
        {
            try
            {
                await _accountantService.BlockUserForUnlimitedTimeAsync(userId);
                return Ok("User blocked successfully");
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UnblockUser")]
        public async Task<IActionResult> UnblockUser(string userId)
        {
            try
            {
                await _accountantService.UnblockUserAsync(userId);
                return Ok("User unblocked successfully");
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AcceptLoan")]
        public async Task<IActionResult> AcceptLoan(Guid id) // loanId
        {
            try
            {
                await _accountantService.AcceptLoanAsync(id);
                return Ok("Loan accepted");
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("RejectLoan")]
        public async Task<IActionResult> RejectLoan(Guid id)
        {
            try
            {
                await _accountantService.RejectLoanAsync(id);
                return Ok("Loan rejected");
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("Loans")]
        public async Task<IActionResult> GetAllLoansList()
        {
            var loans = await _accountantService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("Loans/filter")]
        public async Task<IActionResult> GetFilteredLoans([FromQuery] LoanFilterOptions filterOptions)
        {
            var validator = new LoanFilterOptionsValidator();
            var validationResult = await validator.ValidateAsync(filterOptions);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var filteredLoans = await _accountantService.GetFilteredLoansAsync(filterOptions);
            return Ok(filteredLoans);
        }

    }
}

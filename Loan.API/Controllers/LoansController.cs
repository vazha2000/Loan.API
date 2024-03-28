using Loan.API.Exceptions;
using Loan.API.Models.Loan;
using Loan.API.Services;
using Loan.API.Services.IServices;
using Loan.API.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("ApplyforLoan")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ApplyForLoan([FromBody] LoanDto loanDto)
        {
            var validator = new LoanDtoValidator();
            var validationResult = await validator.ValidateAsync(loanDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized("User not authenticated or user ID not found in claims.");
                }

                await _loanService.ApplyForLoanAsync(loanDto, userId);
                return Ok("You applied for a loan successfully");
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UserBlockedException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (ExceedsMaxPendingLoanRequestException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User, Accountant")]
        public async Task<IActionResult> GetUserLoans(string? userIdFromBody)
        {
            try
            {
                string userId;

                if (!string.IsNullOrEmpty(userIdFromBody))
                {
                    userId = userIdFromBody;
                }
                else
                {

                    userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (userId == null)
                    {
                        return Unauthorized("User not authenticated or user ID not found in claims.");
                    }
                }

                var userLoansResponse = await _loanService.GetUserLoansAsync(userId);

                return Ok(userLoansResponse);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Loans/{loanId}")]
        [Authorize(Roles = "User, Accountant")]
        public async Task<IActionResult> UpdateUserLoan([FromBody] LoanDto loanDto, Guid loanId)
        {
            var validator = new LoanDtoValidator();
            var validationResult = await validator.ValidateAsync(loanDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized("User not authenticated or user ID not found in claims.");
                }

                var userLoanResponse = await _loanService.UpdateLoanAsync(loanDto, userId, loanId);

                return Ok(userLoanResponse);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }

        [HttpDelete("Loans/{loanId}")]
        [Authorize(Roles = "User, Accountant")]
        public async Task<IActionResult> DeleteUserLoan(Guid loanId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized("User not authenticated or user ID not found in claims.");
                }

                await _loanService.DeleteLoanAsync(userId, loanId);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}

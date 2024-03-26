using Loan.API.Exceptions;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
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
        public async Task<IActionResult> ApplyForLoad(LoanDto loanDto)
        {
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
            catch (ExceedsMaxPendingLoanRequestException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

using Loan.API.Exceptions;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountantsController : ControllerBase
    {
        private readonly IAccountantService _accountantService;

        public AccountantsController(IAccountantService accountantService)
        {
            _accountantService = accountantService;
        }

        [HttpPost("BlockUser")]
        [Authorize(Roles = "Accountant")]
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

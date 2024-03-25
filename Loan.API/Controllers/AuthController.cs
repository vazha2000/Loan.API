using Loan.API.Models.DTOs;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("Accountant")]
        public async Task<IActionResult> AccountantRegister(AccountantRegisterDto registerDto)
        {
            try
            {
                await _authService.AccountantRegisterAsync(registerDto);
                return Ok("Accountant registered successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<IActionResult> UserRegister(UserRegisterDto registerDto)
        {
            try
            {
                await _authService.UserRegisterAsync(registerDto);
                return Ok("User registered successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}

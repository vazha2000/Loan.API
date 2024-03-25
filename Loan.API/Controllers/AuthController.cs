using Loan.API.Models.DTOs.Auth;
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


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

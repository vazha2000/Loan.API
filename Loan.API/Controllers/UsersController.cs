using Loan.API.Exceptions;
using Loan.API.Models.DTOs.Auth;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Loan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUserInfo")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized("User not authenticated or user ID not found in claims.");
                }

                var userInfoResponse = await _userService.GetUserInfoAsync(userId);

                return Ok(userInfoResponse);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}

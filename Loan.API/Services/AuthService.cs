using Loan.API.Data;
using Loan.API.Models;
using Loan.API.Models.DTOs;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;

namespace Loan.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task AccountantRegisterAsync(AccountantRegisterDto registerDto)
        {
            var accountantExists = await _userManager.FindByNameAsync(registerDto.UserName);

            if (accountantExists != null)
            {
                throw new InvalidOperationException("Accountant with this email already exists");
            }

            ApplicationUser Accountant = new()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName
            };

            var result = await _userManager.CreateAsync(Accountant, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"User registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(Accountant, "Accountant");
        }

        public Task<string> LoginAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }
    }
}

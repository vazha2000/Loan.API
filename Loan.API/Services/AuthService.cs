using Loan.API.Data;
using Loan.API.Models;
using Loan.API.Models.DTOs.Auth;
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

        public async Task AccountantRegisterAsync(RegisterDto registerDto)
        {
            await RegisterAsync(registerDto, "Accountant");
        }

        public Task<string> LoginAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public async Task UserRegisterAsync(UserRegisterDto registerDto)
        {
            await RegisterAsync(registerDto, "User");
        }

        private async Task RegisterAsync(RegisterDto registerDto, string role)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDto.UserName);

            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with this User Name already exists");
            }

            var newUser = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName
            };

            if (registerDto is UserRegisterDto userDto)
            {
                newUser.Age = userDto.Age;
                newUser.Salary = userDto.Salary;
                newUser.IsBlocked = false;
            }

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("\n", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"{role} registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(newUser, role);
        }
    }
}

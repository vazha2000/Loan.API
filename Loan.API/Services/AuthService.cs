using Loan.API.Data;
using Loan.API.Models;
using Loan.API.Models.DTOs.Auth;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task UserRegisterAsync(UserRegisterDto registerDto)
        {
            await RegisterAsync(registerDto, "User");
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user != null &&
                await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("Firstname", user.FirstName),
                    new Claim("Lastname", user.LastName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwtToken = GetToken(authClaims);
                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return token;
            }

            throw new ArgumentException("incorrect username of password");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
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

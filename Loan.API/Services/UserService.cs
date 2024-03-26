using Loan.API.Data;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.DTOs.User;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace Loan.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<UserInfoDto> GetUserInfoAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            UserInfoDto userInfo = new()
            {
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                Salary = (decimal)existingUser.Salary,
                Age = (int)existingUser.Age
            };

            return userInfo;
        }
    }
}

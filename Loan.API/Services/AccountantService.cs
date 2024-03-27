using Loan.API.Data;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace Loan.API.Services
{
    public class AccountantService : IAccountantService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountantService(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task BlockUserForUnlimitedTimeAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            if(await _userManager.IsInRoleAsync(existingUser, "User") && existingUser.IsBlocked == false)
            {
                existingUser.IsBlocked = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UnblockUserAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            if (await _userManager.IsInRoleAsync(existingUser, "User") && existingUser.IsBlocked == true)
            {
                existingUser.IsBlocked = false;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

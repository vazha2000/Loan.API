using Loan.API.Data;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace Loan.API.Services
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoanService(ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task ApplyForLoanAsync(LoanDto loanDto, string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            var pendingLoanCount = _dbContext.Loans.Count(l => l.UserId == userId && l.Status == 0); // 0 indicates pending loan
            var maxPendingLoanCount = 2;

            if(pendingLoanCount >= maxPendingLoanCount)
            {
                throw new ExceedsMaxPendingLoanRequestException(
                    $"User already has {maxPendingLoanCount} pending loan requests. " +
                    $"Cannot apply for another loan at this time.");
            }

            LoanModel newLoan = new()
            {
                Amount = loanDto.Amount,
                Currency = loanDto.Currency,
                LoanType = loanDto.LoanType,
                Status = 0, // pending loan status by default
                Period = loanDto.Period,
                UserId = userId,
            };

            _dbContext.Loans.Add(newLoan);
            await _dbContext.SaveChangesAsync();
        }
    }
}

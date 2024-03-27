using Loan.API.Data;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            if(existingUser.IsBlocked == true)
            {
                throw new UserBlockedException($"User with id {userId} is blocked and cannot apply for a loan.");
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
                Status = Enums.LoanStatus.InProgress, // pending loan status by default
                Period = loanDto.Period,
                UserId = userId,
            };

            _dbContext.Loans.Add(newLoan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<LoanModel>> GetUserLoansAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            var loans = await _dbContext.Loans
                .Where(x => x.UserId == userId)
                .Select(x => new LoanModel
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    LoanType = x.LoanType,
                    Status = x.Status,
                    Period = x.Period
                })
                .ToListAsync();

            return loans;
        }

        public async Task<LoanDto> UpdateLoanAsync(LoanDto loanDto, string userId, Guid loanId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
            if (existingLoan == null)
            {
                throw new NotFoundException($"Loan with id {loanId} not found");
            }

            if (await _userManager.IsInRoleAsync(existingUser, "Accountant"))
            {
                existingLoan.Amount = loanDto.Amount;
                existingLoan.Currency = loanDto.Currency;
                existingLoan.Period = loanDto.Period;
                existingLoan.LoanType = loanDto.LoanType;
            }
            else
            {
                if (existingLoan.Status != 0 || existingLoan.UserId != userId)
                {
                    throw new InvalidOperationException($"Loan with id {loanId} " +
                        $"cannot be updated because " +
                        $"its status is not pending " +
                        $"or it does not belong to the user.");
                }

                existingLoan.Amount = loanDto.Amount;
                existingLoan.Currency = loanDto.Currency;
                existingLoan.Period = loanDto.Period;
                existingLoan.LoanType = loanDto.LoanType;
            }

            await _dbContext.SaveChangesAsync();

            LoanDto updatedLoan = new()
            {
                Amount = loanDto.Amount,
                Currency = loanDto.Currency,
                Period = loanDto.Period,
                LoanType = loanDto.LoanType
            };

            return updatedLoan;
        }
        public async Task DeleteLoanAsync(string userId, Guid loanId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
            if (existingLoan == null)
            {
                throw new NotFoundException($"Loan with id {loanId} not found");
            }

            if (await _userManager.IsInRoleAsync(existingUser, "Accountant"))
            {
                _dbContext.Loans.Remove(existingLoan);
            }

            else
            {
                if (existingLoan.UserId != userId || existingLoan.Status != 0)
                {
                    throw new InvalidOperationException($"Loan with id {loanId} cannot be deleted.");
                }

                _dbContext.Loans.Remove(existingLoan);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}

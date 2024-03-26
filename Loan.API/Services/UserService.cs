using Loan.API.Data;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.DTOs.User;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            _dbContext.Loans.Remove(existingLoan);
            await _dbContext.SaveChangesAsync();
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
            if(existingLoan.Status != 0)
            {
                throw new InvalidOperationException($"Loan with id {loanId} cannot be updated because its status is not pending.");
            }

            existingLoan.Amount = loanDto.Amount;
            existingLoan.Currency = loanDto.Currency;
            existingLoan.Period = loanDto.Period;
            existingLoan.LoanType = loanDto.LoanType;

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
    }
}

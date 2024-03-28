using Loan.API.Data;
using Loan.API.Enums;
using Loan.API.Exceptions;
using Loan.API.Models;
using Loan.API.Models.Loan;
using Loan.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

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

        public async Task AcceptLoanAsync(Guid loanId)
        {
            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);

            if (existingLoan == null)
            {
                throw new NotFoundException($"Loan with id {loanId} not found");
            }

            if (existingLoan.Status != Enums.LoanStatus.Accepted)
            {
                existingLoan.Status = Enums.LoanStatus.Accepted;
            }
            else
            {
                throw new InvalidOperationException($"Loan with id {loanId} is already accepted");
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task BlockUserForUnlimitedTimeAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new NotFoundException($"User with id {userId} not found");
            }

            if (await _userManager.IsInRoleAsync(existingUser, "User") && existingUser.IsBlocked == false)
            {
                existingUser.IsBlocked = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<LoanModel>> GetAllLoansAsync()
        {
            var loans = await _dbContext.Loans.ToListAsync();

            return MapToLoanModels(loans);
        }

        public async Task<List<LoanModel>> GetFilteredLoansAsync(LoanFilterOptions filterOptions)
        {

            IQueryable<LoanModel> query = _dbContext.Loans;

            if (filterOptions.Status.HasValue)
            {
                query = query.Where(loan => loan.Status == filterOptions.Status);
            }

            if (!string.IsNullOrEmpty(filterOptions.Currency))
            {
                query = query.Where(loan => loan.Currency == filterOptions.Currency);
            }

            if (filterOptions.MinAmount.HasValue)
            {
                query = query.Where(loan => loan.Amount >= filterOptions.MinAmount);
            }

            if (filterOptions.MaxAmount.HasValue)
            {
                query = query.Where(loan => loan.Amount <= filterOptions.MaxAmount);
            }

            if (filterOptions.LoanType.HasValue)
            {
                query = query.Where(loan => loan.LoanType == filterOptions.LoanType);
            }

            var loans = await query.ToListAsync();
            return MapToLoanModels(loans);
        }


        private List<LoanModel> MapToLoanModels(List<LoanModel> loans)
        {
            return loans.Select(loan => new LoanModel
            {
                Id = loan.Id,
                Amount = loan.Amount,
                Currency = loan.Currency,
                Period = loan.Period,
                LoanType = loan.LoanType,
                Status = loan.Status,
                UserId = loan.UserId
            }).ToList();
        }

        public async Task RejectLoanAsync(Guid loanId)
        {
            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(x => x.Id == loanId);

            if (existingLoan == null)
            {
                throw new NotFoundException($"Loan with id {loanId} not found");
            }

            if (existingLoan.Status != Enums.LoanStatus.Rejected)
            {
                existingLoan.Status = Enums.LoanStatus.Rejected;
            }
            else
            {
                throw new InvalidOperationException($"Loan with id {loanId} is already rejected");
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

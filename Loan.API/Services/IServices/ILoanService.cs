using Loan.API.Models;
using Loan.API.Models.Loan;

namespace Loan.API.Services.IServices
{
    public interface ILoanService
    {
        Task ApplyForLoanAsync(LoanDto loanDto, string userId);
        Task<List<LoanModel>> GetUserLoansAsync(string userId);
        Task<LoanDto> UpdateLoanAsync(LoanDto loanDto, string userId, Guid loanId);
        Task DeleteLoanAsync(string userId, Guid loanId);
    }
}

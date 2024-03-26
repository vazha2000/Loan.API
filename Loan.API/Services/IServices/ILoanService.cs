using Loan.API.Models.Loan;

namespace Loan.API.Services.IServices
{
    public interface ILoanService
    {
        Task ApplyForLoanAsync(LoanDto loanDto, string userId);
    }
}

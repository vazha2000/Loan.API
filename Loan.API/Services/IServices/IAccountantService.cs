using Loan.API.Enums;
using Loan.API.Models;
using Loan.API.Models.Loan;

namespace Loan.API.Services.IServices
{
    public interface IAccountantService
    {
        Task BlockUserForUnlimitedTimeAsync(string userId);
        Task UnblockUserAsync(string userId);
        Task AcceptLoanAsync(Guid loanId);
        Task RejectLoanAsync(Guid loanId);
        Task<List<LoanModel>> GetAllLoansAsync();
        Task<List<LoanModel>> GetFilteredLoansAsync(LoanFilterOptions filterOptions);
    }
}

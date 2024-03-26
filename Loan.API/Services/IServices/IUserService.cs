using Loan.API.Models;
using Loan.API.Models.DTOs.User;
using Loan.API.Models.Loan;

namespace Loan.API.Services.IServices
{
    public interface IUserService
    {
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<List<LoanModel>> GetUserLoansAsync(string userId);
        Task<LoanDto> UpdateLoanAsync(LoanDto loanDto, string userId, Guid loanId);
    }
}

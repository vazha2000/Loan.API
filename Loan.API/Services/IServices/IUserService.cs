using Loan.API.Models;
using Loan.API.Models.DTOs.User;

namespace Loan.API.Services.IServices
{
    public interface IUserService
    {
        Task<UserInfoDto> GetUserInfoAsync(string userId);
        Task<List<LoanModel>> GetUserLoansAsync(string userId);
    }
}

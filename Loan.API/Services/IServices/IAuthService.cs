using Loan.API.Models.DTOs;

namespace Loan.API.Services.IServices
{
    public interface IAuthService
    {
        Task AccountantRegisterAsync(AccountantRegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}

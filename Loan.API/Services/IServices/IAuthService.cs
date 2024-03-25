using Loan.API.Models.DTOs.Auth;

namespace Loan.API.Services.IServices
{
    public interface IAuthService
    {
        Task AccountantRegisterAsync(RegisterDto registerDto);
        Task UserRegisterAsync(UserRegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}

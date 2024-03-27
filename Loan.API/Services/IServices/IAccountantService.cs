namespace Loan.API.Services.IServices
{
    public interface IAccountantService
    {
        Task BlockUserForUnlimitedTimeAsync(string userId);
        Task UnblockUserAsync(string userId);
        Task AcceptLoanAsync(Guid loanId);
        Task RejectLoanAsync(Guid loanId);
    }
}

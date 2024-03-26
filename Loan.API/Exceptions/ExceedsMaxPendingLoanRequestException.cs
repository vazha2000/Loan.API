namespace Loan.API.Exceptions
{
    public class ExceedsMaxPendingLoanRequestException : Exception
    {
        public ExceedsMaxPendingLoanRequestException(string message) : base(message)
        {
            
        }
    }
}

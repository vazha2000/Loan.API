namespace Loan.API.Exceptions
{
    public class InvalidLoanIdFormatException : Exception
    {
        public InvalidLoanIdFormatException(string message) : base(message)
        {

        }
    }
}

using Loan.API.Enums;

namespace Loan.API.Models.Loan
{
    public class LoanDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public double Period { get; set; }
        public LoanType LoanType { get; set; }
    }
}

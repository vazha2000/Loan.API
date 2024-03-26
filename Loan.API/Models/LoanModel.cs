using Loan.API.Enums;

namespace Loan.API.Models
{
    public class LoanModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency {  get; set; }
        public double Period { get; set; }
        public LoanType LoanType { get; set; }
        public LoanStatus Status { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}

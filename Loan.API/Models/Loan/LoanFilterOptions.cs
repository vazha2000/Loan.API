using Loan.API.Enums;

namespace Loan.API.Models.Loan
{
    public class LoanFilterOptions
    {
        public LoanStatus? Status { get; set; }
        public string? Currency { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public LoanType? LoanType { get; set; }
    }
}

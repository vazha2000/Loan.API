using FluentValidation;
using Loan.API.Models.Loan;

namespace Loan.API.Validation
{
    public class LoanFilterOptionsValidator : AbstractValidator<LoanFilterOptions>
    {
        public LoanFilterOptionsValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be a valid enum value");

            RuleFor(x => x.LoanType)
                .IsInEnum().WithMessage("Loan type must be a valid enum value");

            RuleFor(x => x.Currency)
                .Must(x => x == null || x.ToLower() == "gel" || x.ToLower() == "usd").WithMessage("Currency must be either GEL or USD");
        }
    }
}

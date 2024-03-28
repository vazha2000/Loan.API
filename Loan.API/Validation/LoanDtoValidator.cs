using FluentValidation;
using Loan.API.Models.Loan;

namespace Loan.API.Validation
{
    public class LoanDtoValidator : AbstractValidator<LoanDto>
    {
        public LoanDtoValidator()
        {
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Must(x => x.ToLower() == "gel" || x.ToLower() == "usd").WithMessage("Currency must be either GEL or USD");

            RuleFor(x => x.Amount)
                .Must((dto, amount) => ValidateMinimumAmount(dto.Currency, amount))
                .WithMessage(dto => dto.Currency.ToLower() == "usd" ? "Minimum amount for USD currency is 20" :
                "Minimum amount for GEL currency is 60");

            RuleFor(x => x.Period).GreaterThan(0).WithMessage("Period must be greater than 0");
            RuleFor(x => x.LoanType).IsInEnum().WithMessage("Loan type must be a valid enum value");
        }

        private bool ValidateMinimumAmount(string currency, decimal amount)
        {
            string lowerCurrency = currency.ToLower();
            if (lowerCurrency == "usd")
            {
                return amount >= 20;
            }
            else if (lowerCurrency == "gel")
            {
                return amount >= 60;
            }
            return false;
        }
    }
}

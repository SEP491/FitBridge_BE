using FluentValidation;

namespace FitBridge_Application.Features.CustomerPurchaseds.CheckCustomerPurchased;

public class CheckCustomerPurchasedCommandValidator : AbstractValidator<CheckCustomerPurchasedCommand>
{
    public CheckCustomerPurchasedCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.PtId.HasValue || x.CustomerId.HasValue)
            .WithMessage("Either PtId or CustomerId must be provided");

        RuleFor(x => x.PtId)
            .NotEqual(Guid.Empty)
            .When(x => x.PtId.HasValue)
            .WithMessage("PtId cannot be empty");

        RuleFor(x => x.CustomerId)
            .NotEqual(Guid.Empty)
            .When(x => x.CustomerId.HasValue)
            .WithMessage("CustomerId cannot be empty");
    }
}

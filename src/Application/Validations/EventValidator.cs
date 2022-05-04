using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class EventValidator: AbstractValidator<CreateEventDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public EventValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.StartDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.SuccessMessageTitle).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EventLink)
                .NotEmpty()
                .When(x => x.IsOnline == true)
                .WithMessage("Event meeting link is required");
            RuleFor(x => x.EventLink)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .When(x => x.IsOnline == true)
                .WithMessage("Event meeting link not in the right format");
            RuleFor(x => x.EventLink)
                .Empty()
                .When(x => x.IsOnline != true)
                .WithMessage("Event meeting link is not required");
        }
    }

    public class UpdateEventValidator : AbstractValidator<UpdateEventDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateEventValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.StartDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.Target).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Target).IsEnumName(typeof(ETarget))
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValidValue.ToString()));
            RuleFor(x => x.SuccessMessageTitle).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }

}
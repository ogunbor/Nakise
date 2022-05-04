using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class SurveyValidator: AbstractValidator<CreateSurveyRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public SurveyValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Description).NotEmpty()
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
            RuleFor(x => x.SuccessMessageBody).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.SuccessMessageBody).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }

    public class UpdateSurveyValidator : AbstractValidator<UpdateSurveyRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateSurveyValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Description).NotEmpty()
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
            RuleFor(x => x.SuccessMessageBody).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.SuccessMessageBody).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
}

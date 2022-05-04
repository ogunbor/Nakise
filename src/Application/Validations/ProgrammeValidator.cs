using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class ProgrammeValidator: AbstractValidator<CreateProgrammeRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public ProgrammeValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Sponsor).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Category).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.DeliveryMethod).IsEnumName(typeof(EDeliveryMethod), caseSensitive: false)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValidValue.ToString()));
            RuleFor(x => x.DeliveryMethod).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Category).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.MaxAge).GreaterThan(x => x.MinAge).When(x => x.HasAge)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.AgeRangeCheck.ToString()));
            RuleFor(x => x.StartDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.GenderOption).NotEmpty().When(x => x.HasGender)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
    public class UpdateProgrammeValidator : AbstractValidator<UpdateProgrammeRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateProgrammeValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Category).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.DeliveryMethod).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.DeliveryMethod).IsEnumName(typeof(EDeliveryMethod), caseSensitive: false)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValidValue.ToString()));
            RuleFor(x => x.Category).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.MaxAge).GreaterThan(x => x.MinAge).When(x => x.HasAge)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.AgeRangeCheck.ToString()));
            RuleFor(x => x.StartDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.GenderOption).NotEmpty().When(x => x.HasGender)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
}

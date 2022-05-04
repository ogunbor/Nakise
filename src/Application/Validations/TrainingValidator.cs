using Application.Enums;
using Application.Resources;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class CreateTrainingValidator : AbstractValidator<CreateTrainingInputDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public CreateTrainingValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.ProgrammeId).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Title).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Description).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.LearningTrackIds).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.StartDate).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
        }
    }

    public class UpdateTrainingValidator : AbstractValidator<UpdateTrainingInputDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public UpdateTrainingValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.StartDate).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
        }
    }
}

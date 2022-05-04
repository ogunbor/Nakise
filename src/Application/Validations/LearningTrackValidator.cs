using Application.Enums;
using Application.Resources;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class LearningTrackValidator: AbstractValidator<CreateLearningTrackRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public LearningTrackValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Title).MaximumLength(100)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.MaxLengthIs100.ToString()));
        }
    }

    public class UpdateLearningTrackValidator : AbstractValidator<UpdateLearningTrackRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateLearningTrackValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Title).MaximumLength(100)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.MaxLengthIs100.ToString()));
        }
    }
}

using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class AssessmentValidator: AbstractValidator<CreateAssessmentRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public AssessmentValidator(IValidationLocalizerService localizer)
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
            RuleFor(x => x.CompletionTitle).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.CompletionMessage).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.TotalObtainableScore).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.PassMark).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.DueDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }

    public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentRequest>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateAssessmentValidator(IValidationLocalizerService localizer)
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
            RuleFor(x => x.CompletionTitle).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.CompletionMessage).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }

    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage(ValidationMessages.EnterValue);
            RuleFor(x => x.Score).NotEmpty().WithMessage(ValidationMessages.EnterValue);
            RuleFor(x => x.Options).NotEmpty().WithMessage(ValidationMessages.OptionValue);
            RuleForEach(x => x.Options).SetValidator(new OptionsValidator())
                .When(x => x.Options.Count > 0);
        }
    }

    public class OptionsValidator : AbstractValidator<QuestionOptionDto>
    {
        public OptionsValidator()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage(ValidationMessages.EnterValue);
        }
    }
}

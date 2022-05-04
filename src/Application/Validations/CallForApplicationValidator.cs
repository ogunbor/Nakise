using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class CallForApplicationValidator : AbstractValidator<CreateCallForApplicationInputDto>
    {
        private readonly IValidationLocalizerService _localizer;

        public CallForApplicationValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.ProgrammeId).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Title).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Description).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Target).IsEnumName(typeof(ETarget)).WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValidValue.ToString()));
            RuleFor(x => x.TargetNumber).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.StartDate).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.SuccessMessageTitle).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.SuccessMessageBody).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Stages).Must(x => x.Count > 0).When(x => x.IsStage).WithMessage(ResponseMessages.StageShouldNotBeEmpty);
            RuleForEach(x => x.Stages).SetValidator(new CreateStageValidator(_localizer)).When(x => x.IsStage).WithMessage(ResponseMessages.StageShouldNotBeEmpty);
        }
    }

    public class CreateStageValidator : AbstractValidator<CreateStageDto>
    {
        private readonly IValidationLocalizerService _localizer;

        public CreateStageValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Name).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0).WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
    public class UpdateCallForApplicationValidator : AbstractValidator<UpdateCallForApplicationInputDto>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateCallForApplicationValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.StartDate).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.DateRangeCheck.ToString()));
            RuleFor(x => x.TargetNumber).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
}

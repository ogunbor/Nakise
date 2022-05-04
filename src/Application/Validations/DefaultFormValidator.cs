using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class CreateDefaultFormValidator : AbstractValidator<CreateFormInputDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public CreateDefaultFormValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.ActivityType).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.ActivityType).IsEnumName(typeof(EActivityType)).WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));

            RuleForEach(x => x.FormFields).SetValidator(new FormFieldInputValidator(_localizer));
            RuleFor(x => x.FormFields).ValidateFieldIndex($"{_localizer.GetLocalizedString(EValidationError.DuplicateFieldIndex.ToString())}");
        }
    }

    public class FormFieldInputValidator : AbstractValidator<FormFieldInputDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public FormFieldInputValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Key).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Type).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Type).IsEnumName(typeof(EFieldType)).WithMessage((_localizer.GetLocalizedString(EValidationError.EnterValue.ToString())));
            RuleFor(x => x.Label).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Options).NotEmpty()
                .When(x => x.Type == EFieldType.MultipleAnswer.ToString() || 
                    x.Type == EFieldType.SingleAnswer.ToString() ||
                    x.Type == EFieldType.Links.ToString() ||
                    x.Type == EFieldType.Select.ToString())
                .WithMessage((_localizer.GetLocalizedString(EValidationError.EnterValue.ToString())));
            RuleFor(x => x.Options)
                .Empty()
                .When(x => x.Type != EFieldType.MultipleAnswer.ToString() &&
                    x.Type != EFieldType.SingleAnswer.ToString() &&
                    x.Type != EFieldType.Links.ToString() &&
                    x.Type != EFieldType.Select.ToString())
                .WithMessage(_localizer.GetLocalizedString(EValidationError.FieldOptionsEmpty.ToString()));
            RuleFor(x => x.RatingLevel).NotEmpty()
                .When(x => x.Type == EFieldType.Rating.ToString())
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.File).NotNull()
                .When(x => x.Type == EFieldType.FileUpload.ToString())
                .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.File).Null()
                .When(x => x.Type != EFieldType.FileUpload.ToString())
                .WithMessage(_localizer.GetLocalizedString(EValidationError.FileEmpty.ToString()));
        }
    }

    public class CreateFormFieldValueInputValidator : AbstractValidator<CreateFormFieldValueInputDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public CreateFormFieldValueInputValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;
            
            RuleFor(x => x.FieldValues).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleForEach(x => x.FieldValues).SetValidator(new CreateFieldValueValidator(_localizer));
        }
    }

    public class CreateFieldValueValidator : AbstractValidator<CreateFieldValueDto>
    {
        private readonly IValidationLocalizerService _localizer;
        public CreateFieldValueValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.FormFieldId).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Files).Empty()
                .When(x => !string.IsNullOrWhiteSpace(x.Value))
                .WithMessage(_localizer.GetLocalizedString(EValidationError.FileShouldBeEmpty.ToString()));
            RuleFor(x => x.Value).Empty()
                .When(x => x.Files.Count > 0)
                .WithMessage(_localizer.GetLocalizedString(EValidationError.ValueShouldBeEmpty.ToString()));
            RuleFor(x => x.Key).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
}

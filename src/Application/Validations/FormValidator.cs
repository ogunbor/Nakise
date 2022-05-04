using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;

namespace Application.Validations
{
    public class FormValidator: AbstractValidator<CreateFormDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public FormValidator(IValidationLocalizerService localizer)
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

    public class UpdateFormValidator : AbstractValidator<UpdateFormDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public UpdateFormValidator(IValidationLocalizerService localizer)
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

using Application.Enums;
using Application.Resources;
using Domain.Enums;
using FluentValidation;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class UserValidator : AbstractValidator<CreateUserInputDTO>
    {
        private readonly IValidationLocalizerService _localizer;
        public UserValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Email).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Email).EmailAddress().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterEmailValue.ToString())).When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            //RuleFor(x => x.Password).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            //RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
            //    .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterStrongPassword.ToString()));
            RuleFor(x => x.Role).IsEnumName(typeof(ERole)).WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValidValue.ToString()));
            RuleFor(x => x.Category).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public UserLoginValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Email).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Email).EmailAddress().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterEmailValue.ToString())).When(x => !string.IsNullOrWhiteSpace(x.Email));
            RuleFor(x => x.Password).NotEmpty().WithMessage(EValidationError.EnterValue.ToString());
        }
    }

    public class ResetPassowordValidator : AbstractValidator<ResetPasswordDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public ResetPassowordValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Email).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Email).EmailAddress().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterEmailValue.ToString())).When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }

    public class VerifyTokenValidator : AbstractValidator<VerifyTokenDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public VerifyTokenValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Token).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
        }
    }
    public class SetPasswordValidator : AbstractValidator<SetPasswordDTO>
    {
        private readonly IValidationLocalizerService _localizer;

        public SetPasswordValidator(IValidationLocalizerService localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Token).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Email).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Password).NotEmpty().WithMessage(_localizer.GetLocalizedString(EValidationError.EnterValue.ToString()));
            RuleFor(x => x.Password).Matches(@"(?-i)(?=^.{8,}$)((?!.*\s)(?=.*[A-Z])(?=.*[a-z]))((?=(.*\d){1,})|(?=(.*\W){1,}))^.*$")
               .WithMessage(_localizer.GetLocalizedString(EValidationError.EnterStrongPassword.ToString()));
            RuleFor(x => x.ProfilePicture).NotMoreThan4MB()
                .WithMessage(x => _localizer.GetLocalizedString(EValidationError.NotMoreThan4MB.ToString()))
                .When(x => x.ProfilePicture?.Length > 0);
            RuleFor(x => x.ProfilePicture).CheckFileFormat()
                .WithMessage(x => _localizer.GetLocalizedString(EValidationError.CheckFileFormat.ToString()))
                .When(x => x.ProfilePicture?.Length > 0);
        }
    }
}

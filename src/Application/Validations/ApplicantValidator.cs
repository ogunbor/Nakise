using Application.Enums;
using FluentValidation;
using Shared;
using Shared.DataTransferObjects;

namespace Application.Validations
{
    public class ApplicantDtoValidator : AbstractValidator<ApplicantStatusDto>
    {
        public ApplicantDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsEnumName(typeof(EUpdateApplicantStatus), caseSensitive: false)
                .WithMessage(ResponseMessages.StatusShouldBeApproveOrReject);
        }
    }

    public class BulkApplicantDtoValidator : AbstractValidator<BulkApplicantStatusDto>
    {
        public BulkApplicantDtoValidator()
        {
            RuleFor(x => x.Ids).NotEmpty().WithMessage(ResponseMessages.ApplicantIdsShouldNotBeEmpty);
            RuleFor(x => x.Status)
                .IsEnumName(typeof(EUpdateApplicantStatus), caseSensitive: false)
                .WithMessage(ResponseMessages.StatusShouldBeApproveOrReject);
        }
    }

    public class BulkApplicantStageDtoValidator : AbstractValidator<BulkApplicantStageDto>
    {
        public BulkApplicantStageDtoValidator()
        {
            RuleFor(x => x.Ids).NotEmpty().WithMessage(ResponseMessages.ApplicantIdsShouldNotBeEmpty);
            RuleFor(x => x.StageId).NotEmpty().WithMessage(ResponseMessages.ApplicantStageIdShouldNotBeEmpty);
        }
    }
}
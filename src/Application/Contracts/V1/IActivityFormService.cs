using Application.Helpers;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface IActivityFormService : IAutoDependencyService
    {
        Task<SuccessResponse<GetDefaultFormDto>> CreateDefaultForm(Guid id, CreateFormInputDto inputDto);
        Task<SuccessResponse<GetDefaultFormDto>> CreateCustomForm(Guid id, CreateFormInputDto inputDto);
        Task<SuccessResponse<GetDefaultFormDto>> GetFormByType(Guid formId, string formType);
        Task<SuccessResponse<GetDefaultFormDto>> CreateFormFieldValues(Guid formId, CreateFormFieldValueInputDto inputDto);
        Task<SuccessResponse<GetFormStatusDto>> GetFormStatus(Guid formId);
        Task<SuccessResponse<GetApplicantFormDetailsDto>> GetApplicantFormDetails(Guid formId, Guid applicantId);
        Task<SuccessResponse<GetApplicantFormStatusDto>> UpdateApplicantStage(Guid applicantId, Guid stageId);
        Task<SuccessResponse<GetBulkApplicantForStatusDto>> BulkUpdateApplicantStage(BulkApplicantStageDto model);
        Task<SuccessResponse<GetApplicantFormStatusDto>> ApproveOrRejectApplicant(Guid id, ApplicantStatusDto model);
        Task<SuccessResponse<GetBulkApplicantForStatusDto>> BulkApproveOrRejectApplicants(BulkApplicantStatusDto model);
        Task<SuccessResponse<GetDefaultFormDto>> SubmitActivityFormValues(Guid approvedApplicantId, SubmitActivityFormFieldValueDto model);
    }
}

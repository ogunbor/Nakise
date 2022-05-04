using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1.Activities
{
    public interface ICallForApplicationService : IAutoDependencyService
    {
        Task<SuccessResponse<GetCallForApplicationDto>> CreateCallForApplication(CreateCallForApplicationInputDto model);
        Task<SuccessResponse<GetCallForApplicationDto>> UpdateCallForApplication(Guid callForApplicationId, UpdateCallForApplicationInputDto model);
        Task<SuccessResponse<GetCallForApplicationDto>> GetCallForApplication(Guid id);
        Task DeleteCallForApplication(Guid id);
        Task<PagedResponse<IEnumerable<CallForApplicationParticipantDTO>>> GetCallForApplicationParticipants(Guid callForApplicationId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<GetCallForApplicationStatusDto>> SuspendCallForApplication(Guid id);
        Task<SuccessResponse<GetCallForApplicationStatusDto>> ExtendCallForApplication(Guid id, ExtendCallForApplication extendCfa);
        Task<SuccessResponse<GetCallForApplicationStatusDto>> ActivateCallForApplication(Guid id);
        Task<SuccessResponse<List<CfaGetStageDto>>> GetStages(Guid callForApplicationId);
        Task<SuccessResponse<List<CfaGetStageStatDto>>> GetStagesStat(Guid callForApplicationId);
        Task<SuccessResponse<GetCfaSubmissionStatDto>> GetCfaSubmission(Guid callForApplicationId);
        Task<SuccessResponse<ApplicantDTO>> GetCallForApplicationApplicantById(Guid callForApplicationId, Guid applicantId);
        Task<SuccessResponse<GetCallForApplicationStatusDto>> CloseCallForApplication(Guid id);
    }
}

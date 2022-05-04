using Application.Helpers;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface ISurveyService: IAutoDependencyService
    {
        Task<SuccessResponse<GetSurveyDTO>> CreateSurvey(CreateSurveyRequest request);
        Task<SuccessResponse<GetSurveyDTO>> GetASurvey(Guid surveyId);
        Task DeleteSurvey(Guid surveyId);
        Task<SuccessResponse<GetSurveyDTO>> UpdateSurvey(Guid assessmentId, UpdateSurveyRequest request);
        Task<SuccessResponse<GetApproveApplicantSurveyStatsDTO>> GetApprovedApplicantSurveyStats(Guid programId, Guid approvedApplicantId);
    }
}

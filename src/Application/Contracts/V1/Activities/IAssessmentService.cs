using Application.Helpers;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts.V1.Activities
{
    public interface IAssessmentService: IAutoDependencyService
    {
        Task<SuccessResponse<GetAssessmentDTO>> CreateAssessment(CreateAssessmentRequest request);
        Task<SuccessResponse<GetAssessmentDTO>> GetAssessment(Guid assessmentId);
        Task DeleteAssessment(Guid assessmentId);
        Task<SuccessResponse<GetAssessmentDTO>> UpdateAssessment(Guid assessmentId, UpdateAssessmentRequest request);
        Task<AssessmentDto> GetAllAssessmentAsync(AssessmentParameters parameters);
        Task<SuccessResponse<AssessmentSessionDto>> StartAssessment(Guid assessmentId, Guid approvedApplicantId);
        Task<SuccessResponse<QuestionAnswerDto>> SubmitAssessmentQuestionAnswer(Guid approvedApplicantId, QuestionAnswerDto model);
        Task<SuccessResponse<string>> CompleteAssessmentSession(Guid approvedApplicantId, SessionCompletionDto model);
        Task<SuccessResponse<AnswersPreviewDto>> GetAssessmentSessionSummary(Guid approvedApplicantId, SessionCompletionDto model);
        Task<SuccessResponse<CreateQuestionResponseDto>> CreateSingleQuestion(Guid id, CreateQuestionDto input);
    }
}
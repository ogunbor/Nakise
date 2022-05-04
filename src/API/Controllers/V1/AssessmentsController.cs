using Application.Contracts.V1;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace API.Controllers.V1
{

    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/assessments")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AssessmentsController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssessment([FromQuery] AssessmentParameters parameter)
        {
            var response = await _service.AssessmentService.GetAllAssessmentAsync(parameter);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create an assessment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<GetAssessmentDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAssessment(CreateAssessmentRequest request)
        {
            var response = await _service.AssessmentService.CreateAssessment(request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create a single question
        /// </summary>
        /// <param name="id"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Facilitator")]
        [HttpPost("{id}/question")]
        [ProducesResponseType(typeof(SuccessResponse<CreateQuestionResponseDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateQuestion(Guid id, CreateQuestionDto question)
        {
            var response = await _service.AssessmentService.CreateSingleQuestion(id, question);
            return CreatedAtAction(null, response);
        }

        /// <summary>
        /// Endpoint to start an assessment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Beneficiary,Facilitator")]
        [HttpPost("{id}/applicant/{applicantId}/start")]
        [ProducesResponseType(typeof(SuccessResponse<AssessmentSessionDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartAssessment(Guid id, Guid applicantId)
        {
            var response = await _service.AssessmentService.StartAssessment(id, applicantId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to answer a question
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Beneficiary,Facilitator")]
        [HttpPost("applicant/{id}/submit/question-answer")]
        [ProducesResponseType(typeof(SuccessResponse<AssessmentSessionDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SubmitAssessmentQuestionAnswer(Guid id, QuestionAnswerDto model)
        {
            var response = await _service.AssessmentService.SubmitAssessmentQuestionAnswer(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to complete assessment session
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Beneficiary,Facilitator")]
        [HttpPost("applicant/{id}/session/complete")]
        [ProducesResponseType(typeof(SuccessResponse<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CompleteAssessmentSession(Guid id, SessionCompletionDto model)
        {
            var response = await _service.AssessmentService.CompleteAssessmentSession(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get assessment session summary
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Beneficiary,Facilitator")]
        [HttpPost("applicant/{id}/session/summary")]
        [ProducesResponseType(typeof(SuccessResponse<AnswersPreviewDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAssessmentSessionSummary(Guid id, SessionCompletionDto model)
        {
            var response = await _service.AssessmentService.GetAssessmentSessionSummary(id, model);

            return Ok(response);
        }

        [HttpGet("applicant/session/{id}/result")]
        public async Task<IActionResult> GetAssessmentResult(AssessmentResultParameter id)
        {
            var response = await _service.AssessmentService.GetAssessmentResultAsync(id);
            return Ok(response);
        }
    }
}

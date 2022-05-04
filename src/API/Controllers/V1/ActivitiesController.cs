using Application.Contracts.V1;
using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using System.Net;

namespace API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/activities")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ActivitiesController(IServiceManager service)
        {
            _service = service;
        }        

        /// <summary>
        /// Endpoint to get an assessment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("assessment/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetAssessmentDTO>), 200)]
        public async Task<IActionResult> GetAssessment(Guid id)
        {
            var response = await _service.AssessmentService.GetAssessment(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update an assessment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("assessment/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetAssessmentDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAssessment(Guid id, UpdateAssessmentRequest request)
        {
            var response = await _service.AssessmentService.UpdateAssessment(id, request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete an assessment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("assessment/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAssessment(Guid id)
        {
            await _service.AssessmentService.DeleteAssessment(id);
            return NoContent();
        }

       
        /// <summary>
        /// Endpoint to get all activities
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet(Name = nameof(GetActivities))]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<GetActivityDTO>>), 200)]
        public async Task<IActionResult> GetActivities()
        {
            var response = await _service.ActivityService.GetActivities();
            
            return Ok(response);
        }
       
        /// <summary>
        /// Endpoint to get activity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetActivityDTO>), 200)]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var response = await _service.ActivityService.GetActivityById(id);

            return Ok(response);
        }  

         
        /// <summary>
        /// Endpoint to create a form
        /// </summary>
        /// <param name="formDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost("form")]
        [ProducesResponseType(typeof(SuccessResponse<GetFormDTO>), 200)]
        public async Task<IActionResult>  CreateForm(CreateFormDTO formDTO)
        {
            var response = await _service.FormService.CreateForm(formDTO);
            return Ok(response);
        }
 

        /// <summary>
        /// Endpoint to get a form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("form/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetFormDTO>), 200)]
        public async Task<IActionResult> GetForm(Guid id)
        {
            var response = await _service.FormService.GetForm(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a form
        /// </summary>
        /// <param name="formDTO"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("form/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetFormDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateForm(Guid id, UpdateFormDTO formDTO)
        {
            var response = await _service.FormService.UpdateForm(id, formDTO);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("form/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteForm(Guid id)
        {
            await _service.FormService.DeleteForm(id);
            return NoContent();
        }

        /// <summary>
        /// Enpdoint to create a call for application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost("training")]
        [ProducesResponseType(typeof(SuccessResponse<GetTrainingDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateTraining(CreateTrainingInputDto model)
        {
            var response = await _service.TrainingService.CreateTrainingActivity(model);

            return CreatedAtAction(nameof(GetTraining), new { id = response.Data.Id }, response);
        }

        /// <summary>
        /// Endpoint to get training activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("training/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetTrainingDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTraining(Guid id)
        {
            var response = await _service.TrainingService.GetTrainingActivity(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete training activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("training/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteTraining(Guid id)
        {
            await _service.TrainingService.DeleteTrainingActivity(id);

            return NoContent();
        }

        /// <summary>
        /// Endpoint to update training activity
        /// </summary>
        /// <param name="id"></param>
        /// /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("training/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetTrainingDto>), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateTraining(Guid id, UpdateTrainingInputDto input)
        {
            var response = await _service.TrainingService.UpdateTrainingActivity(id, input);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create a survey
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost("survey")]
        [ProducesResponseType(typeof(SuccessResponse<GetSurveyDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateSurvey(CreateSurveyRequest request)
        {
            var response = await _service.SurveyService.CreateSurvey(request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a survey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("survey/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetSurveyDTO>), 200)]
        public async Task<IActionResult> GetASurvey(Guid id)
        {
            var response = await _service.SurveyService.GetASurvey(id);

            return Ok(response);
        }
        
        /// Endpoint to delete an survey
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("survey/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteSurvey(Guid id)
        {
            await _service.SurveyService.DeleteSurvey(id);
            return NoContent();
        }

        /// <summary>
        /// Endpoint to update an survey
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("survey/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetSurveyDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSurvey(Guid id, UpdateSurveyRequest request)
        {
            var response = await _service.SurveyService.UpdateSurvey(id, request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all program activities
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("programme/{id}", Name = nameof(GetProgrammeActivities))]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<GetProgrammeActivitiesDTO>>), 200)]
        public async Task<IActionResult> GetProgrammeActivities(Guid id, [FromQuery]ResourceParameter parameter)
        {
            var response = await _service.ActivityService.GetProgrammeActivities(id, parameter, nameof(GetProgrammeActivities), Url);

            return Ok(response);
        }

        /// <summary>
        /// Enpdoint to create a call for application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost("call-for-application")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationDto>), 201)]
        public async Task<IActionResult> CreateCallForApplication(CreateCallForApplicationInputDto input)
        {
            var response = await _service.CallForApplicationService.CreateCallForApplication(input);

            return CreatedAtAction(nameof(GetCallForApplicationById), new { id = response.Data.Id }, response);
        }

        /// <summary>
        /// Endpoint to update a call for application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("call-for-application/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationDto>), 200)]
        public async Task<IActionResult> UpdateCallForApplication(Guid id, UpdateCallForApplicationInputDto input)
        {
            var response = await _service.CallForApplicationService.UpdateCallForApplication(id, input);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get call for applicataion by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("call-for-application/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationDto>), 200)]
        public async Task<IActionResult> GetCallForApplicationById(Guid id)
        {
            var response = await _service.CallForApplicationService.GetCallForApplication(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete call for applicataion by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("call-for-application/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteCallForApplicationById(Guid id)
        {
            await _service.CallForApplicationService.DeleteCallForApplication(id);

            return NoContent();
        }

        /// <summary>
        /// Endpoint to get call for application stages
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("call-for-application/{id}/stages")]
        [ProducesResponseType(typeof(SuccessResponse<List<CfaGetStageDto>>), 200)]
        public async Task<IActionResult> GetStagesByCallForApplication(Guid id)
        {
            var response = await _service.CallForApplicationService.GetStages(id);

            return Ok(response);
        }


        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("call-for-application/{id}/stage-stat")]
        [ProducesResponseType(typeof(SuccessResponse<List<CfaGetStageStatDto>>), 200)]
        public async Task<IActionResult> GetCallForApplicationStageStat(Guid id)
        {
            var response = await _service.CallForApplicationService.GetStagesStat(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get CFA submission stats
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("call-for-application/{id}/submision-stat")]
        [ProducesResponseType(typeof(SuccessResponse<GetCfaSubmissionStatDto>), 200)]
        public async Task<IActionResult> GetCfaSubmissionStat(Guid id)
        {
            var response = await _service.CallForApplicationService.GetCfaSubmission(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get applicataion for a given callForApplication by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("call-for-application/{id}/applicants/{applicantId}")]
        [ProducesResponseType(typeof(SuccessResponse<ApplicantDTO>), 200)]
        public async Task<IActionResult> GetCallForApplicationApplicantById(Guid id, Guid applicantId)
        {
            var response = await _service.CallForApplicationService.GetCallForApplicationApplicantById(id, applicantId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all participants for a program callforapplication
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("call-for-application/{id}/applicants", Name = nameof(GetCallForApplicationParticipants))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<CallForApplicationParticipantDTO>>), 200)]
        public async Task<IActionResult> GetCallForApplicationParticipants(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.CallForApplicationService.GetCallForApplicationParticipants(id, parameter, nameof(GetCallForApplicationParticipants), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to activate a callForApplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("call-for-application/{id}/activate")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationStatusDto>), 200)]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            var response = await _service.CallForApplicationService.ActivateCallForApplication(id);
            
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to suspend a CallForApplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("call-for-application/{id}/suspend")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationStatusDto>), 200)]
        public async Task<IActionResult> SuspendCallForApplication(Guid id)
        {
            var response = await _service.CallForApplicationService.SuspendCallForApplication(id);
            
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to Close a CallForApplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("call-for-application/{id}/close")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationStatusDto>), 200)]
        public async Task<IActionResult> CloseCallForApplication(Guid id)
        {
            var response = await _service.CallForApplicationService.CloseCallForApplication(id);
            return Ok(response);
        }
        
        /// <summary>
        /// Endpoints to extend a CallForApplication
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("call-for-application/{id}/extend")]
        [ProducesResponseType(typeof(SuccessResponse<GetCallForApplicationStatusDto>), 200)]
        public async Task<IActionResult> ExtendCallForApplication(Guid id, ExtendCallForApplication extendCfa)
        {
            var response = await _service.CallForApplicationService.ExtendCallForApplication(id, extendCfa);
            return Ok(response);
        }


        /// <summary>
        /// Endpoint to create default and default forms
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/default-form")]
        [ProducesResponseType(typeof(SuccessResponse<GetDefaultFormDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateDefaultForms(Guid id, CreateFormInputDto model)
        {
            var response = await _service.ActivityFormService.CreateDefaultForm(id, model);

            return CreatedAtAction(nameof(GetFormByType), new { formId = response.Data.Id }, response);
        }

        /// <summary>
        /// Endpoint to create default and custom forms
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/custom-form")]
        [ProducesResponseType(typeof(SuccessResponse<GetDefaultFormDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCustomForms(Guid id, CreateFormInputDto model)
        {
            var response = await _service.ActivityFormService.CreateCustomForm(id, model);

            return CreatedAtAction(nameof(GetFormByType), new { formId = response.Data.Id }, response);
        }

        /// <summary>
        /// Endpoint to get default and custom forms
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("dynamic-form/{formId}")]
        [ProducesResponseType(typeof(SuccessResponse<GetDefaultFormDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFormByType(Guid formId, [FromQuery] string formType = null)
        {
            var response = await _service.ActivityFormService.GetFormByType(formId, formType);
            
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create custom form values
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("dynamic-form/{formId}")]
        [ProducesResponseType(typeof(SuccessResponse<GetDefaultFormDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCustomFormValues(Guid formId, [FromForm] CreateFormFieldValueInputDto model)
        {
            var response = await _service.ActivityFormService.CreateFormFieldValues(formId, model);

            return CreatedAtAction(nameof(GetFormByType), new { formId = response.Data.Id }, response);
        }

        /// <summary>
        /// Endpoint to get the status of a form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("forms/{id}/status")]
        [ProducesResponseType(typeof(SuccessResponse<GetFormStatusDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFormStatus(Guid id)
        {
            var response = await _service.ActivityFormService.GetFormStatus(id);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("form/{formId}/applicant/{applicantId}")]
        [ProducesResponseType(typeof(SuccessResponse<GetApplicantFormDetailsDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetApplicantFormDetails(Guid formId, Guid applicantId)
        {
            var response = await _service.ActivityFormService.GetApplicantFormDetails(formId, applicantId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update the status of an applicant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stageId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("applicants/{id}/stage")]
        [ProducesResponseType(typeof(SuccessResponse<GetApplicantFormStatusDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateApplicantStage(Guid id, Guid stageId)
        {
            var response = await _service.ActivityFormService.UpdateApplicantStage(id, stageId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to bulk-update the status of applicants
        /// </summary>
        /// <param name="applicantIds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("applicants/stage")]
        [ProducesResponseType(typeof(SuccessResponse<GetBulkApplicantForStatusDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BulkUpdateApplicantStage(BulkApplicantStageDto model)
        {
            var response = await _service.ActivityFormService.BulkUpdateApplicantStage(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to approve or reject an applicant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,ProgramManager")]
        [HttpPut("applicants/{id}/status")]
        [ProducesResponseType(typeof(SuccessResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApproveOrRejectApplicant(Guid id, ApplicantStatusDto model)
        {
            var response = await _service.ActivityFormService.ApproveOrRejectApplicant(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to bulk approve or reject applicants
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,ProgramManager")]
        [HttpPut("applicants/status")]
        [ProducesResponseType(typeof(SuccessResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApproveOrRejectApplicants(BulkApplicantStatusDto model)
        {
            var response = await _service.ActivityFormService.BulkApproveOrRejectApplicants(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to submit activity form values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("applicant/{id}/activity-form")]
        [ProducesResponseType(typeof(SuccessResponse<GetDefaultFormDto>), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> SubmitActivityFormValues(Guid id, SubmitActivityFormFieldValueDto model)
        {
            var response = await _service.ActivityFormService.SubmitActivityFormValues(id, model);

            return Ok(response);
        }
    }
}

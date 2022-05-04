using Application.Contracts.V1;
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
    [Route("api/v{version:apiVersion}/people")]
    public class PeopleController : ControllerBase
    {
        private readonly IServiceManager _service;
        public PeopleController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoints to get the people's statistics
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager, Admin")]
        [HttpGet("stats")]
        [ProducesResponseType(typeof(SuccessResponse<GetPeopleStatsDto>), 200)]
        public async Task<IActionResult> GetPeopleStat()
        {
            var response = await _service.PeopleService.GetPeopleStats();
            return Ok(response);
        }



        /// <summary>
        /// Endpoint to get all beneficiaries
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("beneficiaries", Name = nameof(GetBeneficiaries))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<BeneficiaryDTO>>), 200)]
        public async Task<IActionResult> GetBeneficiaries([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.PeopleService.GetBeneficiaries(parameter, nameof(GetBeneficiaries), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiaries statistics
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("beneficiaries/stats")]
        [ProducesResponseType(typeof(SuccessResponse<BeneficiaryStatsDTO>), 200)]
        public async Task<IActionResult> GetBeneficiariesStat()
        {
            var response = await _service.PeopleService.GetBeneficiariesStat();

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiary by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("beneficiaries/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BeneficiaryDTO>), 200)]
        public async Task<IActionResult> GetBeneficiaryById(Guid id)
        {
            var response = await _service.PeopleService.GetBeneficiaryById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to edit beneficiary details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Beneficiary")]
        [HttpPut("beneficiaries/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<BeneficiaryDTO>), 200)]
        public async Task<IActionResult> UpdateBeneficiary(Guid id, UpdateBeneficiaryDTO model)
        {
            var response = await _service.PeopleService.UpdateBeneficiary(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiary's programme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("beneficiaries/{id}/programmes")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<BeneficiaryProgrammeDTO>>), 200)]
        public async Task<IActionResult> GetBeneficiaryProgrammes(Guid id)
        {
            var response = await _service.PeopleService.GetBeneficiaryProgrammes(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiary's programme Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("beneficiary/{id}/programme{programId}")]
        [ProducesResponseType(typeof(SuccessResponse<BeneficiaryProgrammeAndLearningTrackStatsDTO>), 200)]
        public async Task<IActionResult> GetBeneficiaryProgrammeDetails(Guid id, Guid programId)
        {
            var response = await _service.PeopleService.GetBeneficiaryProgrammeDetails(id, programId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiary's programmes stats
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("beneficiary/{id}/programmes/stats")]
        [ProducesResponseType(typeof(SuccessResponse<BeneficiaryProgrammesStatsDTO>), 200)]
        public async Task<IActionResult> GetBeneficiaryProgrammesStats(Guid id)
        {
            var response = await _service.PeopleService.GetBeneficiaryProgrammesStats(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get beneficiary's programmes events
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("beneficiary/programme/{id}/events")]
        [ProducesResponseType(typeof(SuccessResponse<ICollection<GetEventDTO>>), 200)]
        public async Task<IActionResult> GetBeneficiaryProgrammeEvents(Guid id)
        {
            var response = await _service.PeopleService.GetBeneficiaryProgrammeEvents(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all programme managers
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("programme-managers", Name = nameof(GetAllProgrammeManagers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetProgrammeManagerDto>>), 200)]
        public async Task<IActionResult> GetAllProgrammeManagers([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.PeopleService.GetAllProgrammeManagersAsync(parameter, nameof(GetAllProgrammeManagers), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get a single programme manager
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("programme-managers/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetSingleProgrammeManagerDto>), 200)]
        public async Task<IActionResult> GetProgrammeManager(Guid id)
        {
            var response = await _service.PeopleService.GetProgrammeManagerAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get a programmes for a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("programme-managers/{id}/programmes")]
        [ProducesResponseType(typeof(SuccessResponse<ICollection<GetUserProgrammeDto>>), 200)]
        public async Task<IActionResult> GetUserProgrammes(Guid id, [FromQuery] string search)
        {
            var response = await _service.PeopleService.GetUserProgrammesAsync(id, search);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get programme manager stats
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("programme-managers/stats")]
        [ProducesResponseType(typeof(SuccessResponse<GetProgrammeManagerStatDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProgrammeManagerStat()
        {
            var response = await _service.PeopleService.GetProgrammeManagerStatAsync();

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all facilitators
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("facilitators", Name = nameof(GetAllFacilitators))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetFacilitatorDto>>), 200)]
        public async Task<IActionResult> GetAllFacilitators([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.PeopleService.GetAllFacilitatorsAsync(parameter, nameof(GetAllFacilitators), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get the facilitators's statistics
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager, Admin")]
        [HttpGet("facilitators/stats")]
        [ProducesResponseType(typeof(SuccessResponse<GetFacilitatorStatsDto>), 200)]
        public async Task<IActionResult> GetFacilitatorsStat()
        {
            var response = await _service.PeopleService.GetFacilitatorsStat();
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get the approved applicant's survey statistics
        /// </summary>
        /// <param name="id"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("applicant/{id}/program/{programId}/survey/stats")]
        [ProducesResponseType(typeof(SuccessResponse<GetApproveApplicantSurveyStatsDTO>), 200)]
        public async Task<IActionResult> GetApprovedApplicantSurveyStats(Guid id,Guid programId)
        {
            var response = await _service.SurveyService.GetApprovedApplicantSurveyStats( programId, id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get approved-applicant's survey and survey stats
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("applicant/{id}/surveys")]
        [ProducesResponseType(typeof(SuccessResponse<ICollection<GetApprovedApplicantSurveysDto>>), 200)]
        public async Task<IActionResult> GetApplicantSurvey(Guid id)
        {
            var response = await _service.PeopleService.GetApplicantSurvey(id);

            return Ok(response);
        }
    }
}

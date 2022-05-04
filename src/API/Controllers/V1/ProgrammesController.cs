using Application.Contracts.V1;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace API.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/programmes")]
    [ApiController]
    public class ProgrammesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public ProgrammesController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create a program
        /// Delivery method(TimeBound, SelfPaced)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<GetProgrammeDTO>), 200)]
        public async Task<IActionResult> CreateProgramme(CreateProgrammeRequest request)
        {
            var response = await _service.ProgrammeService.CreateProgram(request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get program categories
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("categories", Name = nameof(GetCategories))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetProgrammeCategory>>), 200)]
        public async Task<IActionResult> GetCategories([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgramCategories(parameter, nameof(GetCategories), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a program
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetProgrammeDTO>), 200)]
        public async Task<IActionResult> GeProgrammeById(Guid id)
        {
            var response = await _service.ProgrammeService.GetProgrammeById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a program
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> DeleteProgramme(Guid id)
        {
            var response = await _service.ProgrammeService.DeleteProgramme(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a program
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"/>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetProgrammeDTO>), 200)]
        public async Task<IActionResult> UpdateProgramme(Guid id, UpdateProgrammeRequest request)
        {
            var response = await _service.ProgrammeService.UpdateProgramme(id, request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all programmes
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetProgrammes))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllProgrammmeDTO>>), 200)]
        public async Task<IActionResult> GetProgrammes([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammes(parameter, nameof(GetProgrammes), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get programme Stat
        /// </summary>
        /// <returns></returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetProgrammeStatsDTO>>), 200)]
        public async Task<IActionResult> GetProgrammeStat()
        {
            var response = await _service.ProgrammeService.GetProgrammeStat();

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all learning tracks
        /// </summary>
        /// <param name="id"/>
        /// <param name="parameter"/>
        /// <returns></returns>
        [HttpGet("{id}/learning-track", Name = nameof(GetLearningTracks))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetLearningTrackDTO>>), 200)]
        public async Task<IActionResult> GetLearningTracks(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.LearningTrackService.GetLearningTracks(id, parameter, nameof(GetLearningTracks), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get learning track by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("learning-track/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetLearningTrackDTO>), 200)]
        public async Task<IActionResult> GetLearningTrackById(Guid id)
        {
            var response = await _service.LearningTrackService.GetLearningTrackById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create learning track
        /// </summary>
        /// <param name="id"/>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpPost("{id}/learning-track")]
        [ProducesResponseType(typeof(SuccessResponse<GetLearningTrackDTO>), 200)]
        public async Task<IActionResult> CreateLearningTrack(Guid id, CreateLearningTrackRequest request)
        {
            var response = await _service.LearningTrackService.CreateLearningTrack(id, request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update a learning track
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"/>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpPut("learning-track/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetLearningTrackDTO>), 200)]
        public async Task<IActionResult> UpdateLearningTrack(Guid id, UpdateLearningTrackRequest request)
        {
            var response = await _service.LearningTrackService.UpdateLearningTrack(id, request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete a learning track
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpDelete("learning-track/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> DeleteLearningTrack(Guid id)
        {
            var response = await _service.LearningTrackService.DeleteLearningTrack(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get program sponsors
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("sponsors", Name = nameof(GetSponsors))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetProgrammeSponsor>>), 200)]
        public async Task<IActionResult> GetSponsors([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammeSponsor(parameter, nameof(GetSponsors), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create program sponsor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPost("sponsors")]
        [ProducesResponseType(typeof(SuccessResponse<GetProgrammeSponsor>), 200)]
        public async Task<IActionResult> CreatProgrammeSponsor(CreateProgrammeSponsorRequest request)
        {
            var response = await _service.ProgrammeService.CreateSponsor(request);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all programme managers
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("managers", Name = nameof(GetProgrammeManagers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ManagerDTO>>), 200)]
        public async Task<IActionResult> GetProgrammeManagers([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammeManagers(parameter, nameof(GetProgrammeManagers), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all programme managers
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/managers", Name = nameof(GetManagersByProgrammeId))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ManagerDTO>>), 200)]
        public async Task<IActionResult> GetManagersByProgrammeId(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammeManagers(id, parameter, nameof(GetProgrammeManagers), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all facilitators
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("learning-track/facilitators", Name = nameof(GetFacilitaors))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllFacilitatorsDto>>), 200)]
        public async Task<IActionResult> GetFacilitaors([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.LearningTrackService.GetFacilitators(parameter, nameof(GetFacilitaors), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all facilitators by learning-track
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("learning-track/{id}/facilitators", Name = nameof(GetFacilitaorsByLearningTrackId))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllFacilitatorsDto>>), 200)]
        public async Task<IActionResult> GetFacilitaorsByLearningTrackId(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.LearningTrackService.GetFacilitatorsByLearningTrack(id, parameter, nameof(GetFacilitaorsByLearningTrackId), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a program preview
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/preview")]
        [ProducesResponseType(typeof(SuccessResponse<ProgrammePreviewDTO>), 200)]
        public async Task<IActionResult> GetProgrammePreview(Guid id)
        {
            var response = await _service.ProgrammeService.GetProgrammePreview(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a program target stat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/target-stat")]
        [ProducesResponseType(typeof(SuccessResponse<ProgrammeTargetStatDto>), 200)]
        public async Task<IActionResult> GetBeneficiaryFacilitatorStat(Guid id)
        {
            var response = await _service.ProgrammeService.GetBeneficiaryFacilitatorStat(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a learning track stat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet("learning-track/{id}/stat")]
        [ProducesResponseType(typeof(SuccessResponse<LearningTrackStatDTO>), 200)]
        public async Task<IActionResult> GetLearningTrackStat(Guid id)
        {
            var response = await _service.LearningTrackService.GetLearningTrackStat(id);
            return Ok(response);
        }

        [HttpGet("{id}/learning-track-stat")]
        [ProducesResponseType(typeof(SuccessResponse<List<ProgLearningTrackStat>>), 200)]
        public async Task<IActionResult> GetProgrammeLearningTrackStat(Guid id)
        {
            var response = await _service.ProgrammeService.GetLearningTrackStat(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all program beneficiaries
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/beneficiaries", Name = nameof(GetProgramBeneficiaries))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ProgramApplicantDTO>>), 200)]
        public async Task<IActionResult> GetProgramBeneficiaries(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammeBeneficiaries(id, parameter, nameof(GetProgramBeneficiaries), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all program facilitators
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/facilitators", Name = nameof(GetFacilitatorsByProgrammeId))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<ProgramApplicantDTO>>), 200)]
        public async Task<IActionResult> GetFacilitatorsByProgrammeId(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.ProgrammeService.GetProgrammeFacilitators(id, parameter, nameof(GetProgramBeneficiaries), Url);

            return Ok(response);
        }

        /// <summary> 
        /// Endpoints to get all applicants by learingtrackId 
        /// </summary>
        /// <param name="parameter"></param>
        ///  <param name="learningTrackId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("applicants/{learningTrackId}", Name = nameof(GetApplicantsByLearningTrack))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetLearningTrackApplicantsDTO>>), 200)]
        public async Task<IActionResult> GetApplicantsByLearningTrack(Guid learningTrackId, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.LearningTrackService.GetLearningTrackApplicants(learningTrackId, parameter, nameof(GetApplicantsByLearningTrack), Url);

            return Ok(response);
        }        

        
        /// <summary>
        /// Endpoint to get a other learning track stats
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpGet("other-learning-track")]
        [ProducesResponseType(typeof(SuccessResponse<List<OtherLearningTrackDTO>>), 200)]
        public async Task<IActionResult> OtherLearningTracks()
        {
            var response = await _service.LearningTrackService.OtherLearningTracks();

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a other learning track stats for a programme 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpGet("{id}/other-learning-track")]
        [ProducesResponseType(typeof(SuccessResponse<List<OtherLearningTrackDTO>>), 200)]
        public async Task<IActionResult> OtherLearningTracksByProgrammeId(Guid id)
        {
            var response = await _service.LearningTrackService.OtherLearningTracksByProgramme(id);

            return Ok(response);
        }
    }
}

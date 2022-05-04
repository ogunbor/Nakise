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
    [Route("api/v{version:apiVersion}/events")]
    public class EventsController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EventsController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to create an event
        /// </summary>
        /// <param name="eventDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<GetEventDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateEvent(CreateEventDTO eventDTO)
        {
            var response = await _service.EventService.CreateEvent(eventDTO);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get scheduled events with learning tracks user belongs to in those events
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<GetAllEventDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ScheduledEvents(Guid userId)
        {
            var response = await _service.EventService.GetScheduledEvents(userId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get upcoming events
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpGet("users/{userId}/upcoming")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<GetEventDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUpcomingEvents(Guid userId)
        {
            var response = await _service.EventService.GetUpcomingEvents(userId);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create an event's detail
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventDetailDto"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin,Facilitator")]
        [HttpPut("{id}/details")]
        [ProducesResponseType(typeof(SuccessResponse<GetEventDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddEventDetail(Guid id, [FromForm] CreateEventDetailInputDto eventDetailDto)
        {
            var response = await _service.EventService.AddEventDetail(id, eventDetailDto);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all events
        /// </summary>
        /// <param name="parameter"/>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet(Name = nameof(GetEvents))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllEventDto>>), 200)]
        public async Task<IActionResult> GetEvents([FromQuery] EventResourceParameter parameter)
        {
            var response = await _service.EventService.GetEventsAsync(parameter, nameof(GetEvents), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get an assessment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetEventDTO>), 200)]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var response = await _service.EventService.GetEvent(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update an event
        /// </summary>
        /// <param name="eventDTO"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GetEventDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateEvent(Guid id, UpdateEventDTO eventDTO)
        {
            var response = await _service.EventService.UpdateEvent(id, eventDTO);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete an event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _service.EventService.DeleteEvent(id);
            return NoContent();
        }

        /// <summary>
        /// Endpoint to register for event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/register")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> RegisterForEvent(Guid id, RegisterForEventDTO model)
        {
            var response = await _service.EventService.RegisterForEvent(id, model);

            return Ok(response);
        }


        /// <summary>
        /// Endpoint to cancel an event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(SuccessResponse<string>), 200)]
        public async Task<IActionResult> CancelAnEvent(Guid id, CancelEventDTO model)
        {
            var response = await _service.EventService.CancelAnEvent(id, model);

            return Ok(response);
        }


        /// <summary>
        /// Endpoints to get an event register (participants of an event)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("{id}/applicants", Name = nameof(GetEventApplicants))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetEventApplicantsDTO>>), 200)]
        public async Task<IActionResult> GetEventApplicants(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.EventService.GetEventApplicants(id, parameter, nameof(GetEventApplicants), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get registered events for beneficiaries
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpPut("beneficiaries", Name = nameof(GetRegisteredEvents))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetRegisteredBeneficaryDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRegisteredEvents([FromQuery] ResourceParameter parameters)
        {
            var response = await _service.EventService.GetRegisteredBeneficiaries(parameters, nameof(GetRegisteredEvents), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to download registered beneficiaries as csv
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager,Admin")]
        [HttpGet("beneficiaries/download-csv")]
        [Produces("application/csv")]
        public async Task<IActionResult> GetRegisteredBeneficiariesCsv()
        {
            var stream = await _service.EventService.GetRegisteredBeneficiariesCsv();

            return File(stream, "application/csv", "loan-request.csv");
        }
    }
}

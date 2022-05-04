using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;

namespace Application.Contracts.V1.Activities
{
    public interface IEventService : IAutoDependencyService
    {
        Task<SuccessResponse<GetEventDTO>> CreateEvent(CreateEventDTO request);
        Task<SuccessResponse<GetEventDTO>> AddEventDetail(Guid id, CreateEventDetailInputDto request);
        Task<SuccessResponse<GetEventDTO>> UpdateEvent(Guid eventId, UpdateEventDTO request);
        Task<SuccessResponse<GetEventDTO>> GetEvent(Guid eventId);
        Task<PagedResponse<IEnumerable<GetAllEventDto>>> GetEventsAsync(EventResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<object>> DeleteEvent(Guid eventId);
        Task<SuccessResponse<string>> RegisterForEvent(Guid eventId, RegisterForEventDTO model);
        Task<SuccessResponse<string>> CancelAnEvent(Guid eventId, CancelEventDTO cancelEventDTO);
        Task<PagedResponse<IEnumerable<GetEventApplicantsDTO>>> GetEventApplicants(Guid eventId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<GetRegisteredBeneficaryDto>>> GetRegisteredBeneficiaries(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<IEnumerable<GetAllEventDto>>> GetScheduledEvents(Guid userId);
        Task<SuccessResponse<IEnumerable<GetEventDto>>> GetUpcomingEvents(Guid userId);
        Task<MemoryStream> GetRegisteredBeneficiariesCsv();
    }
}
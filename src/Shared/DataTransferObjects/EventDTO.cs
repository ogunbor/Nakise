using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public record CreateEventDTO
    {
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EventTime { get; set; }
        public bool? IsOnline { get; set; }
        public string EventLink { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public record GetEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BannerUrl { get; set; }
        public string Status { get; set; }
        public bool? IsOnline { get; set; }
        public DateTime EventTime { get; set; }
        public string EventLink { get; set; }
    }

    public record GetAllEventDto : GetEventDto
    {
        public Guid CreatedById { get; set; }
        public IEnumerable<GetLearningTrackDto> LearningTracks { get; set; }
    }

    public record GetEventDTO
    {
        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EventTime { get; set; }
        public string EventLink { get; set; }
        public string BannerUrl { get; set; }
        public string EventDetail { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<GetLearningTrackEventDTO> LearningTracks { get; set; }
    }

    public record UpdateEventDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EventTime { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public record GetLearningTrackEventDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetEventApplicantsDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisteredAt { get; set; }
        public GetLearningTrackDto LearningTrack { get; set; }
    }

    public record RegisterForEventDTO
    {
        public Guid ApplicantId { get; set; }
    }

    public record CreateEventDetailInputDto
    {
        public IFormFile Banner { get; set; }
        public string Details { get; set; }
    }

    public record GetRegisteredBeneficaryDto : GetRegisteredBeneficaryCsvDto
    {
        public IEnumerable<GetLearningTrackDto> LearningTracks { get; set; }
    }

    public record GetRegisteredBeneficaryCsvDto
    {
        public Guid EventId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisteredDate { get; set; }
    }

    public record CancelEventDTO
    {
        public string ReasonForCancellation { get; set; }
    }
}
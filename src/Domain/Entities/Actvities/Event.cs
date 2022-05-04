using Domain.Common;
using Domain.Entities.Activities;

namespace Domain.Entities.Actvities
{
    public class Event : AuditableEntity
    {
        public Event()
        {
            EventLearningTracks = new HashSet<EventLearningTrack>();
        }

        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public Guid CreatedById { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EventTime { get; set; }
        public bool? IsOnline { get; set; }
        public string ReasonForCancellation { get; set; }
        public string Status { get; set; }
        public string EventLink { get; set; }
        public string BannerUrl { get; set; }
        public string EventDetail { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public Activity Activity { get; set; }
        public Programme Programme { get; set; }
        public User CreatedBy { get; set; }
        public ICollection<EventLearningTrack> EventLearningTracks { get; set; }
    }
}

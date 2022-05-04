namespace Domain.Entities.Actvities
{
    public class EventLearningTrack
    {
        public Guid EventId { get; set; }
        public Guid LearningTrackId { get; set; }
        public Event Event { get; set; }
        public LearningTrack LearningTrack { get; set; }
    }
}

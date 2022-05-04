using Domain.Entities.Activities;

namespace Domain.Entities.Actvities
{
    public class AssessmentLearningTrack
    {
        public Guid AssessmentId { get; set; }
        public Guid LearningTrackId { get; set; }
        public Assessment Assessment { get; set; }
        public LearningTrack LearningTrack { get; set; }
    }
}

using Domain.Common;
using Domain.Entities.Actvities;

namespace Domain.Entities.Activities
{
    public class Assessment : AuditableEntity
    {
        public Assessment()
        {
            AssessmentLearningTracks = new HashSet<AssessmentLearningTrack>();
        }

        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string AssessmentId { get; set; }
        public string Status { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int TotalObtainableScore { get; set; }
        public int PassMark { get; set; }
        public int NoOfSubmission { get; set; }
        public string? CompletionTitle { get; set; }
        public string? CompletionMessage { get; set; }
        public Activity Activity { get; set; }
        public Programme Programme { get; set; }
        public ICollection<AssessmentLearningTrack> AssessmentLearningTracks { get; set; }
    }
}

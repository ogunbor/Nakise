using Domain.Entities.Activities;

namespace Domain.Entities.Actvities
{
    public class AssessmentSession
    {
        public Guid Id { get; set; }
        public Guid AssessmentId { get; set; }
        public Guid ApprovedApplicantId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SessionId { get; set; }
        public Assessment Assessment { get; set; }
        public ApprovedApplicant ApprovedApplicant { get; set; }
    }
}

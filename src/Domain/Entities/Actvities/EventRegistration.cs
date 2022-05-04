using Domain.Common;

namespace Domain.Entities.Actvities
{
    public class EventRegistration : AuditableEntity
    {
        public Guid EventId { get; set; }
        public Guid ApprovedApplicantId { get; set; }
        public Event Event { get; set; }
        public ApprovedApplicant ApprovedApplicant { get; set; }
    }
}

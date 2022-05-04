using Domain.Common;
using Domain.Entities.Activities;

namespace Domain.Entities.Actvities
{
    public class CallForApplication : AuditableEntity
    {
        public CallForApplication()
        {
            Stages = new HashSet<Stage>();
        }

        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Target { get; set; }
        public int TargetNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStage { get; set; }
        public bool IsClosed { get; set; }
        public string? Status { get; set; }
        public string? SuccessMessageTitle { get; set; }
        public string? SuccessMessageBody { get; set; }
        public Activity Activity { get; set; }
        public Programme Programme { get; set; }
        public ICollection<Stage> Stages { get; set; }
    }
}

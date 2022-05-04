using Domain.Common;
using Domain.Entities.Activities;


namespace Domain.Entities.Actvities
{
    public class SurveyResponse : AuditableEntity
    {
        public Guid ProgramId { get; set; }
        public Guid SurveyId { get; set; }
        public Guid ApprovedApplicantId { get; set; }
        public Programme Programme { get; set; }
        public Survey Survey { get; set; }
        public ApprovedApplicant ApprovedApplicant { get; set; }
    }
}
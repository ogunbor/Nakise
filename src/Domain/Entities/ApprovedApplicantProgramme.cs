namespace Domain.Entities
{
    public class ApprovedApplicantProgramme
    {
        public Guid? LearningTrackId { get; set; }
        public Guid ApprovedApplicantId { get; set; }
        public Guid ProgrammeId { get; set; }
        public ApprovedApplicant ApprovedApplicant { get; set; }
        public Programme Programme { get; set; }
        public LearningTrack learningTrack { get; set; }
    }
}

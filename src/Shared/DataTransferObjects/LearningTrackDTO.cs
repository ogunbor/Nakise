namespace Shared.DataTransferObjects
{
    public class CreateLearningTrackRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Guid> Facilitators { get; set; } = new List<Guid>();
    }
    public class GetLearningTrackDTO
    {
        public Guid Id { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FacilitatorDTO> Facilitators { get; set; }
    }

    public class LearningTrackFacilitatorDTO
    {
        public Guid LearningTrackId { get; set; }
        public Guid FacilitatorId { get; set; }
    }
    public class UpdateLearningTrackRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Guid> Facilitators { get; set; } = new List<Guid>();
    }
    public class FacilitatorDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
    }

    public class GetLearningTrackDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetAllFacilitatorsDto : FacilitatorDTO
    {
        public GetLearningTrackDto LearningTrack { get; set; }
    }

    public class GetLearningTrackApplicantsDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
    }
    public class LearningTrackGenderStat
    {
      
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
    public class LearningTrackStatDTO
    {

        public string Title { get; set; }
        public int TotalCount { get; set; }
        public string Description { get; set; }
        public LearningTrackGenderStat Male { get; set; }
        public LearningTrackGenderStat Female { get; set; }
        public LearningTrackGenderStat Others { get; set; }
        public LearningTrackGenderStat NotSpecified { get; set; }


    }
    public class OtherLearningTrackDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int ApplicantCount { get; set; }

    }
}

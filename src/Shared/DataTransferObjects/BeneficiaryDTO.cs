

namespace Shared.DataTransferObjects
{
    public class BeneficiaryDTO : UpdateBeneficiaryDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Picture { get; set; }
        public string Status { get; set; }
        public ICollection<ProgrammeDto> Programmes { get; set; }
    }

    public class UpdateBeneficiaryDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string AccessToLaptop { get; set; }
        public string SocialMedia { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class BeneficiaryProgrammeDTO
    {
        public Guid Id { get; set;}
        public string Title { get; set; } 
        public string Sponsor {get; set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TimeLine { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string LearningTrack { get; set; }
        public ICollection<ManagerDTO> ProgrammeManagers { get; set; }
    }

    public class BeneficiaryProgrammesStatsDTO : BeneficiaryStatsDTO
    {
        public int Ongoing { get; set; }
        public int Completed { get; set; }
    }

    public class BeneficiaryStatsDTO
    {
        public int Total { get; set; }  
    }

    public class BeneficiaryProgrammeAndLearningTrackStatsDTO : BeneficiaryProgrammeDTO
    {
        public string Description { get; set; }
        public string Country { get; set; }
        public BeneficiaryLearningTrackStatsDTO LearningTrackStats { get; set; }
    }

    public class BeneficiaryLearningTrackStatsDTO
    {
        public int TotalBeneficiaries { get; set; }
        public int TotalFacilitators { get; set; }
    }

}

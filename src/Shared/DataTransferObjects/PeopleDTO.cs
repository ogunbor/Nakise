namespace Shared.DataTransferObjects
{
    public class GetPeopleStatsDto
    {
        public int BeneficiaryTotalCount { get; set; }
        public int FacilitatorTotalCount { get; set; }
        public int ProgramManagerTotalCount { get; set; }
        public int SponsorTotalCount { get; set; }
        public int MentorTotalCount { get; set; }
        public int AlumniTotalCount { get; set; }
        public int JobPartnerTotalCount { get; set; }
    }

    public record GetProgrammeManagerStatDto
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PendingCount { get; set; }
        public int DisabledCount { get; set; }
    }

    public class GetProgrammeManagerDto : GetSingleProgrammeManagerDto
    {
        public ICollection<ProgrammeDto> Programmes { get; set; }
    }

    public class GetSingleProgrammeManagerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record GetFacilitatorDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<FacilitatorProgrammeDto> Programmes { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record FacilitatorProgrammeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public record GetUserProgrammeDto
{
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Sponsor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public ICollection<ProgrammeManagerDto> ProgrammeManagers { get; set; }
    }

    public record ProgrammeManagerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
    }

    public class GetFacilitatorStatsDto : GetUserStatsDto
    {
    }
}

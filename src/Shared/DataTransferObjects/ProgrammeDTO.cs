using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class CreateProgrammeRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DeliveryMethod { get; set; }
        public bool HasLearningTrack { get; set; }
        public string Sponsor { get; set; }
        public bool HasGender { get; set; }
        public List<string> GenderOption { get; set; }
        public bool HasAge { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public List<Guid> Managers { get; set; } = new List<Guid>();
    }

    public class GetProgrammeDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DeliveryMethod { get; set; }
        public bool HasLearningTrack { get; set; }
        public string Sponsor { get; set; }
        public List<string> GenderOption { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string StateOrProvince { get; set; }
        public Guid CreatedById { get; set; }
        public Guid OrganizationId { get; set; }
        public List<ManagerDTO> Managers { get; set; }
    }

    public class ManagerDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
    }

    public class ProgrammeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetAllProgrammeManagerDto : ManagerDTO
    {
        public ProgrammeDto Programme { get; set; }
    }

    public class UpdateProgrammeRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DeliveryMethod { get; set; }
        public bool HasLearningTrack { get; set; }
        public string Sponsor { get; set; }
        public bool HasGender { get; set; }
        public List<string> GenderOption { get; set; }
        public bool HasAge { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public List<Guid> Managers { get; set; } = new List<Guid>();
    }

    public class GetAllProgrammmeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Sponsor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ManagerDTO> Managers { get; set; }
    }

    public class GetProgrammeStatsDTO
    {
        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
        public int OngoingCount { get; set; }
        public int NotStartedCount { get; set; }
    }

    public class GetProgrammeCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class GetProgrammeSponsor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateProgrammeSponsorRequest
    {
        public string Name { get; set; }
    }

    public class ProgrammePreviewDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Sponsor { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string StateOrProvince { get; set; }
        public List<ManagerDTO> Managers { get; set; }
        public CreatedBy CreatedBy { get; set; }
        public int LearningTrackCount { get; set; }
    }
    public class TargetGroup
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
    public class GenderTargetGroup
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class TargetStatusDto
    {
        public int Total { get; set; }
        public List<TargetGroup> Type { get; set; }
    }

    public class GenderTargetStatusDto
    {
        public List<GenderTargetGroup> Gender { get; set; }
        public TargetStatusDto Target { get; set; }
    }

    public class ProgrammeTargetStatDto
    {
        public GenderTargetStatusDto Beneficiaries { get; set; }
        public GenderTargetStatusDto Facilitators { get; set; }
    }

    public class LearningTrackGroup
    {
        public Guid Id { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
    }

    public class ProgLearningTrackStat
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int BeneficiaryCount { get; set; }

        public int FacilitatorCount { get; set; }
        
    }
    public class ProgramApplicantDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string LearningTrack { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
    }

    public record CallForApplicationParticipantDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LearningTrack { get; set; }
        public string Status { get; set; }
        public int StageIndex { get; set; }
        public int CurrentStageCount { get; set; }
        public Guid FormId { get; set; }
    }
}

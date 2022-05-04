using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface IPeopleService : IAutoDependencyService
    {
        Task<PagedResponse<IEnumerable<BeneficiaryDTO>>> GetBeneficiaries(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<BeneficiaryStatsDTO>> GetBeneficiariesStat();
        Task<SuccessResponse<BeneficiaryDTO>> GetBeneficiaryById(Guid id);
        Task<SuccessResponse<GetPeopleStatsDto>> GetPeopleStats();
        Task<SuccessResponse<GetProgrammeManagerStatDto>> GetProgrammeManagerStatAsync();
        Task<PagedResponse<IEnumerable<GetProgrammeManagerDto>>> GetAllProgrammeManagersAsync(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<GetFacilitatorDto>>> GetAllFacilitatorsAsync(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<ICollection<GetUserProgrammeDto>>> GetUserProgrammesAsync(Guid userId, string search);
        Task<SuccessResponse<GetSingleProgrammeManagerDto>> GetProgrammeManagerAsync(Guid userId);
        Task<SuccessResponse<BeneficiaryDTO>> UpdateBeneficiary(Guid id, UpdateBeneficiaryDTO model);
        Task<SuccessResponse<GetFacilitatorStatsDto>> GetFacilitatorsStat();
        Task<SuccessResponse<IEnumerable<BeneficiaryProgrammeDTO>>> GetBeneficiaryProgrammes(Guid id);
        Task<SuccessResponse<BeneficiaryProgrammesStatsDTO>> GetBeneficiaryProgrammesStats(Guid BeneficiaryId);
        Task<SuccessResponse<BeneficiaryProgrammeAndLearningTrackStatsDTO>> GetBeneficiaryProgrammeDetails(Guid beneficiaryId, Guid programmeId);
        Task<SuccessResponse<ICollection<GetEventDTO>>> GetBeneficiaryProgrammeEvents(Guid programId);
        Task<SuccessResponse<ICollection<GetApprovedApplicantSurveysDto>>> GetApplicantSurvey(Guid approvedApplicantId);
    }
}

using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface IProgrammeService: IAutoDependencyService
    {
        Task<PagedResponse<IEnumerable<GetProgrammeCategory>>> GetProgramCategories(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<GetProgrammeDTO>> CreateProgram(CreateProgrammeRequest request);
        Task<SuccessResponse<GetProgrammeDTO>> GetProgrammeById(Guid programmeId);
        Task<SuccessResponse<object>> DeleteProgramme(Guid programmeId);
        Task<SuccessResponse<GetProgrammeDTO>> UpdateProgramme(Guid programmeId, UpdateProgrammeRequest request);
        Task<PagedResponse<IEnumerable<GetAllProgrammmeDTO>>> GetProgrammes(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<GetProgrammeStatsDTO>> GetProgrammeStat();
        Task<PagedResponse<IEnumerable<GetProgrammeSponsor>>> GetProgrammeSponsor(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<GetProgrammeSponsor>> CreateSponsor(CreateProgrammeSponsorRequest request);
        Task<PagedResponse<IEnumerable<ManagerDTO>>> GetProgrammeManagers(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<ManagerDTO>>> GetProgrammeManagers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<ProgrammePreviewDTO>> GetProgrammePreview(Guid programmeId);
        Task<PagedResponse<IEnumerable<ProgramApplicantDTO>>> GetProgrammeBeneficiaries(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<ProgramApplicantDTO>>> GetProgrammeFacilitators(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<ProgrammeTargetStatDto>> GetBeneficiaryFacilitatorStat(Guid programmeId);
        Task<SuccessResponse<List<ProgLearningTrackStat>>> GetLearningTrackStat(Guid programmeId);
    }
}

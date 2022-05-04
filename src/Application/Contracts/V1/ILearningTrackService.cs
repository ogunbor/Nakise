using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface ILearningTrackService: IAutoDependencyService
    {
        Task<PagedResponse<IEnumerable<GetLearningTrackDTO>>> GetLearningTracks(Guid programmeId, ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<GetLearningTrackDTO>> GetLearningTrackById(Guid learningTrackId);
        Task<SuccessResponse<GetLearningTrackDTO>> CreateLearningTrack(Guid programmeId, CreateLearningTrackRequest request);
        Task<SuccessResponse<GetLearningTrackDTO>> UpdateLearningTrack( Guid id, UpdateLearningTrackRequest request);
        Task<SuccessResponse<object>> DeleteLearningTrack(Guid learningTrackid);
        Task<PagedResponse<IEnumerable<GetAllFacilitatorsDto>>> GetFacilitators(ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<GetAllFacilitatorsDto>>> GetFacilitatorsByLearningTrack(Guid learningTrackId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<PagedResponse<IEnumerable<GetLearningTrackApplicantsDTO>>> GetLearningTrackApplicants(Guid learningTrackId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<LearningTrackStatDTO>> GetLearningTrackStat(Guid learningTrackId);
        Task<SuccessResponse<List<OtherLearningTrackDTO>>> OtherLearningTracks();
        Task<SuccessResponse<List<OtherLearningTrackDTO>>> OtherLearningTracksByProgramme(Guid programmeId);
    }
}

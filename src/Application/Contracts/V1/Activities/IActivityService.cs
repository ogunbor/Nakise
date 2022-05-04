using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1.Activities
{
    public interface IActivityService : IAutoDependencyService
    {
        Task<SuccessResponse<IEnumerable<GetActivityDTO>>> GetActivities();
        Task<SuccessResponse<GetActivityDTO>> GetActivityById(Guid id);

        Task<PagedResponse<IEnumerable<GetProgrammeActivitiesDTO>>> GetProgrammeActivities(Guid programmeId,
            ResourceParameter parameter, string name, IUrlHelper urlHelper);
        void CheckActivityDate(DateTime endDate);
    }
}

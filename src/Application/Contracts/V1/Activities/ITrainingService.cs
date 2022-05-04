using Application.Helpers;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1.Activities
{
    public interface ITrainingService : IAutoDependencyService
    {
        Task<SuccessResponse<GetTrainingDto>> CreateTrainingActivity(CreateTrainingInputDto model);
        Task<SuccessResponse<GetTrainingDto>> GetTrainingActivity(Guid id);
        Task DeleteTrainingActivity(Guid id);
        Task<SuccessResponse<GetTrainingDto>> UpdateTrainingActivity(Guid id, UpdateTrainingInputDto model);
    }
}

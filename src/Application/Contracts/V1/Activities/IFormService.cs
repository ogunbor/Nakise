using Application.DTOs;
using Application.Helpers;

namespace Application.Contracts.V1.Activities
{
    public interface IFormService : IAutoDependencyService
    {
        Task<SuccessResponse<GetFormDTO>> CreateForm(CreateFormDTO request);
        Task<SuccessResponse<GetFormDTO>> UpdateForm(Guid formId, UpdateFormDTO request);
        Task<SuccessResponse<GetFormDTO>> GetForm(Guid formId);
        Task<SuccessResponse<object>> DeleteForm(Guid formId);
    }
}
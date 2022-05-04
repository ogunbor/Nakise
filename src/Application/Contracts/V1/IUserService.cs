using Application.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using static Shared.DataTransferObjects.UpdateAdmininistratorDTO;

namespace Application.Contracts.V1
{
    public interface IUserService : IAutoDependencyService
    {
        public Task<SuccessResponse<CreateUserResponse>> InviteUser(CreateUserInputDTO model);
        Task<SuccessResponse<GetUserProfileDto>> GetUserById(Guid userId);
        Task<SuccessResponse<GetUserStatsDto>> GetUsersStat();
        Task<PagedResponse<IEnumerable<GetAllUserDto>>> GetAllUsers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<GetProgrammesByUserDto>> GetAllProgrammesByUser(Guid userId);
        Task<PagedResponse<IEnumerable<GetAllAdminUserDto>>> GetAllAdminUsers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task<SuccessResponse<GetAdministratorStatsDTO>> GetAdmininistratorStat();
        Task<SuccessResponse<GetAdminUserDto>> ActivateUser(Guid userId);
        Task<SuccessResponse<GetAdminUserDto>> SuspendUser(Guid userId);
        Task<SuccessResponse<UpdateAdminResponse>> EditAdmin(Guid userId, UpdateAdmininistratorDTO adminDto);
        Task<string> UploadFile(IFormFile file);
        Task<PagedResponse<IEnumerable<GetUserActivityDto>>> GetUserActivitiesAsync(Guid userId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper);
        Task DeleteDocument(Guid id);
        Task<SuccessResponse<IEnumerable<UserDocumentDto>>> UploadDocuments(Guid userId, ICollection<IFormFile> files);
        Task<SuccessResponse<IEnumerable<GetUserDocumentDto>>> GetUserDocuments(Guid userId);
    }
}
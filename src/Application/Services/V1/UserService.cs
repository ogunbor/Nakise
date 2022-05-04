using Application.Contracts.V1;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.DataTransferObjects;
using System.Net;
using static Shared.DataTransferObjects.UpdateAdmininistratorDTO;

namespace Application.Services.V1.Implementations
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailManager _mailerService;
        private readonly IRepositoryManager _repository;
        private readonly IAwsS3Client _awsS3Client;
        private readonly IWebHelper _webHelper;

        public UserService(
            IMapper mapper,
            IConfiguration configuration,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IEmailManager mailerService,
            IAwsS3Client awsS3Client,
            IWebHelper webHelper,
            IRepositoryManager repository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailerService = mailerService;
            _awsS3Client = awsS3Client;
            _webHelper = webHelper;
            _repository = repository;
        }

        public async Task<SuccessResponse<CreateUserResponse>> InviteUser(CreateUserInputDTO model)
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var email = model.Email.Trim().ToLower();
            var isEmailExist = await _repository.User.ExistsAsync(x => x.Email == email);
            if (isEmailExist)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.DuplicateEmail);

            var role = await _roleManager.FindByNameAsync(model.Role);
            if (role == null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.RoleNotFound);

            var user = _mapper.Map<User>(model);
            user.Status = EUserStatus.Pending.ToString();
            user.UserName = user.Email;
            user.Verified = false;
            user.OrganizationId = organizationId;
            user.Password = _userManager.PasswordHasher.HashPassword(user, "Password@1");

            var result = await _userManager.CreateAsync(user, "Password@1");
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, result.Errors.FirstOrDefault().Description);

            await _userManager.AddToRoleAsync(user, model.Role);

            var token = CustomToken.GenerateRandomString(128);
            var tokenEntity = new Token
            {
                UserId = user.Id,
                Value = token,
                TokenType = ETokenType.InviteUser.ToString()
            };

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Invited a User", user.Id);

            var userInfoId = await CreateUserProfile(user);
            var userToUpdate = await _repository.User.GetByIdAsync(user.Id);
            userToUpdate.UserInformationId = userInfoId;

            _repository.User.Update(userToUpdate);
            await _repository.Token.AddAsync(tokenEntity);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            string emailLink = $"{_configuration["CLIENT_URL"]}/signup?token={token}";

            var message = string.Empty;
            string subject = "Confirm Email";

            if (role.Name.Equals(ERole.ProgramManager.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                message = _mailerService.GetProgrammeManagerInvitationTemplate(emailLink, user.FirstName);
            }
            else
            {
                message = _mailerService.GetConfirmEmailTemplate(emailLink, user.Email);
            }

            _mailerService.SendSingleEmail(user.Email, message, subject);

            var userResponse = _mapper.Map<CreateUserResponse>(user);

            return new SuccessResponse<CreateUserResponse>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = userResponse
            };
        }

        public async Task<Guid> CreateUserProfile(User user)
        {
            var userInfo = new UserInformation
            {
                UserId = user.Id
            };

            await _repository.UserInformation.AddAsync(userInfo);

            return userInfo.Id;
        }

        public async Task<SuccessResponse<GetUserProfileDto>> GetUserById(Guid userId)
        {
            
            var user = await _repository.User.Get(u => u.Id == userId)
                 .Include(u => u.UserInformation).FirstOrDefaultAsync();

            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userResponse = _mapper.Map<GetUserProfileDto>(user);

            return new SuccessResponse<GetUserProfileDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = userResponse
            };
        }    

        public async Task<SuccessResponse<GetUserStatsDto>> GetUsersStat()
        {
            var organizationId = _webHelper.User().OrganizationId;
            var totalCount = await _repository.User.CountAsync(x => x.OrganizationId == organizationId);
            var activeCount = await _repository.User.CountAsync(x => x.Status == EUserStatus.Active.ToString() && x.OrganizationId == organizationId);
            var pendingCount = await _repository.User.CountAsync(x => x.Status == EUserStatus.Pending.ToString() && x.OrganizationId == organizationId);
            var disabledCount = await _repository.User.CountAsync(x => x.Status == EUserStatus.Disabled.ToString() && x.OrganizationId == organizationId);

            var response = new SuccessResponse<GetUserStatsDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetUserStatsDto
                {
                    Total = totalCount,
                    Active = activeCount,
                    Pending = pendingCount,
                    Disabled = disabledCount
                }
            };

            return response;
        }

        public async Task<PagedResponse<IEnumerable<GetUserActivityDto>>> GetUserActivitiesAsync(Guid userId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var usersQuery = _repository.UserActivity
                .QueryAll()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt);

            var usersDto = usersQuery.ProjectTo<GetUserActivityDto>(_mapper.ConfigurationProvider);
            var pagedUsersDto = await PagedList<GetUserActivityDto>.Create(usersDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetUserActivityDto>.CreateResourcePageUrl(parameter, actionName, pagedUsersDto, urlHelper);

            var response = new PagedResponse<IEnumerable<GetUserActivityDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUsersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };

            return response;
        }

        public async Task<PagedResponse<IEnumerable<GetAllUserDto>>> GetAllUsers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var usersQuery = _repository.User.Get(x => x.OrganizationId == organizationId && x.Verified && x.Status == EUserStatus.Active.ToString())
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Programme) as IQueryable<User>;

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim().ToLower();
                usersQuery = usersQuery.Where(x => x.FirstName.ToLower().Contains(search) ||
                    x.LastName.ToLower().Contains(search) ||
                    x.Category.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search) ||
                    x.Status.ToLower().Contains(search));
            }

            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();
            if (!string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                string searchBy = parameter.SearchBy.Trim().ToLower();
                usersQuery = from user in usersQuery
                             join uRole in userRoles on user.Id equals uRole.UserId
                             join role in roles on uRole.RoleId equals role.Id
                             where role.Name.ToLower() == searchBy
                             select user;
            }

            var usersDto = usersQuery.ProjectTo<GetAllUserDto>(_mapper.ConfigurationProvider);
            var pagedUsersDto = await PagedList<GetAllUserDto>.Create(usersDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetAllUserDto>.CreateResourcePageUrl(parameter, actionName, pagedUsersDto, urlHelper);

            var response = new PagedResponse<IEnumerable<GetAllUserDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedUsersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };

            return response;
        }

        public async Task<SuccessResponse<GetProgrammesByUserDto>> GetAllProgrammesByUser(Guid userId)
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var usersQuery = await _repository.User.Get(x => 
                x.OrganizationId == organizationId && 
                x.Id == userId &&
                x.Verified && 
                x.Status == EUserStatus.Active.ToString())
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Programme)
                .FirstOrDefaultAsync();

            var response = _mapper.Map<GetProgrammesByUserDto>(usersQuery);

            return new SuccessResponse<GetProgrammesByUserDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<PagedResponse<IEnumerable<GetAllAdminUserDto>>> GetAllAdminUsers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            Guid organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var usersQuery = _repository.User.Get(x => x.OrganizationId == organizationId) as IQueryable<User>; 
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();
            
            usersQuery = from user in usersQuery
                         join uRoles in userRoles on user.Id equals uRoles.UserId
                         join role in roles on uRoles.RoleId equals role.Id
                         where role.Name == ERole.Admin.ToString()
                         orderby user.CreatedAt descending
                         select user;

            if (!string.IsNullOrWhiteSpace(parameter.Search) && string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                string search = parameter.Search.Trim();
                usersQuery = usersQuery.Where(x => x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(parameter.SearchBy) && !string.IsNullOrWhiteSpace(parameter.Search))
            {
                usersQuery = UsersQuerySearchBy(parameter, usersQuery);
            }

            var usersDto = usersQuery.ProjectTo<GetAllAdminUserDto>(_mapper.ConfigurationProvider);
            var pagedAdminUsersDto = await PagedList<GetAllAdminUserDto>.Create(usersDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetAllAdminUserDto>.CreateResourcePageUrl(parameter, actionName, pagedAdminUsersDto, urlHelper);

            var response = new PagedResponse<IEnumerable<GetAllAdminUserDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedAdminUsersDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };
            
            return response;
        }

        private static IQueryable<User> UsersQuerySearchBy(ResourceParameter parameter, IQueryable<User> usersQuery)
        {
            string searchBy = parameter.SearchBy.Trim().ToLower();
            string search = parameter.Search.Trim().ToLower();

            if (searchBy.Equals("status", StringComparison.OrdinalIgnoreCase))
            {
                usersQuery = usersQuery.Where(x => x.Status.ToLower() == search);
            }
            
            return usersQuery;
        }

        public async Task<SuccessResponse<GetAdministratorStatsDTO>> GetAdmininistratorStat()
        {
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();
            var users = _repository.User.QueryAll();

            var query = from user in users
                        join uRoles in userRoles on user.Id equals uRoles.UserId
                        join role in roles on uRoles.RoleId equals role.Id
                        where role.Name == ERole.Admin.ToString()
                        select new { user };
            var totalCount = await query.CountAsync();

           
            var activeCount = await query.CountAsync(x=>x.user.Status == EUserStatus.Active.ToString());
            var pendingCount = await query.CountAsync(x => x.user.Status == EUserStatus.Pending.ToString());
            var disabledCount = await query.CountAsync(x => x.user.Status == EUserStatus.Disabled.ToString());

            var response = new SuccessResponse<GetAdministratorStatsDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetAdministratorStatsDTO
                {
                    TotalCount = totalCount,
                    ActiveCount = activeCount,
                    PendingCount = pendingCount,
                    DisabledCount = disabledCount,
                  
                }
            };

            return response;
        }

        public async Task<SuccessResponse<GetAdminUserDto>> ActivateUser(Guid userId)
        {
            Guid organizationId = ValidateUser();

            User user = await ActivateOrSuspendUser(userId, organizationId, EUserStatus.Active);

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Activated a User", user.Id);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var userResponse = _mapper.Map<GetAdminUserDto>(user);

            return new SuccessResponse<GetAdminUserDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = userResponse
            };
        }

        private async Task<User> ActivateOrSuspendUser(Guid userId, Guid organizationId, EUserStatus status)
        {
            var user = await _repository.User.FirstOrDefault(x => x.OrganizationId == organizationId && x.Id == userId);

            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            user.Status = status.ToString();
            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task<SuccessResponse<UpdateAdminResponse>> EditAdmin(Guid userId, UpdateAdmininistratorDTO adminDto)
        {
            var user = await getUser(userId);
            bool isEmailExist = false;

            if (!string.IsNullOrEmpty(adminDto.Email))
            {
                isEmailExist = await _repository.User.ExistsAsync(x => x.Email == adminDto.Email.Trim().ToLower());
            }
            
            
            if (user.Email != adminDto.Email && isEmailExist)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.DuplicateEmail);
            _mapper.Map(adminDto, user);
            user.UpdatedAt = DateTime.UtcNow;
           
            user.UserName = user.Email;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, result.Errors.FirstOrDefault().Description);
            var userResponse = _mapper.Map<UpdateAdminResponse>(user);

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Updated a User", user.Id);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<UpdateAdminResponse>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = userResponse
            };
        }
        public async Task<SuccessResponse<GetAdminUserDto>> SuspendUser(Guid userId)
        {
            Guid organizationId = ValidateUser();

            User user = await ActivateOrSuspendUser(userId, organizationId, EUserStatus.Disabled);

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Suspended a User", user.Id);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var userResponse = _mapper.Map<GetAdminUserDto>(user);

            return new SuccessResponse<GetAdminUserDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = userResponse
            };
        }

        private Guid ValidateUser()
        {
            Guid organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);
            return organizationId;
        }

        private async Task< User> getUser(Guid userId)
        {
            var user = await _repository.User.GetByIdAsync(userId);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.UserNotFound}");
            return user;
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            var fileUpload = await _awsS3Client.UploadFileAsync(file);
            return fileUpload;
        }

        public async Task<SuccessResponse<IEnumerable<UserDocumentDto>>> UploadDocuments(Guid userId, ICollection<IFormFile> files)
        {
            User user = await _repository.User.FirstOrDefault(x => x.Id.Equals(userId));

            List<UserDocument> documents = new();

            foreach (var file in files)
            {
                var documentUrl = await _awsS3Client.UploadFileAsync(file);
                var document = new UserDocument
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DocumentName = file.Name,
                    DocumentUrl = documentUrl
                };

                await _repository.UserDocument.AddAsync(document);
                documents.Add(document);
            }

            await _repository.SaveChangesAsync();

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Uploaded documents", user.Id);

            var response = _mapper.Map<IEnumerable<UserDocumentDto>>(documents);

            return new SuccessResponse<IEnumerable<UserDocumentDto>>
            {
                Message = ResponseMessages.DocumentsUploadSuccessful,
                Data = response
            };
        }

        public async Task<SuccessResponse<IEnumerable<GetUserDocumentDto>>> GetUserDocuments(Guid userId)
        {
            var userExists = await _repository.User.ExistsAsync(x => x.Id == userId);
            if (!userExists)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userDocuments = await _repository.UserDocument.FindAsync(x => x.UserId == userId);

            var documentsDto = _mapper.Map<IEnumerable<GetUserDocumentDto>>(userDocuments);
            var response = new SuccessResponse<IEnumerable<GetUserDocumentDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = documentsDto
            };

            return response;
        }

        public async Task DeleteDocument(Guid id)
        {
            var userId = _webHelper.User().UserId;
            User user = await _repository.User.FirstOrDefault(x => x.Id.Equals(userId));

            var document = await _repository.UserDocument.FirstOrDefault(x => x.Id == id && x.UserId == userId);

            if (document is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DocumentNotFound);

            var fileUrl = document.DocumentUrl;
            _repository.UserDocument.Remove(document);
            await _repository.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(fileUrl))
                await _awsS3Client.RemoveObject(fileUrl);

            UserActivity userActivity = AuditLog.UserActivity(user, _webHelper.User().UserId, "User", "Deleted a document", userId);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();
        }
    }
}
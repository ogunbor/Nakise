using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Implementations
{
    public class ProgrammeService: IProgrammeService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;

        public ProgrammeService(
            IMapper mapper,
            IRepositoryManager repository, IWebHelper webHelper)

        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
        }
        public async Task<PagedResponse<IEnumerable<GetProgrammeCategory>>> GetProgramCategories(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var query = _repository.ProgrammeCategory.QueryAll();

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                query = query.Where(x => x.Name.Contains(parameter.Search));
            }

            var categoryQuery = query.ProjectTo<GetProgrammeCategory>(_mapper.ConfigurationProvider);
            var categories = await PagedList<GetProgrammeCategory>.Create(categoryQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetProgrammeCategory>.CreateResourcePageUrl(parameter, name, categories, urlHelper);

            var response = new PagedResponse<IEnumerable<GetProgrammeCategory>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = categories,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<GetProgrammeDTO>> CreateProgram(CreateProgrammeRequest request)
        {
            var managers = await GetManagers(request.Managers);

            await CreateCategory(request.Category);
            var program = _mapper.Map<Programme>(request);
            program.CreatedById = _webHelper.User().UserId;
            program.OrganizationId = _webHelper.User().OrganizationId;

            if (managers.Any())
            {
                var managersIds = managers.Select(x => x.Id);
                program.ProgrammeManagers = GetProgramManagersForCreation(managersIds, program.Id);
            }
           
            await _repository.Programme.AddAsync(program);

            UserActivity userActivity = AuditLog.UserActivity(program, _webHelper.User().UserId, nameof(program), $"Created a new Program - \"{program.Title}\"", program.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var getProgramDTO = _mapper.Map<GetProgrammeDTO>(program);
            getProgramDTO.Managers = managers;

            return new SuccessResponse<GetProgrammeDTO>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = getProgramDTO
            };
        }

        public async Task<SuccessResponse<GetProgrammeDTO>> GetProgrammeById(Guid programmeId)
        {
            var programme = await _repository.Programme.Get(x => x.Id == programmeId)
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Manager)
                .FirstOrDefaultAsync();

            if (programme == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeNotFound}");
            }

            var programmeDto = _mapper.Map<GetProgrammeDTO>(programme);
            var managers = programme.ProgrammeManagers.Select(x => x.Manager);
            programmeDto.Managers = _mapper.Map<List<ManagerDTO>>(managers);
            return new SuccessResponse<GetProgrammeDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = programmeDto
            };
        }

        public async Task<SuccessResponse<object>> DeleteProgramme(Guid programmeId)
        {
            var programme = await GetProgramme(programmeId);

            _repository.Programme.Remove(programme);

            UserActivity userActivity = AuditLog.UserActivity(programme, _webHelper.User().UserId, nameof(programme), $"Deleted a Program - \"{programme.Title}\"", programme.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeletionSuccessResponse
            };
        }

        public async Task<SuccessResponse<GetProgrammeDTO>> UpdateProgramme(Guid programmeId, UpdateProgrammeRequest request)
        {
            var programme = await GetProgramme(programmeId);
            var managers = await GetManagers(request.Managers);

            await CreateCategory(request.Category);
            _mapper.Map(request, programme);
            var programmeManagerIdsToRemove = await HandleProgrammeManagersUpdate(request.Managers, programmeId);

            UserActivity userActivity = AuditLog.UserActivity(programme, _webHelper.User().UserId, nameof(programme), $"Updated a Program - \"{programme.Title}\"", programme.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var programDto = _mapper.Map<GetProgrammeDTO>(programme);
            var managerDto = managers.Where(x => !programmeManagerIdsToRemove.Contains(x.Id)).ToList();
            programDto.Managers = managerDto;

            return new SuccessResponse<GetProgrammeDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = programDto
            };
        }

        public async Task<PagedResponse<IEnumerable<GetAllProgrammmeDTO>>> GetProgrammes(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var organizationId = _webHelper.User().OrganizationId;
            var programmeQuery = _repository.Programme.Get(x => x.OrganizationId == organizationId)
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Manager) as IQueryable<Programme>;
            
            if (!string.IsNullOrWhiteSpace(parameter.Search) && string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var search = parameter.Search.Trim();
                programmeQuery = programmeQuery.Where(x =>
                    x.Title.Contains(search) || x.Category.Contains(search) || x.Sponsor.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(parameter.Search) && !string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var search = parameter.Search.Trim();
                var searchBy = parameter.SearchBy.Trim();
                programmeQuery = ProgrammeQuerySearchBy(programmeQuery, searchBy, search);
            }

            programmeQuery = programmeQuery.OrderByDescending(x => x.CreatedAt);
            var programmeDto = programmeQuery.ProjectTo<GetAllProgrammmeDTO>(_mapper.ConfigurationProvider);
            var programmes = await PagedList<GetAllProgrammmeDTO>.Create(programmeDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetAllProgrammmeDTO>.CreateResourcePageUrl(parameter, name, programmes, urlHelper);

            var response = new PagedResponse<IEnumerable<GetAllProgrammmeDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = programmes,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        private static IQueryable<Programme> ProgrammeQuerySearchBy(IQueryable<Programme> query, string searchBy, string search)
        {
            search = search.Trim().ToLower();
            if (searchBy.Equals(EProgrammeSearchBy.Category.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return query.Where(x => x.Category.ToLower() == search);
            }

            if (searchBy.Equals(EProgrammeSearchBy.ProgramManager.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return query
                    .Where(x => x.ProgrammeManagers
                    .Any(x => x.Manager.FirstName.ToLower() == search || x.Manager.LastName.ToLower() == search));
            }
            
            if (searchBy.Equals(EProgrammeSearchBy.Sponsor.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return query.Where(x => x.Sponsor.ToLower() == search);
            }

            if (searchBy.Equals("status", StringComparison.OrdinalIgnoreCase))
            {
                return query.Where(x => x.Status.ToLower() == search);
            }

            return query;
        }

        public async Task<SuccessResponse<GetProgrammeStatsDTO>> GetProgrammeStat()
        {
            var organizationId = _webHelper.User().OrganizationId;
            var totalCount = await _repository.Programme.CountAsync(x => x.OrganizationId == organizationId);

            var ongoingStatus = EProgrammeStatus.Ongoing.ToString();
            var ongoingCount = await _repository.Programme.CountAsync(x => x.OrganizationId == organizationId && x.Status == ongoingStatus);

            var notStartedStatus = EProgrammeStatus.NotStarted.ToString();
            var notStartedCount = await _repository.Programme.CountAsync(x => x.OrganizationId == organizationId && x.Status == notStartedStatus);


            var completedStatus = EProgrammeStatus.Completed.ToString();
            var completedCount = await _repository.Programme.CountAsync(x => x.OrganizationId == organizationId && x.Status == completedStatus);

            var response = new SuccessResponse<GetProgrammeStatsDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetProgrammeStatsDTO
                {
                    TotalCount = totalCount,
                    CompletedCount = completedCount,
                    OngoingCount = ongoingCount,
                    NotStartedCount = notStartedCount
                }
            };

            return response;
        }

        public async Task<PagedResponse<IEnumerable<GetProgrammeSponsor>>> GetProgrammeSponsor(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var query = _repository.ProgrammeSponsor.QueryAll();

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                query = query.Where(x => x.Name.Contains(parameter.Search));
            }

            var sponsorQuery = query.ProjectTo<GetProgrammeSponsor>(_mapper.ConfigurationProvider);
            var sponsors = await PagedList<GetProgrammeSponsor>.Create(sponsorQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetProgrammeSponsor>.CreateResourcePageUrl(parameter, name, sponsors, urlHelper);

            var response = new PagedResponse<IEnumerable<GetProgrammeSponsor>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = sponsors,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<GetProgrammeSponsor>> CreateSponsor(CreateProgrammeSponsorRequest request)
        {
            var sponsorExist = await _repository.ProgrammeSponsor.ExistsAsync(x => x.Name == request.Name);

            if (sponsorExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateProgrammeSponsor}");
            }

            var programmeSponsor = new ProgrammeSponsor {Name = request.Name};            

            await _repository.ProgrammeSponsor.AddAsync(programmeSponsor);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetProgrammeSponsor>(programmeSponsor);

            return new SuccessResponse<GetProgrammeSponsor>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };

        }

        public async Task<PagedResponse<IEnumerable<ManagerDTO>>> GetProgrammeManagers(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var userQuery = _repository.User.QueryAll();
            var programmeManagerQuery = _repository.ProgrammeManager.QueryAll();
            var userRoleQuery = _repository.UserRole.QueryAll();

            var programmeManagers = from user in userQuery
                                    join manager in programmeManagerQuery on user.Id equals manager.ManagerId
                                    join role in userRoleQuery on user.Id equals role.UserId
                                    where user.Role == ERole.ProgramManager.ToString() && manager.ProgrammeId == programmeId
                                    select user;

            var managerDto = programmeManagers.ProjectTo<ManagerDTO>(_mapper.ConfigurationProvider);
            var pagedManagersDto = await PagedList<ManagerDTO>.Create(managerDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<ManagerDTO>.CreateResourcePageUrl(parameter, actionName, pagedManagersDto, urlHelper);

            var response = new PagedResponse<IEnumerable<ManagerDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedManagersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };

            return response;
        }

        // ToDo
        public async Task<PagedResponse<IEnumerable<ManagerDTO>>> GetProgrammeManagers(ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            //await ValidateProgrammeManager(programmeId);


            return await ProgrammeManagers(null, parameter, actionName, urlHelper);
        }

        private async Task ValidateProgrammeManager(Guid? programmeId)
        {
            var programmeExist = await _repository.Programme.ExistsAsync(x => x.Id == programmeId);

            if (!programmeExist)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeNotFound}");
            }
        }

        private async Task<PagedResponse<IEnumerable<ManagerDTO>>> ProgrammeManagers(Guid? programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var managerQuery = _repository.User.QueryAll().Where(x => x.Role == ERole.ProgramManager.ToString());

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                managerQuery = managerQuery.Where(x => x.FirstName.Contains(search) ||
                    x.LastName.Contains(search));
            }

            var managerDto = managerQuery.ProjectTo<ManagerDTO>(_mapper.ConfigurationProvider);
            var pagedManagersDto = await PagedList<ManagerDTO>.Create(managerDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<ManagerDTO>.CreateResourcePageUrl(parameter, actionName, pagedManagersDto, urlHelper);

            var response = new PagedResponse<IEnumerable<ManagerDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedManagersDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };

            return response;
        }

        public async Task<SuccessResponse<ProgrammePreviewDTO>> GetProgrammePreview(Guid programmeId)
        {
            var query = _repository.Programme.Get(x => x.Id == programmeId)
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Manager)
                .Include(x => x.CreatedBy)
                .Include(x => x.LearningTracks) as IQueryable<Programme>;

            var programme = await _mapper.ProjectTo<ProgrammePreviewDTO>(query).FirstOrDefaultAsync();

            if (programme == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeNotFound}");
            }

            var programmeDto = _mapper.Map<ProgrammePreviewDTO>(programme);
            return new SuccessResponse<ProgrammePreviewDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = programmeDto
            };
        }

        public async Task<SuccessResponse<ProgrammeTargetStatDto>> GetBeneficiaryFacilitatorStat(Guid programmeId)
        {
            var programmeTarget = new ProgrammeTargetStatDto();
            programmeTarget.Beneficiaries = await GetGenderTargetStatus(programmeId, ETarget.Beneficiary);
            programmeTarget.Facilitators = await GetGenderTargetStatus(programmeId, ETarget.Facilitator);

            return new SuccessResponse<ProgrammeTargetStatDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = programmeTarget
            };
        }

        public async Task<SuccessResponse<List<ProgLearningTrackStat>>> GetLearningTrackStat(Guid programmeId)
        {
            var stat = new ProgLearningTrackStat();
            var beneficiaryStat = await GetLearningTrackGroup(programmeId, ETarget.Beneficiary);
            var facilitatorStat = await GetLearningTrackGroup(programmeId, ETarget.Facilitator);
            var facilitatorStatDic = facilitatorStat.ToDictionary(x => x.Id);
            var progLearningTrackStat = new List<ProgLearningTrackStat>();

            foreach (var bs in beneficiaryStat)
            {
                var facilitatorGetValue = facilitatorStatDic.TryGetValue(bs.Id, out LearningTrackGroup facLearningTrackGroup);
                progLearningTrackStat.Add(new ProgLearningTrackStat
                {
                    Id = bs.Id,
                    Title = bs.Title,
                    BeneficiaryCount = bs.Count,
                    FacilitatorCount = facilitatorGetValue ? facLearningTrackGroup.Count : 0
                });
            }

            return new SuccessResponse<List<ProgLearningTrackStat>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = progLearningTrackStat
            };
        }

        private async Task<List<LearningTrackGroup>> GetLearningTrackGroup(Guid programmeId, ETarget target)
        {
            var callForApplication = _repository.CallForApplication.QueryAll();
            var applicantDetails = _repository.Applicant.QueryAll();
            var learningTrack = _repository.LearningTrack.QueryAll();

            var targetGroups = await (from a in applicantDetails
                join c in callForApplication on a.ActivityId equals c.Id
                join l in learningTrack on a.LearningTrackId.Value equals l.Id 
                where c.ProgrammeId == programmeId && c.Target == target.ToString()
                group a by new { l.Title, l.Id }
                into targetGroup
                select new LearningTrackGroup
                {
                    Id = targetGroup.Key.Id,
                    Title = targetGroup.Key.Title,
                    Count = targetGroup.Count()
                }).ToListAsync();

            return targetGroups;
        }

        private async Task<GenderTargetStatusDto> GetGenderTargetStatus(Guid programmeId, ETarget target)
        {
            var genderTargetStatusDto = new GenderTargetStatusDto();
            genderTargetStatusDto.Gender = await GetGenderStat(programmeId, target);
            genderTargetStatusDto.Target = await GetTargetStat(programmeId, target);

            return genderTargetStatusDto;
        }

        private async Task<TargetStatusDto> GetTargetStat(Guid programmeId, ETarget target)
        {
            var callForApplication = _repository.CallForApplication.QueryAll();
            var applicantDetails = _repository.Applicant.QueryAll();

            var total = (from a in applicantDetails
                join c in callForApplication on a.ActivityId equals c.Id
                where c.ProgrammeId == programmeId && c.Target == target.ToString()
                select a).Count();


            var targetGroups = await (from a in applicantDetails
                join c in callForApplication on a.ActivityId equals c.Id
                where c.ProgrammeId == programmeId && c.Target == target.ToString()
                group a by new {a.Status}
                into targetGroup
                select new TargetGroup
                {
                    Status = targetGroup.Key.Status,
                    Count = targetGroup.Count()
                }).ToListAsync();

            return new TargetStatusDto{ Total = total, Type = targetGroups};
        }

        private async Task<List<GenderTargetGroup>> GetGenderStat(Guid programmeId, ETarget target)
        {
            var callForApplication = _repository.CallForApplication.QueryAll();
            var applicantDetails = _repository.Applicant.QueryAll();

            var genderStat = await (from a in applicantDetails
                join c in callForApplication on a.ActivityId equals c.Id
                where c.ProgrammeId == programmeId && c.Target == target.ToString()
                group a by new { a.Gender }
                into targetGroup
                select new GenderTargetGroup
                {
                    Status = targetGroup.Key.Gender,
                    Count = targetGroup.Count()
                }).ToListAsync();

            var total = genderStat.Sum(x => x.Count);

            foreach (var gs in genderStat)
            {
                gs.Percentage = (double)gs.Count / total;
            }

            return genderStat;
        }


        private async Task<List<ManagerDTO>> GetManagers(List<Guid> managerIdsRequest)
        {
            if (managerIdsRequest.Any())
            {
                var users = _repository.User.Get(x => managerIdsRequest.Contains(x.Id));
                var userRoles = _repository.UserRole.QueryAll();
                var roles = _repository.Role.QueryAll();

                var userQuery = from user in users
                    join uRoles in userRoles on user.Id equals uRoles.UserId
                    join role in roles on uRoles.RoleId equals role.Id
                    where managerIdsRequest.Contains(user.Id) && role.Name == ERole.ProgramManager.ToString()
                    select user;
                var managers = await userQuery.ProjectTo<ManagerDTO>(_mapper.ConfigurationProvider).ToListAsync();

                if (managers.Count != managerIdsRequest.Count)
                {
                    var managerIds = managers.Select(x => x.Id).ToList();
                    var managerIdsNotFound = managerIdsRequest.Except(managerIds);
                    throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ManagerNotFound} {string.Join(",", managerIdsNotFound)}");
                }

                return managers;
            }

            return new List<ManagerDTO>();

        }

        private async Task<IEnumerable<Guid>> HandleProgrammeManagersUpdate(List<Guid> managersRequest, Guid programmeId)
        {
            var programmeManagers = await _repository.ProgrammeManager.Get(x => x.ProgrammeId == programmeId).ToListAsync();
            var programmeManagerIds = programmeManagers.Select(x => x.ManagerId);

            var programmeManagerIdsForCreation = managersRequest.Except(programmeManagerIds);
            var programManagersForCreation =  GetProgramManagersForCreation(programmeManagerIdsForCreation, programmeId);
            await _repository.ProgrammeManager.AddRangeAsync(programManagersForCreation);

            var programmeManagerIdsToRemove = programmeManagerIds.Except(managersRequest);
            var programmeManagersToDelete =
                programmeManagers.Where(x => programmeManagerIdsToRemove.Contains(x.ManagerId));

            _repository.ProgrammeManager.RemoveRange(programmeManagersToDelete);

            return programmeManagerIdsToRemove;

        }
        private async Task<Programme> GetProgramme(Guid programmeId)
        {
            var programme = await _repository.Programme.GetByIdAsync(programmeId);

            if (programme == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeNotFound}");
            }

            return programme;
        }
        private List<ProgrammeManager> GetProgramManagersForCreation(IEnumerable<Guid> managerIds, Guid programmeId)
        {
            var programmeManagers = new List<ProgrammeManager>();

            foreach (var managerId in managerIds)
            {
                programmeManagers.Add(new ProgrammeManager{ManagerId = managerId, ProgrammeId = programmeId});
            }

            return programmeManagers;
        }

        private async Task CreateCategory(string name)
        {
            var sponsorExist = await _repository.ProgrammeCategory.ExistsAsync(x => x.Name == name);

            if (!sponsorExist)
            {
                var sponsor = new ProgrammeCategory { Name = name };
                await _repository.ProgrammeCategory.AddAsync(sponsor);
            }
        }

        public async Task<PagedResponse<IEnumerable<ProgramApplicantDTO>>> GetProgrammeBeneficiaries(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            IQueryable<ProgramApplicantDTO> beneficiaries = GetProgrammeByTarget(programmeId, parameter, ETarget.Beneficiary);

            var pagedBeneficiaryDto = await PagedList<ProgramApplicantDTO>.Create(beneficiaries, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<ProgramApplicantDTO>.CreateResourcePageUrl(parameter, actionName, pagedBeneficiaryDto, urlHelper);

            var response = new PagedResponse<IEnumerable<ProgramApplicantDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedBeneficiaryDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };

            return response;
        }

        public async Task<PagedResponse<IEnumerable<ProgramApplicantDTO>>> GetProgrammeFacilitators(Guid programmeId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            IQueryable<ProgramApplicantDTO> beneficiaries = GetProgrammeByTarget(programmeId, parameter, ETarget.Facilitator);

            var pagedBeneficiaryDto = await PagedList<ProgramApplicantDTO>.Create(beneficiaries, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<ProgramApplicantDTO>.CreateResourcePageUrl(parameter, actionName, pagedBeneficiaryDto, urlHelper);

            var response = new PagedResponse<IEnumerable<ProgramApplicantDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedBeneficiaryDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };

            return response;
        }

        private IQueryable<ProgramApplicantDTO> GetProgrammeByTarget(Guid programmeId, ResourceParameter parameter, ETarget target)
        {
            var applicantsQuery = _repository.Applicant.QueryAll();
            var learningTrackQuery = _repository.LearningTrack.QueryAll();
            var callForApplicationQuery = _repository.CallForApplication.QueryAll();

            var beneficiaries = from applicant in applicantsQuery
                            join learningTrack in learningTrackQuery on applicant.LearningTrackId equals learningTrack.Id
                            join callForApplication in callForApplicationQuery on applicant.ActivityId equals callForApplication.Id
                            where applicant.ProgrammeId == programmeId && callForApplication.Target == target.ToString()
                            select new ProgramApplicantDTO
                            {
                                Id = applicant.Id,
                                FirstName = applicant.FirstName,
                                LastName = applicant.LastName,
                                Email = applicant.Email,
                                Gender = applicant.Gender,
                                Country = applicant.Country,
                                LearningTrack = learningTrack.Title,
                                Status = applicant.Status,
                                PhoneNumber = applicant.PhoneNumber,
                                Age = applicant.DateOfBirth != null ? DateTime.Today.Year - applicant.DateOfBirth.Value.Year : 0,
                            };

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                beneficiaries = beneficiaries.Where(x => x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Country.Contains(search) ||
                    x.LearningTrack.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var searchBy = parameter.SearchBy.Trim().ToLower();
                beneficiaries = beneficiaries.Where(x => searchBy.Contains(x.Status));
            }

            return beneficiaries;
        }
    }
}

using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DataTransferObjects;
using System.Net;
using Shared;
using System.Dynamic;
using Domain.Entities.Activities;

namespace Application.Services.V1
{
    public class PeopleService : IPeopleService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly UserManager<User> _userManager;
        private readonly IWebHelper _webHelper;

        public PeopleService(
            IMapper mapper,
            IRepositoryManager repository,
            UserManager<User> userManager, 
            IWebHelper webHelper)
        {
            _mapper = mapper;
            _repository = repository;
            _userManager = userManager;
            _webHelper = webHelper;
        }

        public async Task<PagedResponse<IEnumerable<BeneficiaryDTO>>> GetBeneficiaries(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            Guid organizationId = GetOrganizationId();

            var beneficiariesQuery = _repository.ApprovedApplicant.QueryAll(x => x.Role == ERole.Beneficiary.ToString() && x.User.OrganizationId == organizationId)
                                        .Include(z => z.ApprovedApplicantProgrammes)
                                        .ThenInclude(x => x.Programme) as IQueryable<ApprovedApplicant>;

            if (!string.IsNullOrEmpty(parameter.Search))
            {
                var search = parameter.Search.Trim().ToLower();
                beneficiariesQuery = beneficiariesQuery.Where(x =>
                      (x.User.FirstName.ToLower().Contains(search))
                     || (x.User.LastName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrEmpty(parameter.SearchBy))
            {
                beneficiariesQuery = beneficiariesQuery.Where(x => 
                    x.Country.ToLower() == parameter.SearchBy.ToLower()
                    || x.Gender.ToLower() == parameter.SearchBy.ToLower()
                    || x.ApprovedApplicantProgrammes.Any(x => x.Programme.Title.ToLower() == parameter.SearchBy.ToLower()));
            };

            beneficiariesQuery = beneficiariesQuery.OrderByDescending(x => x.CreatedAt);
           
            var beneficiariesResponses = beneficiariesQuery.ProjectTo<BeneficiaryDTO>(_mapper.ConfigurationProvider);
            var beneficiaries = await PagedList<BeneficiaryDTO>.Create(beneficiariesResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<BeneficiaryDTO>.CreateResourcePageUrl(parameter, name, beneficiaries, urlHelper);

            return new PagedResponse<IEnumerable<BeneficiaryDTO>>
            {
                Message = "Data successfully retrieved",
                Data = beneficiaries,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<SuccessResponse<BeneficiaryStatsDTO>> GetBeneficiariesStat()
        {
            Guid organizationId = GetOrganizationId();

            var beneficiaryQuery = _repository.ApprovedApplicant.QueryAll(x => x.Role == ERole.Beneficiary.ToString());
            var userQuery = _repository.User.QueryAll();

            var query = from applicant in beneficiaryQuery
                        join user in userQuery on applicant.UserId equals user.Id
                        where user.OrganizationId == organizationId
                        select applicant;

            var beneficiariesCount = await query.CountAsync();

            return new SuccessResponse<BeneficiaryStatsDTO>
            {
                Message = "Data successfully retrieved",
                Data = new BeneficiaryStatsDTO
                {
                    Total = beneficiariesCount,
                }
            };
        }

        public async Task<SuccessResponse<BeneficiaryDTO>> GetBeneficiaryById(Guid id)
        {
            Guid organizationId = GetOrganizationId();

            var beneficiaryQuery = _repository.ApprovedApplicant.QueryAll();
            var userQuery = _repository.User.QueryAll();

            var query = from applicant in beneficiaryQuery
                        join user in userQuery on applicant.UserId equals user.Id
                        where user.OrganizationId == organizationId && applicant.Id == id
                        select applicant;

            var beneficiary = await query.Include(u => u.User).FirstOrDefaultAsync();

            if (beneficiary == null)
                throw new RestException(HttpStatusCode.NotFound, "Beneficiary not found");

            return new SuccessResponse<BeneficiaryDTO>
            {
                Message = "Data retrieval successful",
                Data = _mapper.Map<BeneficiaryDTO>(beneficiary)
            };
        }

        public async Task<SuccessResponse<IEnumerable<BeneficiaryProgrammeDTO>>> GetBeneficiaryProgrammes(Guid id)
        {
            var BeneficiaryProgrammes = _repository.ApprovedApplicantProgramme.Get(x => x.ApprovedApplicantId == id)
                .Include(p => p.Programme)
                .Include(p => p.learningTrack)
                .Include(p => p.Programme).ThenInclude(s => s.ProgrammeManagers).ThenInclude(m => m.Manager)
                .Include(x => x.ApprovedApplicant).ThenInclude(u => u.User);

            if (BeneficiaryProgrammes == null)
                throw new RestException(HttpStatusCode.NotFound, "No programme found for this beneficiary");

            var response = BeneficiaryProgrammes.ProjectTo<BeneficiaryProgrammeDTO>(_mapper.ConfigurationProvider);

            return new SuccessResponse<IEnumerable<BeneficiaryProgrammeDTO>>()
            {
                Message = "Data successfully retrieved",
                Data = await response.ToListAsync(),
                Success = true
            };
        }

        public async Task<SuccessResponse<BeneficiaryProgrammeAndLearningTrackStatsDTO>> GetBeneficiaryProgrammeDetails(Guid beneficiaryId, Guid programmeId)
        {
            var BeneficiariesProgram = _repository.ApprovedApplicantProgramme.Get(x => x.ProgrammeId == programmeId)
                .Include(p => p.Programme)
                .Include(p => p.learningTrack)
                     .ThenInclude(f => f.LearningTrackFacilitators)
                .Include(p => p.Programme)
                     .ThenInclude(s => s.ProgrammeManagers)
                         .ThenInclude(m => m.Manager)
                .Include(x => x.ApprovedApplicant).ThenInclude(u => u.User)
                .OrderByDescending(x => x.Programme.CreatedAt);

            var BeneficiaryProgrammeDetails = BeneficiariesProgram.Where(x => x.ApprovedApplicantId == beneficiaryId)
                .Select(x => new BeneficiaryProgrammeAndLearningTrackStatsDTO
                {
                    Title = x.Programme.Title,
                    StartDate = x.Programme.StartDate,
                    EndDate = x.Programme.EndDate,
                    Sponsor = x.Programme.Sponsor,
                    Country = x.Programme.Country,
                    Status = x.Programme.Status,
                    LearningTrack = x.learningTrack.Title,
                    Description = x.Programme.Description,
                    Category = x.Programme.Category,
                    TimeLine = (DateTime.Now < x.Programme.EndDate ? (int)((DateTime.Now - x.Programme.StartDate).TotalSeconds * 100 / (x.Programme.EndDate - x.Programme.StartDate).TotalSeconds) : 100),
                    LearningTrackStats = new BeneficiaryLearningTrackStatsDTO()
                    {
                        TotalBeneficiaries = BeneficiariesProgram.Where(l => l.LearningTrackId == x.LearningTrackId).Count(),
                        TotalFacilitators = x.learningTrack.LearningTrackFacilitators.Count(),
                    },
                    ProgrammeManagers = x.Programme.ProgrammeManagers.Select(m => new ManagerDTO()
                    {
                        Id = m.ManagerId,
                        FirstName = m.Manager.FirstName,
                        LastName = m.Manager.LastName,
                        Picture = m.Manager.Picture,
                    }).ToList(),
                });
            
             if(BeneficiaryProgrammeDetails == null)
                 throw new RestException(HttpStatusCode.NotFound, "Programme not found for this beneficiary");

            return new SuccessResponse<BeneficiaryProgrammeAndLearningTrackStatsDTO>()
            {
                Message = "Data successfully retrieved",
                Data = await BeneficiaryProgrammeDetails.FirstOrDefaultAsync(),
                Success = true
            };
        }

        public async Task<SuccessResponse<GetPeopleStatsDto>> GetPeopleStats()
        {
            var beneficiaryQuery = UserQuery(ERole.Beneficiary);
            var facilitatorQuery = UserQuery(ERole.Facilitator);
            var programManagerQuery = UserQuery(ERole.ProgramManager);
            var sponsorQuery = UserQuery(ERole.Sponsor);
            var mentorQuery = UserQuery(ERole.Mentor);
            var alumniQuery = UserQuery(ERole.Alumni);
            var jobPartnerQuery = UserQuery(ERole.JobPartner);

            var beneficiaryTotalCount = await beneficiaryQuery.CountAsync();
            var facilitatorTotalCount = await facilitatorQuery.CountAsync();
            var programManagerTotalCount = await programManagerQuery.CountAsync();
            var mentorTotalCount = await mentorQuery.CountAsync();
            var sponsorTotalCount = await sponsorQuery.CountAsync();
            var alumniTotalCount = await alumniQuery.CountAsync();
            var jobPartnerTotalCount = await jobPartnerQuery.CountAsync();

            var response = new SuccessResponse<GetPeopleStatsDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetPeopleStatsDto
                {
                   BeneficiaryTotalCount = beneficiaryTotalCount,
                   FacilitatorTotalCount = facilitatorTotalCount,
                   ProgramManagerTotalCount = programManagerTotalCount,
                   SponsorTotalCount = sponsorTotalCount,
                   MentorTotalCount = mentorTotalCount,
                   AlumniTotalCount= alumniTotalCount,
                   JobPartnerTotalCount = jobPartnerTotalCount
                }
            };

            return response;
        }

        public async Task<SuccessResponse<BeneficiaryDTO>> UpdateBeneficiary(Guid id, UpdateBeneficiaryDTO model)
        {
            var beneficiary = await _repository.ApprovedApplicant.Get(x => x.Id == id).Include(u => u.User).FirstOrDefaultAsync();

            if (beneficiary == null)
                throw new RestException(HttpStatusCode.BadRequest, "Beneficiary not found");

            if (!string.IsNullOrEmpty(model.FirstName) || !string.IsNullOrEmpty(model.LastName))
            {
                User user = beneficiary.User;
                user.FirstName = !string.IsNullOrEmpty(model.FirstName) ? model.FirstName : user.FirstName;
                user.LastName = !string.IsNullOrEmpty(model.LastName) ? model.LastName : user.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new RestException(HttpStatusCode.BadRequest, result.Errors.FirstOrDefault().Description);
            }

            ApprovedApplicant beneficiaryToUpdate = _mapper.Map(model, beneficiary);

            _repository.ApprovedApplicant.Update(beneficiaryToUpdate);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<BeneficiaryDTO>
            {
                Success = true,
                Message = "Beneficiary’s data updated successfully",
                Data = _mapper.Map<BeneficiaryDTO>(beneficiaryToUpdate)
            };
        }
        public async Task<SuccessResponse<GetSingleProgrammeManagerDto>> GetProgrammeManagerAsync(Guid userId)
        {
            Guid organizationId = GetOrganizationId(); 

            var userExist = await _repository.User.ExistsAsync(x => x.Id == userId);
            if (!userExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var users = _repository.User.QueryAll();
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();

            var userQuery = from user in users
                            join uRole in userRoles on user.Id equals uRole.UserId
                            join role in roles on uRole.RoleId equals role.Id
                            where role.Name.ToLower() == ERole.ProgramManager.ToString().ToLower() && user.Id == userId && user.OrganizationId == organizationId
                            select user;

            var programmeManagers = userQuery.ProjectTo<GetSingleProgrammeManagerDto>(_mapper.ConfigurationProvider);
            var result = await programmeManagers.FirstOrDefaultAsync();

            if (result is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var response = new SuccessResponse<GetSingleProgrammeManagerDto>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = result
            };

            return response;
        }

        public async Task<PagedResponse<IEnumerable<GetProgrammeManagerDto>>> GetAllProgrammeManagersAsync(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            Guid organizationId = GetOrganizationId();

            var users = _repository.User.QueryAll();
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();

            var userQuery = from user in users
                            join uRole in userRoles on user.Id equals uRole.UserId
                            join role in roles on uRole.RoleId equals role.Id
                            where role.Name.ToLower() == ERole.ProgramManager.ToString().ToLower() && user.OrganizationId == organizationId
                            select user;

            if (!string.IsNullOrWhiteSpace(parameter.Search) && string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var search = parameter.Search.Trim().ToLower();
                var splitSearch = search.Split(' ');

                if (splitSearch.Length > 1)
                {
                    userQuery = userQuery.Where(x =>
                        (x.FirstName.Contains(splitSearch[0].Trim()) && x.LastName.Contains(splitSearch[1].Trim())) ||
                        (x.LastName.Contains(splitSearch[0].Trim()) && x.FirstName.Contains(splitSearch[0].Trim())) ||
                        x.Status == search);
                }
                else
                {
                    userQuery = userQuery.Where(x => x.FirstName.Contains(search) ||
                        x.LastName.Contains(search) ||
                        x.Status == search);
                }

                
            }

            if (!string.IsNullOrWhiteSpace(parameter.SearchBy) && !string.IsNullOrWhiteSpace(parameter.Search))
            {
                var searchBy = parameter.SearchBy.Trim().ToLower();
                var search = parameter.Search.Trim().ToLower();

                if (searchBy.Equals("status", StringComparison.OrdinalIgnoreCase))
                {
                    userQuery = userQuery.Where(x => x.Status.ToLower() == search);
                }

                if (searchBy.Equals("programme", StringComparison.OrdinalIgnoreCase))
                {
                    userQuery = userQuery.Where(x => x.ProgrammeManagers.Where(x => x.Programme.Title.Contains(search)).Any());
                }
            }

            userQuery = userQuery
                .Include(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Programme)
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.UpdatedAt);

            var programmeManagers = userQuery.ProjectTo<GetProgrammeManagerDto>(_mapper.ConfigurationProvider);
            var results = await PagedList<GetProgrammeManagerDto>.Create(programmeManagers, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetProgrammeManagerDto>.CreateResourcePageUrl(parameter, name, results, urlHelper);

            var response = new PagedResponse<IEnumerable<GetProgrammeManagerDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = results,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public async Task<SuccessResponse<GetProgrammeManagerStatDto>> GetProgrammeManagerStatAsync()
        {
            Guid organizationId = GetOrganizationId();

            var users = _repository.User.QueryAll();
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();

            var userQuery = from user in users
                            join uRole in userRoles on user.Id equals uRole.UserId
                            join role in roles on uRole.RoleId equals role.Id
                            where role.Name.ToLower() == ERole.ProgramManager.ToString().ToLower() && user.OrganizationId == organizationId
                            select user;

            int total = await userQuery.CountAsync();
            int active = await userQuery.CountAsync(x => x.Status == EUserStatus.Active.ToString());
            int pending = await userQuery.CountAsync(x => x.Status == EUserStatus.Pending.ToString());
            int disabled = await userQuery.CountAsync(userQuery => userQuery.Status == EUserStatus.Disabled.ToString());

            return new SuccessResponse<GetProgrammeManagerStatDto>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = new GetProgrammeManagerStatDto
                {
                    TotalCount = total,
                    ActiveCount = active,
                    PendingCount = pending,
                    DisabledCount = disabled
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<GetFacilitatorDto>>> GetAllFacilitatorsAsync(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            Guid organizationId = GetOrganizationId();

            var applicantQuery = _repository.ApprovedApplicant.Get(x => x.Role == ERole.Facilitator.ToString() && x.User.OrganizationId == organizationId);

            var facilitatorQuery = applicantQuery
                .Include(x => x.User)
                .Include(x => x.ApprovedApplicantProgrammes)
                .Select(x => new GetFacilitatorDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Gender = x.Gender,
                    Country = x.Country,
                    Status = x.User.Status,
                    CreatedAt = x.CreatedAt,
                    Programmes = x.ApprovedApplicantProgrammes.Select(p => new FacilitatorProgrammeDto()
                    {
                        Id = p.Programme.Id,
                        Title = p.Programme.Title
                    })
                });

            if (!string.IsNullOrWhiteSpace(parameter.Search) && string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var search = parameter.Search.Trim().ToLower();
                var splitSearch = search.Split(' ');

                if (splitSearch.Length > 1)
                {
                    facilitatorQuery = facilitatorQuery.Where(x =>
                        (x.FirstName.Contains(splitSearch[0].Trim()) && x.LastName.Contains(splitSearch[1].Trim())) ||
                        (x.LastName.Contains(splitSearch[0].Trim()) && x.FirstName.Contains(splitSearch[0].Trim())) ||
                        x.Gender == search ||
                        x.Country.Contains(search) ||
                        x.Status == search);
                }
                else
                {
                    facilitatorQuery = facilitatorQuery.Where(x =>
                        x.FirstName.Contains(search) ||
                        x.LastName.Contains(search) ||
                        x.Gender == search ||
                        x.Country.Contains(search) ||
                        x.Status == search);
                }
            }

            if (!string.IsNullOrWhiteSpace(parameter.SearchBy) && !string.IsNullOrWhiteSpace(parameter.Search))
            {
                var searchBy = parameter.SearchBy.Trim().ToLower();
                var search = parameter.Search.Trim().ToLower();

                facilitatorQuery = FacilitatorSearchByQuery(facilitatorQuery, searchBy, search);
            }

            facilitatorQuery = facilitatorQuery.OrderByDescending(x => x.CreatedAt);

            var facilitators = await PagedList<GetFacilitatorDto>.Create(facilitatorQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetFacilitatorDto>.CreateResourcePageUrl(parameter, name, facilitators, urlHelper);

            var response = new PagedResponse<IEnumerable<GetFacilitatorDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = facilitators,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        /// <summary>
        /// Gets the logged in User's Organization Id
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RestException"></exception>
        private Guid GetOrganizationId()
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);
            return organizationId;
        }

        private static IQueryable<GetFacilitatorDto> FacilitatorSearchByQuery(IQueryable<GetFacilitatorDto> facilitatorQuery, string searchBy, string search)
        {
            if (searchBy.Equals("Country", StringComparison.OrdinalIgnoreCase))
            {
                facilitatorQuery = facilitatorQuery.Where(x => x.Country.ToLower() == search);
            }
            if (searchBy.Equals("Gender", StringComparison.OrdinalIgnoreCase))
            {
                facilitatorQuery = facilitatorQuery.Where(x => x.Gender.ToLower() == search);
            }
            if (searchBy.Equals("Status", StringComparison.OrdinalIgnoreCase))
            {
                facilitatorQuery = facilitatorQuery.Where(x => x.Status.ToLower() == search);
            }
            if (searchBy.Equals("Programme", StringComparison.OrdinalIgnoreCase))
            {
                facilitatorQuery = facilitatorQuery.Where(x => x.Programmes.Where(x => x.Title.Contains(search)).Any());
            }

            return facilitatorQuery;
        }

        public async Task<SuccessResponse<ICollection<GetUserProgrammeDto>>> GetUserProgrammesAsync(Guid userId, string search)
        {
            var userExist = await _repository.User.ExistsAsync(x => x.Id == userId);
            if (!userExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userProgrammesQuery = _repository.ProgrammeManager.QueryAll()
                .Include(x => x.Programme)
                .ThenInclude(x => x.ProgrammeManagers)
                .ThenInclude(x => x.Manager)
                .Where(x => x.ManagerId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchBy = search.ToLower();
                userProgrammesQuery = userProgrammesQuery.Where(x => x.Programme.Status.ToLower() == searchBy);
            }

            var userProgrammes = await userProgrammesQuery.ToListAsync();
            var result = _mapper.Map<ICollection<GetUserProgrammeDto>>(userProgrammes);

            return new SuccessResponse<ICollection<GetUserProgrammeDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = result,
            };
        }

        private IQueryable<User> UserQuery(ERole roleTitle)
        {
            var userRoles = _repository.UserRole.QueryAll();
            var roles = _repository.Role.QueryAll();
            var users = _repository.User.QueryAll();

            var query = from user in users
                        join uRoles in userRoles on user.Id equals uRoles.UserId
                        join role in roles on uRoles.RoleId equals role.Id
                        where role.Name == roleTitle.ToString()
                        select user;

            return query;
        }

        public async Task<SuccessResponse<GetFacilitatorStatsDto>> GetFacilitatorsStat()
        {
            var userQuery = _repository.User.QueryAll();
            var applicantQuery = _repository.ApprovedApplicant.QueryAll().Where(x => x.Role == ERole.Facilitator.ToString());

            var applicants = from applicant in applicantQuery
                             join user in userQuery on applicant.UserId equals user.Id
                             select new { applicant.Id, user.Status };

            var totalCount = await applicants.CountAsync();
            var activeCount = await applicants.CountAsync(x => x.Status == EUserStatus.Active.ToString());
            var pendingCount = await applicants.CountAsync(x => x.Status == EUserStatus.Pending.ToString());
            var disabledCount = await applicants.CountAsync(x => x.Status == EUserStatus.Disabled.ToString());

            var response = new SuccessResponse<GetFacilitatorStatsDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetFacilitatorStatsDto
                {
                    Total = totalCount,
                    Active = activeCount,
                    Pending = pendingCount,
                    Disabled = disabledCount
                },
                Success = true
            };

            return response;
        }

        public async Task<SuccessResponse<BeneficiaryProgrammesStatsDTO>> GetBeneficiaryProgrammesStats(Guid BeneficiaryId)
        {
            var beneficiaryProgrammes = await _repository.ApprovedApplicantProgramme.Get(x => x.ApprovedApplicantId == BeneficiaryId)
                .Include(p => p.Programme).ToListAsync();

            var response = new BeneficiaryProgrammesStatsDTO()
            {
                Total = beneficiaryProgrammes.Count(),
                Ongoing = beneficiaryProgrammes.Count(x => x.Programme.Status == EProgrammeStatus.Ongoing.ToString()),
                Completed = beneficiaryProgrammes.Count(x => x.Programme.Status == EProgrammeStatus.Completed.ToString()),
            };

            return new SuccessResponse<BeneficiaryProgrammesStatsDTO>
            {
                Data = response,
                Message = "Data successfully retrieved",
                Success = true
            };
        }

        public async Task<SuccessResponse<ICollection<GetEventDTO>>> GetBeneficiaryProgrammeEvents(Guid programId)
        {
            var programmeEvents = await _repository.Event.Get(x => x.ProgrammeId == programId).ToListAsync();

            if(programmeEvents == null)
                throw new RestException(HttpStatusCode.NotFound, "No event found for this programme");

            var events = _mapper.Map<ICollection<GetEventDTO>>(programmeEvents);

            return new SuccessResponse<ICollection<GetEventDTO>>
            {
                Data = events,
                Message = "Data successfully retrieved",
                Success = true
            };
        }

        public async Task<SuccessResponse<ICollection<GetApprovedApplicantSurveysDto>>> GetApplicantSurvey(Guid approvedApplicantId)
        {
            var approvedApplicant = await _repository.ApprovedApplicant.Get(x => x.Id == approvedApplicantId).FirstOrDefaultAsync();
            if (approvedApplicant is null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var surveyQuery = _repository.Survey.QueryAll();

            var ActivityFormQuery = _repository.ActivityForm.QueryAll();
            var applicantProgramsQuery = _repository.ApprovedApplicantProgramme.QueryAll();

            var surveyList =await (from applicantProgram in applicantProgramsQuery
                        join survey in surveyQuery on applicantProgram.ProgrammeId equals survey.ProgrammeId 
                        join activityForm in ActivityFormQuery on survey.Id equals activityForm.ActivityId
                        where applicantProgram.ApprovedApplicantId == approvedApplicantId
                        select new GetApprovedApplicantSurveysDto()
                        {
                            Id = survey.Id,
                            ProgrammeId = survey.Programme.Id,
                            ActivityId = survey.ActivityId,
                            ProgramTitle = survey.Programme.Title,
                            Title = survey.Title,
                            StartDate = survey.StartDate,
                            EndDate = survey.EndDate,
                            Description = survey.Description,
                            Target = survey.Target,
                            Status = survey.EndDate.Date < DateTime.Now.Date ? "Closed" : "Open", 
                            SuccessMessageBody = survey.SuccessMessageBody,
                            SuccessMessageTitle = survey.SuccessMessageTitle,
                            ActivityForm = new GetActivityDTO
                            {
                                Id = activityForm.Id,
                                Description = activityForm.Description,
                                Name = activityForm.Name
                            },
                            LearningTracks = survey.SurveyLearningTracks.Select(x => new SurveyLearningTrackDTO
                            {
                                Title = x.LearningTrack.Title,
                                Id = x.LearningTrack.Id,
                            }).ToList()
                        }).OrderBy(z => z.EndDate).ToArrayAsync();

            return new SuccessResponse<ICollection<GetApprovedApplicantSurveysDto>>
            {
                Data = surveyList,
                Success = true,
                Message = "Data successfully retrieved"
            };
                      

        }
    }
}

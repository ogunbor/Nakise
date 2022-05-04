using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Domian.Entities.ActivityForms;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Implementations
{
    public class LearningTrackService: ILearningTrackService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;

        public LearningTrackService(
            IMapper mapper,
            IRepositoryManager repository, IWebHelper webHelper)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
        }


        public async Task<PagedResponse<IEnumerable<GetLearningTrackDTO>>> GetLearningTracks(Guid programmeId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            await ProgrammeExist(programmeId);

            var query = _repository.LearningTrack.Get(x => x.ProgrammeId == programmeId)
                .Include(x => x.LearningTrackFacilitators)
                .ThenInclude(x => x.Facilitator) as IQueryable<LearningTrack>;

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.ToLower().Trim();
                query = query.Where(x => x.Title.Contains(search));
            }

            var learningTrackQuery = query.ProjectTo<GetLearningTrackDTO>(_mapper.ConfigurationProvider);
            var learningTracks = await PagedList<GetLearningTrackDTO>.Create(learningTrackQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetLearningTrackDTO>.CreateResourcePageUrl(parameter, name, learningTracks, urlHelper);

            var response = new PagedResponse<IEnumerable<GetLearningTrackDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = learningTracks,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }
       
        public async Task<SuccessResponse<GetLearningTrackDTO>> GetLearningTrackById(Guid learningTrackId)
        {
            var learningTrack = await _repository.LearningTrack.Get(x => x.Id == learningTrackId)
                .Include(x => x.LearningTrackFacilitators)
                .ThenInclude(x => x.Facilitator)
                .FirstOrDefaultAsync();

            if (learningTrack == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.LearningTrackNotFound}");
            }

            var learningTrackDto = _mapper.Map<GetLearningTrackDTO>(learningTrack);
            return new SuccessResponse<GetLearningTrackDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = learningTrackDto
            };
        }

        public async Task<SuccessResponse<GetLearningTrackDTO>> CreateLearningTrack(Guid programmeId, CreateLearningTrackRequest request)
        {
            var facilitators = await GetFacilitators(request.Facilitators);

            await CheckDuplicateTitle(request.Title, programmeId);

            var learningTrack = _mapper.Map<LearningTrack>(request);
            learningTrack.ProgrammeId = programmeId;
            learningTrack.CreatedById = _webHelper.User().UserId;

            if (facilitators.Any())
            {
                var facilitatorsIds = facilitators.Select(x => x.Id);
                learningTrack.LearningTrackFacilitators = GetFacilitatorsForCreation(facilitatorsIds, learningTrack.Id);
            }

            await _repository.LearningTrack.AddAsync(learningTrack);

            UserActivity userActivity = AuditLog.UserActivity(learningTrack, _webHelper.User().UserId, nameof(learningTrack), $"Created a LearningTrack - {learningTrack.Title}", learningTrack.Id);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            await UpdateFormFieldOptions(programmeId);            

            var getLearningTrackDTO = _mapper.Map<GetLearningTrackDTO>(learningTrack);
            getLearningTrackDTO.Facilitators = facilitators;

            return new SuccessResponse<GetLearningTrackDTO>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = getLearningTrackDTO
            };
        }

        private async Task RemoveFormFieldOptions(Guid programmeId)
        {
            var callForApplications = await _repository.CallForApplication.QueryAll(x => x.ProgrammeId == programmeId).Select(x => x.Id).ToListAsync();
            var activityForms = await _repository.ActivityForm.QueryAll(x => callForApplications.Contains(x.ActivityId)).Select(x => x.Id).ToListAsync();
            var formFields = await _repository.FormField.QueryAll(x => activityForms.Contains(x.ActivityFormId) && x.Key == "learningTrack").Include(x => x.Options).ToListAsync();

            foreach (var formField in formFields)
            {
                _repository.FieldOption.RemoveRange(formField.Options);
                await _repository.SaveChangesAsync();
            }
        }

        private async Task UpdateFormFieldOptions(Guid programmeId)
        {
            var learningTracks = await _repository.LearningTrack.Get(x => x.ProgrammeId == programmeId).ToListAsync();
            var callForApplications = await _repository.CallForApplication.QueryAll(x => x.ProgrammeId == programmeId).Select(x => x.Id).ToListAsync();
            var activityForms = await _repository.ActivityForm.QueryAll(x => callForApplications.Contains(x.ActivityId)).Select(x => x.Id).ToListAsync();
            var formFields = await _repository.FormField.QueryAll(x => activityForms.Contains(x.ActivityFormId) && x.Key == "learningTrack").Include(x => x.Options).ToListAsync();

            foreach (var field in formFields)
            {
                _repository.FieldOption.RemoveRange(field.Options);
                await _repository.SaveChangesAsync();

                var fieldList = new List<FieldOption>();
                foreach (var ltrack in learningTracks)
                {
                    fieldList.Add(new FieldOption
                    {
                        Id = Guid.NewGuid(),
                        FormFieldId = field.Id,
                        Key = ltrack.Title,
                        Value = ltrack.Id.ToString(),
                    });
                }

                await _repository.FieldOption.AddRangeAsync(fieldList);
            }

            await _repository.SaveChangesAsync();
        }

        public async Task<SuccessResponse<GetLearningTrackDTO>> UpdateLearningTrack(Guid id,UpdateLearningTrackRequest request)
        {
            var learningTrack = await GetLearningTrack(id);

            var facilitators = await GetFacilitators(request.Facilitators);

            if (learningTrack.Title.ToLower() != request.Title.ToLower())
            {
                await CheckDuplicateTitle(request.Title, learningTrack.ProgrammeId);
            }
            learningTrack = _mapper.Map(request, learningTrack);
            var facilitatorIdToRemove = await HandleFacilitatorsUpdate(request.Facilitators, id);
            learningTrack.UpdatedAt = DateTime.UtcNow;

            UserActivity userActivity = AuditLog.UserActivity(learningTrack, _webHelper.User().UserId, nameof(learningTrack), $"Updated a LearningTrack - {learningTrack.Title}", learningTrack.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            await UpdateFormFieldOptions(learningTrack.ProgrammeId);

            var getlearningtrackDto = _mapper.Map<GetLearningTrackDTO>(learningTrack);
            var facilitatorDto = facilitators.Where(x => !facilitatorIdToRemove.Contains(x.Id)).ToList();
            getlearningtrackDto.Facilitators = facilitatorDto;

            return new SuccessResponse<GetLearningTrackDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = getlearningtrackDto
            };
        }

        public async Task<SuccessResponse<object>> DeleteLearningTrack(Guid learningTrackId)
        {
            var learningTrack = await _repository.LearningTrack.GetByIdAsync(learningTrackId);
            if (learningTrack == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.LearningTrackNotFound);

            _repository.LearningTrack.Remove(learningTrack);

            UserActivity userActivity = AuditLog.UserActivity(learningTrack, _webHelper.User().UserId, nameof(learningTrack), $"Deleted a LearningTrack - {learningTrack.Title}", learningTrack.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            var programmeId = learningTrack.ProgrammeId;

            await _repository.SaveChangesAsync();

            await RemoveFormFieldOptions(programmeId);

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeleteSuccessResponse,
            };

        }
        public async Task<PagedResponse<IEnumerable<GetLearningTrackApplicantsDTO>>> GetLearningTrackApplicants(Guid learningTrackId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var learningTracks = _repository.LearningTrack.QueryAll();
            var applicants =  _repository.Applicant.QueryAll();
            var query = from applicant in applicants
                        join learningTrack in learningTracks on applicant.LearningTrackId equals learningTrack.Id
                        where learningTrack.Id == learningTrackId
                        select applicant;
           
            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                query = query.Where(x => x.FirstName.Contains(search) ||
                    x.LastName.Contains(search));
            }
           
            var applicantsDto = query.ProjectTo<GetLearningTrackApplicantsDTO>(_mapper.ConfigurationProvider);
            var pagedApplicantsDto = await PagedList<GetLearningTrackApplicantsDTO>.Create(applicantsDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetLearningTrackApplicantsDTO>.CreateResourcePageUrl(parameter, actionName, pagedApplicantsDto, urlHelper);

            var response = new PagedResponse<IEnumerable<GetLearningTrackApplicantsDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedApplicantsDto,
                Meta = new Meta
                {
                    Pagination = page
                },
            };

            return response;

        }
        public async Task<SuccessResponse<LearningTrackStatDTO>> GetLearningTrackStat(Guid learningTrackId)
        {
            var learningTrackStat = await LearningTrackStats(learningTrackId);

            return new SuccessResponse<LearningTrackStatDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = learningTrackStat
            };
        }
        public async Task<SuccessResponse<List<OtherLearningTrackDTO>>> OtherLearningTracks()
        {
            var learningTrack =  _repository.LearningTrack.QueryAll();

            return await GetOtherLearningTracks(learningTrack);
        }

        public async Task<SuccessResponse<List<OtherLearningTrackDTO>>> OtherLearningTracksByProgramme(Guid programmeId)
        {
            var learningTrack = _repository.LearningTrack.QueryAll(x => x.ProgrammeId == programmeId);
            
            return await GetOtherLearningTracks(learningTrack);
        }

        private async Task<SuccessResponse<List<OtherLearningTrackDTO>>> GetOtherLearningTracks(IQueryable<LearningTrack> learningTrack)
        {
            var applicants = _repository.Applicant.QueryAll();
            List<OtherLearningTrackDTO> otherLearningTrackDTOs = new();

            foreach (var item in learningTrack)
            {
                var query = from a in applicants
                            join l in learningTrack on a.LearningTrackId equals l.Id
                            where l.Id == item.Id
                            select a;

                otherLearningTrackDTOs.Add(new OtherLearningTrackDTO
                {
                    Id = item.Id,
                    Title = item.Title,
                    ApplicantCount = await query.CountAsync()
                });
            }


            return new SuccessResponse<List<OtherLearningTrackDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = otherLearningTrackDTOs
            };
        }

        private async Task<List<FacilitatorDTO>> GetFacilitators(List<Guid> facilitatorIdsRequest)
        {
            if (facilitatorIdsRequest.Any())
            {
                var users = _repository.User.Get(x => facilitatorIdsRequest.Contains(x.Id));
                var userRoles = _repository.UserRole.QueryAll();
                var roles = _repository.Role.QueryAll();

                var userQuery = from user in users
                    join uRoles in userRoles on user.Id equals uRoles.UserId
                    join role in roles on uRoles.RoleId equals role.Id
                    where facilitatorIdsRequest.Contains(user.Id) && role.Name == ERole.Facilitator.ToString()
                    select user;

                var facilitators = await userQuery.ProjectTo<FacilitatorDTO>(_mapper.ConfigurationProvider).ToListAsync();

                if (facilitators.Count != facilitatorIdsRequest.Count)
                {
                    var facilitatorIds = facilitators.Select(x => x.Id).ToList();
                    var facilitatorIdsNotFound = facilitatorIdsRequest.Except(facilitatorIds);
                    throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.FacilitatorNotFound} {string.Join(",", facilitatorIdsNotFound)}");
                }

                return facilitators;
            }

            return new List<FacilitatorDTO>();

        }
        private async Task<LearningTrack> GetLearningTrack(Guid learningTrackId)
        {
            var learningTrack = await _repository.LearningTrack.GetByIdAsync(learningTrackId);

            if (learningTrack == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.LearningTrackNotFound}");
            }

            return learningTrack;
        }
        private List<LearningTrackFacilitator> GetFacilitatorsForCreation(IEnumerable<Guid> facilitatorIds, Guid learningTrackId)
        {
            var learningTrackFacilitators = new List<LearningTrackFacilitator>();

            foreach (var facilitatorId in facilitatorIds)
            {
                learningTrackFacilitators.Add(new LearningTrackFacilitator { FacilitatorId = facilitatorId, LearningTrackId = learningTrackId });
            }

            return learningTrackFacilitators;
        }
        private async Task<IEnumerable<Guid>> HandleFacilitatorsUpdate(List<Guid> managersRequest, Guid learningTrackId)
        {
            var learningTrackFacilitators = await _repository.LearningTrackFacilitator.Get(x => x.LearningTrackId == learningTrackId).ToListAsync();
            var learningTrackFacilitatorIds = learningTrackFacilitators.Select(x => x.FacilitatorId);

            var programmeManagerIdsForCreation = managersRequest.Except(learningTrackFacilitatorIds);
            var programManagersForCreation = GetFacilitatorsForCreation(programmeManagerIdsForCreation, learningTrackId);
            await _repository.LearningTrackFacilitator.AddRangeAsync(programManagersForCreation);

            var facilitatorIdsToRemove = learningTrackFacilitatorIds.Except(managersRequest);
            var facilitatorsToDelete =
                learningTrackFacilitators.Where(x => facilitatorIdsToRemove.Contains(x.FacilitatorId));

            _repository.LearningTrackFacilitator.RemoveRange(facilitatorsToDelete);

            return facilitatorIdsToRemove;

        }

        private async Task ProgrammeExist(Guid programmeId)
        {
            var programmeExist = await _repository.Programme.ExistsAsync(x => x.Id == programmeId);

            if (!programmeExist)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeNotFound}");
            }
        }

        private async Task CheckDuplicateTitle(string title, Guid programmeId)
        {
            var learningTrackExist = await _repository.LearningTrack.ExistsAsync(x => x.Title == title && x.ProgrammeId == programmeId);

            if (learningTrackExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateLearningTrack}");
            }
        }

        public async Task<PagedResponse<IEnumerable<GetAllFacilitatorsDto>>> GetFacilitators(ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            PagedResponse<IEnumerable<GetAllFacilitatorsDto>> response = await GetFacilitators(null, parameter, actionName, urlHelper);

            return response;
        }

        public async Task<PagedResponse<IEnumerable<GetAllFacilitatorsDto>>> GetFacilitatorsByLearningTrack(Guid learningTrackId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            PagedResponse<IEnumerable<GetAllFacilitatorsDto>> response = await GetFacilitators(learningTrackId, parameter, actionName, urlHelper);

            return response;
        }

        private async Task<PagedResponse<IEnumerable<GetAllFacilitatorsDto>>> GetFacilitators(Guid? learningTrackId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            IQueryable<LearningTrackFacilitator> facilitatorsQuery;
            if (learningTrackId is not null)
            {
                facilitatorsQuery = _repository.LearningTrackFacilitator.QueryAll()
                            .Include(x => x.Facilitator)
                            .Include(x => x.LearningTrack)
                            .Where(x => x.LearningTrackId == learningTrackId);
            }
            else
            {
                facilitatorsQuery = _repository.LearningTrackFacilitator.QueryAll()
                            .Include(x => x.Facilitator)
                            .Include(x => x.LearningTrack);
            }            

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                facilitatorsQuery = facilitatorsQuery.Where(x => x.Facilitator.FirstName.Contains(search) ||
                                                                 x.Facilitator.LastName.Contains(search));
            }

            var facilitatorsDto = facilitatorsQuery.ProjectTo<GetAllFacilitatorsDto>(_mapper.ConfigurationProvider);
            var pagedFacilitatorsDto = await PagedList<GetAllFacilitatorsDto>.Create(facilitatorsDto, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetAllFacilitatorsDto>.CreateResourcePageUrl(parameter, actionName, pagedFacilitatorsDto, urlHelper);

            var response = new PagedResponse<IEnumerable<GetAllFacilitatorsDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = pagedFacilitatorsDto,
                Meta = new Meta
                {
                    Pagination = page
                }
            };


            return response;
        }

        private async Task<LearningTrackStatDTO> LearningTrackStats(Guid learningTrackId)
        {
            var learningTracks = _repository.LearningTrack.QueryAll();
            var applicants = _repository.Applicant.QueryAll();
            var learningTrack = await GetLearningTrack(learningTrackId);

            var learningTrackGenderStats = from a in applicants
                                  join l in learningTracks on a.LearningTrackId equals l.Id
                                  where l.Id == learningTrackId
                                  select a;

            var male = await LearningTrackGenderStats(learningTrackGenderStats, EGender.Male);
            var female = await LearningTrackGenderStats(learningTrackGenderStats, EGender.Female);
            var others = await LearningTrackGenderStats(learningTrackGenderStats, EGender.Others);
            var notSpecified = await LearningTrackGenderStats(learningTrackGenderStats, EGender.NotSpecified);

            var learningTrackStat = new LearningTrackStatDTO
            {
                Description = learningTrack.Description,
                Title = learningTrack.Title,
                TotalCount = await learningTrackGenderStats.CountAsync(),
                Male = male,
                Female= female,
                Others = others,
                NotSpecified = notSpecified
            };

            return learningTrackStat;
        }
        private static async Task<LearningTrackGenderStat> LearningTrackGenderStats(IQueryable<ApplicantDetail> learningTrackGenderStats, EGender gender)
        {  
            var genderQuery = learningTrackGenderStats.Where(x => x.Gender == gender.ToString());
            var genderCount = await genderQuery.CountAsync();
            var total = await learningTrackGenderStats.CountAsync();

            var genderPercentage = total != default ? Math.Round((double)(100 * genderCount) / total) : default;
            var genderStat = new LearningTrackGenderStat
            {
                Count = genderCount,
                Percentage = genderPercentage
            };

            return genderStat;
        }
        private async Task<LearningTrackGenderStat> LearningTrackNotSpecifiedStats(IQueryable<ApplicantDetail> learningTrackGenderStats)
        {
            var notSpecifiedGenderStats = learningTrackGenderStats.Where(x => x.Gender == EGender.NotSpecified.ToString());
            var notSpecifiedCount = await notSpecifiedGenderStats.CountAsync();
            var total = await learningTrackGenderStats.CountAsync();

            var genderPercentage = Math.Round((double)(100 * notSpecifiedCount) / total);
            var genderStat = new LearningTrackGenderStat
            {
                Count = notSpecifiedCount,
                Percentage = genderPercentage
            };

            return genderStat;
        }
    }
}

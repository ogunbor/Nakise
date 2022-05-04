using Application.Contracts.V1.Activities;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Domian.Entities.ActivityForms;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Activities
{
    public class CallForApplicationService : ICallForApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;

        public CallForApplicationService(
            IMapper mapper,
            IRepositoryManager repository, 
            IWebHelper webHelper)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
        }

        private async Task<IEnumerable<Stage>> AddStages(Guid id, ICollection<CreateStageDto> stages)
        {
            var stagesList = new List<Stage>();
            foreach (var stage in stages)
            {
                var stageEntity = _mapper.Map<Stage>(stage);
                stageEntity.CallForApplicationId = id;
                stagesList.Add(stageEntity);
            }
            await _repository.Stage.AddRangeAsync(stagesList);

            return stagesList;
        }

        private Guid ValidateUser()
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            return organizationId;
        }

        private async Task<Activity> ValidateActivity()
        {
            var activity = await _repository.Activity.FirstOrDefaultNoTracking(x => x.Type == EActivityType.CallForApplication.ToString());
            if (activity == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

            return activity;
        }

        public async Task<SuccessResponse<GetCallForApplicationDto>> CreateCallForApplication(CreateCallForApplicationInputDto model)
        {
            var organizationId = ValidateUser();
            var activity = await ValidateActivity();
            await ValidateTitle(model.Title);
            Programme programme = await ValidateProgramme(model.ProgrammeId, organizationId);

            var callForApplication = _mapper.Map<CallForApplication>(model);
            callForApplication.ActivityId = activity.Id;
            callForApplication.Status = ECallForApplicationStatus.Activate.ToString();
            await _repository.CallForApplication.AddAsync(callForApplication);

            var form = new ActivityForm
            {
                Id = Guid.NewGuid(),
                ActivityId = callForApplication.Id,
                Name = EActivityType.CallForApplication.GetDescription(),
                Type = EActivityType.CallForApplication.ToString(),
                Description = callForApplication.Description
            };

            await _repository.ActivityForm.AddAsync(form);

            IEnumerable<Stage> stages = new List<Stage>();
            if (model.IsStage)
                stages = await AddStages(callForApplication.Id, model.Stages);

            UserActivity userActivity = AuditLog.UserActivity(callForApplication, _webHelper.User().UserId, "CallForApplication", $"Created CFA under - {programme.Title}", callForApplication.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetCallForApplicationDto>(callForApplication);
            response.Programme = _mapper.Map<GetProgrammeDto>(programme);
            response.Stages = _mapper.Map<ICollection<GetStageDto>>(stages);
            response.ActivityForm = _mapper.Map<GetActivityFormDto>(form);

            return new SuccessResponse<GetCallForApplicationDto>
            {
                Message = ResponseMessages.CallForApplicationCreatedSuccessfully,
                Data = response
            };
        }

        private async Task ValidateTitle(string title)
        {
            var titleExist = await _repository.CallForApplication.ExistsAsync(x => x.Title.ToLower() == title.ToLower());
            if (titleExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DuplicateTitle);
        }

        private async Task ValidateTitleForUpdate(string title, Guid id)
        {
            var titleExist = await _repository.CallForApplication.ExistsAsync(x => x.Title.ToLower() == title.ToLower() && x.Id != id);
            if (titleExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DuplicateTitle);
        }

        private async Task<Programme> ValidateProgramme(Guid programmeId, Guid organizationId)
        {
            var programme = await _repository.Programme.FirstOrDefault(x => x.Id == programmeId && x.OrganizationId == organizationId);
            if (programme == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ProgrammeNotFound);
            return programme;
        }

        public async Task<SuccessResponse<GetCallForApplicationDto>> UpdateCallForApplication(Guid callForApplicationId, UpdateCallForApplicationInputDto model)
        {
            var organizationId = ValidateUser();

            var callForApplication = await _repository.CallForApplication
                .Get(x => x.Id == callForApplicationId)
                .Include(x => x.Programme)
                .Include(x => x.Stages)
                .FirstOrDefaultAsync();
            if (callForApplication == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);
            
            await CheckIfCfaIsClosed(callForApplicationId);
            
            if (model.Title != callForApplication.Title)
                await ValidateTitleForUpdate(model.Title, callForApplication.Id);

            if (callForApplication.Programme.OrganizationId != organizationId)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            _repository.Stage.RemoveRange(callForApplication.Stages);

            UserActivity userActivity = AuditLog.UserActivity(callForApplication, _webHelper.User().UserId, nameof(callForApplication), $"Updated CFA under - {callForApplication.Programme.Title}", callForApplication.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            _mapper.Map(model, callForApplication);
            var stages = await AddStages(callForApplication.Id, model.Stages);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetCallForApplicationDto>(callForApplication);
            response.Programme = _mapper.Map<GetProgrammeDto>(callForApplication.Programme);
            response.Stages = _mapper.Map<ICollection<GetStageDto>>(stages);

            return new SuccessResponse<GetCallForApplicationDto>
            {
                Message = ResponseMessages.CallForApplicationUpdatedSuccessfully,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetCallForApplicationDto>> GetCallForApplication(Guid id)
        {
            CallForApplication callForApplication = await GetCallForApplicationById(id);

            var form = await _repository.ActivityForm.FirstOrDefault(x => x.ActivityId == id && x.Type == EActivityType.CallForApplication.ToString());

            var response = _mapper.Map<GetCallForApplicationDto>(callForApplication);
            response.OtherStatus =
                GetCallForApplicationOtherStatus(callForApplication.StartDate, callForApplication.EndDate);
            response.Programme = _mapper.Map<GetProgrammeDto>(callForApplication.Programme);
            response.Stages = _mapper.Map<ICollection<GetStageDto>>(callForApplication.Stages);
            response.ActivityForm = _mapper.Map<GetActivityFormDto>(form);

            return new SuccessResponse<GetCallForApplicationDto>
            {
                Message = ResponseMessages.CallForApplicationUpdatedSuccessfully,
                Data = response
            };
        }

        private async Task CheckIfCfaIsClosed(Guid id)
        {
            var cfa = await _repository.CallForApplication.GetByIdAsync(id);
            if (cfa is not null)
            {
                if (cfa.EndDate.Date < DateTime.Now.Date)
                {
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationClosed);
                }
            }
        }

        private void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }

        public async Task DeleteCallForApplication(Guid id)
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var callForApplication = await _repository.CallForApplication
               .Get(x => x.Id == id)
               .Include(x => x.Stages)
               .FirstOrDefaultAsync();

            CheckActivityDate(callForApplication.EndDate);

            var programme = await _repository.Programme.FirstOrDefaultNoTracking(x => x.Id == callForApplication.ProgrammeId);

            if (callForApplication == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);

            if (programme.OrganizationId != organizationId)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            UserActivity userActivity = AuditLog.UserActivity(callForApplication, _webHelper.User().UserId, nameof(callForApplication), $"Deleted CFA under - {programme.Title}", callForApplication.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            _repository.CallForApplication.Remove(callForApplication);
            await _repository.SaveChangesAsync();
        }

        public async Task<PagedResponse<IEnumerable<CallForApplicationParticipantDTO>>> GetCallForApplicationParticipants(Guid callForApplicationId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var cfa = await ValidateCallForApplicationIncludeStages(callForApplicationId);
            int stageCount = cfa.Stages.Count;

            var applicants = _repository.Applicant
                .QueryAll()
                .Include(x => x.LearningTrack)
                .Where(x => x.ActivityType == EActivityType.CallForApplication.ToString() && x.ActivityId == callForApplicationId)
                .Select(x => new CallForApplicationParticipantDTO
                {
                    Id = x.Id,
                    FormId = x.FormId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    LearningTrack = x.LearningTrack.Title,
                    Status = x.Status,
                    StageIndex = stageCount,
                    CurrentStageCount = GetCurrentStageIndex(cfa.Stages.ToList(), x.Status, x.StageId)
                });

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                applicants = applicants.Where(x => x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.LearningTrack.Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(parameter.SearchBy))
            {
                var searchBy = parameter.SearchBy.Trim().ToLower();
                applicants = applicants.Where(x => x.Status.ToLower() == searchBy);
            }

            var pagedBeneficiaryDto = await PagedList<CallForApplicationParticipantDTO>.Create(applicants, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<CallForApplicationParticipantDTO>.CreateResourcePageUrl(parameter, actionName, pagedBeneficiaryDto, urlHelper);

            var response = new PagedResponse<IEnumerable<CallForApplicationParticipantDTO>>
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
        private async Task<CallForApplication> GetCallForApplicationById(Guid id)
        {
            var callForApplication = await _repository.CallForApplication
                .Get(x => x.Id == id)
                .Include(x => x.Programme)
                .Include(x => x.Stages)
                .FirstOrDefaultAsync();

            if (callForApplication is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);

            return callForApplication;
        }
        private static int GetCurrentStageIndex(List<Stage> stages, string status, Guid? applicantStageId)
        {
            int currentStage = default;
            var ids = stages.OrderBy(x => x.Index).Select(x => x.Id).ToArray();

            if (status.Equals(EApplicantStatus.Pending.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                currentStage = 0;
            }
            if (status.Equals(EApplicantStatus.InReview.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (!applicantStageId.HasValue)
                    currentStage = default;
                else
                    currentStage = Array.IndexOf(ids, applicantStageId.Value) + 1;
            }
            if (status.Equals(EApplicantStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase) ||
                status.Equals(EApplicantStatus.Rejected.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                currentStage = ids.Length;
            }

            return currentStage;
        }

        public async Task<SuccessResponse<GetCallForApplicationStatusDto>> ActivateCallForApplication(Guid id)
        {
            Guid organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var response = await ChangeCallForApplicationStatus(id, ECallForApplicationStatus.Activate);

            return new SuccessResponse<GetCallForApplicationStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetCallForApplicationStatusDto>> SuspendCallForApplication(Guid id)
        {
            Guid organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var response = await ChangeCallForApplicationStatus(id, ECallForApplicationStatus.Deactivate);

            return new SuccessResponse<GetCallForApplicationStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetCallForApplicationStatusDto>> ExtendCallForApplication(Guid id, ExtendCallForApplication extendCfa)
        {
            await CheckIfCfaIsClosed(id);
            var cfa = await GetCallForApplicationById(id);

            if (cfa.EndDate.Date < DateTime.Now.Date)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.CallForApplicationClosed);

            if (extendCfa.Date < cfa.EndDate)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.CfaDateShouldBeGreaterThanDate);

            cfa.EndDate = extendCfa.Date;
            //cfa.IsClosed = false;

            UserActivity userActivity = AuditLog.UserActivity(cfa, _webHelper.User().UserId, nameof(cfa), $"Extended CFA - {cfa.Title}", cfa.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            _repository.CallForApplication.Update(cfa);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetCallForApplicationStatusDto>
            {
                Message = ResponseMessages.CfaExtendedSuccessfully,
                Data = _mapper.Map<GetCallForApplicationStatusDto>(cfa)
            };
        }

        private async Task<GetCallForApplicationStatusDto> ChangeCallForApplicationStatus(Guid id, ECallForApplicationStatus target)
        {

            var callForApplication = await GetCallForApplicationById(id);

            if (callForApplication is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);

            callForApplication.Status = target.ToString();
            callForApplication.UpdatedAt = DateTime.UtcNow;

            UserActivity userActivity = AuditLog.UserActivity(callForApplication, _webHelper.User().UserId, nameof(callForApplication), $"Change a CFA's status under - {callForApplication.Programme?.Title}", callForApplication.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            _repository.CallForApplication.Update(callForApplication);
            await _repository.SaveChangesAsync();

            return _mapper.Map<GetCallForApplicationStatusDto>(callForApplication);
        }
        public async Task<SuccessResponse<List<CfaGetStageDto>>> GetStages(Guid callForApplicationId)
        {
            await ValidateCallForApplication(callForApplicationId);

            var stages = await _repository.Stage.Get(x => x.CallForApplicationId == callForApplicationId)
                .Select(x => new CfaGetStageDto
                {
                    Id = x.Id,
                    Index = x.Index,
                    Name = x.Name
                }).ToListAsync();

            return new SuccessResponse<List<CfaGetStageDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = stages
            };
        }

        public async Task<SuccessResponse<List<CfaGetStageStatDto>>> GetStagesStat(Guid callForApplicationId)
        {
            var stage = _repository.Stage.QueryAll();
            var applicantDetail = _repository.Applicant.QueryAll();

            var stagesStat = await (from a in applicantDetail
                join s in stage on a.StageId equals s.Id
                where a.ActivityId == callForApplicationId 
                group a by new { s.Name }
                into stageGroup
                select new CfaGetStageStatDto
                {
                    Name = stageGroup.Key.Name,
                    Count = stageGroup.Count()
                }).ToListAsync();

            return new SuccessResponse<List<CfaGetStageStatDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = stagesStat
            };
        }

        private async Task ValidateCallForApplication(Guid callForApplicationId)
        {
            var isCallForApplicationExist = await  _repository.CallForApplication.ExistsAsync(x => x.Id == callForApplicationId);
            if (!isCallForApplicationExist)
            {
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);
            }
        }

        public async Task<SuccessResponse<GetCfaSubmissionStatDto>> GetCfaSubmission(Guid callForApplicationId)
        {
            var callForApplicationExist = await _repository.CallForApplication.ExistsAsync(x => x.Id == callForApplicationId);
            if (!callForApplicationExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);

            var callForApplication = await _repository.CallForApplication.FirstOrDefaultNoTracking(x => x.Id == callForApplicationId);
            var applicantCount = await _repository.Applicant
                .Get(x => x.ActivityId == callForApplicationId && x.ActivityType == EActivityType.CallForApplication.ToString())
                .CountAsync();

            return new SuccessResponse<GetCfaSubmissionStatDto>()
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetCfaSubmissionStatDto
                {
                    SubmissionCount = applicantCount,
                    TargetNumber = callForApplication.TargetNumber
                }
            };
        }

        public async Task<SuccessResponse<ApplicantDTO>> GetCallForApplicationApplicantById(Guid callForApplicationId, Guid applicantId)
        {
            CallForApplication callForApplication = await ValidateCallForApplicationIncludeStages(callForApplicationId);
            ApplicantDetail applicant = await ValidateApplicant(applicantId);

            LearningTrack learningTrack = new();
            if (applicant.LearningTrackId != null)
            {
                learningTrack = await _repository.LearningTrack.FirstOrDefaultNoTracking(x => x.Id == applicant.LearningTrackId);
            }

            var stage = new Stage();
            if (applicant.StageId != null)
            {
               stage = callForApplication.Stages.FirstOrDefault(x => x.Id == applicant.StageId.Value);
            }
            var applicantDto = new ApplicantDTO
            {
                Id = applicant.Id,
                FormId = applicant.FormId,
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                Email = applicant.Email,
                Gender = applicant.Gender,
                Country = applicant.Country,
                PhoneNumber = applicant.PhoneNumber,
                LearningTrack = learningTrack?.Title,
                Status = applicant.Status,
                StageId = applicant.StageId != null ? applicant.StageId : null,
                StageName = stage?.Name,
                StageIndex = stage?.Index ?? default,
                CurrentStageCount = callForApplication.Stages.Count,
                Age = applicant.DateOfBirth != null ? DateTime.Today.Year - applicant.DateOfBirth.Value.Year : 0,
            };

            return new SuccessResponse<ApplicantDTO>()
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = applicantDto
            };
        }

        private async Task<ApplicantDetail> ValidateApplicant(Guid applicantId)
        {
            var applicant = await _repository.Applicant.FirstOrDefaultNoTracking(x => x.Id == applicantId);
            if (applicant is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ApplicantDetailNotFound);
            
            return applicant;
        }

        private async Task<CallForApplication> ValidateCallForApplicationIncludeStages(Guid callForApplicationId)
        {
            var callForApplication = await _repository.CallForApplication
                            .QueryAll(x => x.Id == callForApplicationId)
                            .Include(x => x.Stages)
                            .FirstOrDefaultAsync();
            if (callForApplication is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);
            
            return callForApplication;
        }

        private string GetCallForApplicationOtherStatus(DateTime StartDate, DateTime EndDate)
        {
            var currentDate = DateTime.Now;
            if (currentDate.Date < StartDate.Date)
                return ECallForApplicationOtherStatus.NotStarted.GetDescription();

            if (StartDate.Date >= currentDate.Date && currentDate.Date <= EndDate.Date)
                return ECallForApplicationOtherStatus.Ongoing.GetDescription();

            return ECallForApplicationOtherStatus.Completed.GetDescription();
        }

        public async Task<SuccessResponse<GetCallForApplicationStatusDto>> CloseCallForApplication(Guid id)
        {
            var callForApplication = await GetCallForApplicationById(id);

            if (callForApplication is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationNotFound);

            callForApplication.IsClosed = true;
            callForApplication.UpdatedAt = DateTime.UtcNow;

            UserActivity userActivity = AuditLog.UserActivity(callForApplication, _webHelper.User().UserId, nameof(callForApplication), $"Closed a CFA under - {callForApplication.Programme.Title}", callForApplication.Id);

            _repository.CallForApplication.Update(callForApplication);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetCallForApplicationStatusDto>()
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = _mapper.Map<GetCallForApplicationStatusDto>(callForApplication)
            };
        }
    }
}

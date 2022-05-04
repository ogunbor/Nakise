using Application.Contracts.V1;
using Application.Contracts.V1.Activities;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Enums;
using Domian.Entities.ActivityForms;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Activities
{
    public class SurveyService: ISurveyService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;
        public SurveyService(
            IMapper mapper,
            IWebHelper webHelper, 
            IRepositoryManager repository)
        {
            _mapper = mapper;
            _webHelper = webHelper;
            _repository = repository;
        }

        public async Task<SuccessResponse<GetSurveyDTO>> CreateSurvey(CreateSurveyRequest request)
        {
            var activity = await GetActivity();

            await ProgrammeExist(request.ProgrammeId);

            await CheckDuplicateTitle(request.Title, request.ProgrammeId);

            var learningTracks = await GetLearningTracks(request.LearningTracks);

            var survey = _mapper.Map<Survey>(request);
            survey.ActivityId = activity.Id;

            await _repository.Survey.AddAsync(survey);
            if (learningTracks.Any())
            {
                survey.SurveyLearningTracks =
                    GetSurveyLearningTrackForCreation(request.LearningTracks, survey.Id);
            }

            var form = new ActivityForm
            {
                Id = Guid.NewGuid(),
                ActivityId = survey.Id,
                Name = EActivityType.Survey.ToString(),
                Type = EActivityType.Survey.ToString(),
                Description = survey.Description
            };

            UserActivity userActivity = AuditLog.UserActivity(survey, _webHelper.User().UserId, nameof(survey), $"Created a Survey - {survey.Title}", survey.Id);

            await _repository.ActivityForm.AddAsync(form);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetSurveyDTO>(survey);
            response.LearningTracks = learningTracks;

            return new SuccessResponse<GetSurveyDTO>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetSurveyDTO>> GetASurvey(Guid surveyId)
        {
            var survey = await _repository.Survey.Get(x => x.Id == surveyId)
                .Include(x => x.SurveyLearningTracks)
                .ThenInclude(x => x.LearningTrack)
                .FirstOrDefaultAsync();

            if (survey == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.SurveyNotFound}");
            }

            var response = _mapper.Map<GetSurveyDTO>(survey);

            return new SuccessResponse<GetSurveyDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetSurveyDTO>> UpdateSurvey(Guid assessmentId, UpdateSurveyRequest request)
        {
            var survey = await GetSurveyById(assessmentId);

            var learningTracks = await GetLearningTracks(request.LearningTracks);

            if (survey.Title.ToLower() != request.Title.ToLower())
            {
                await CheckDuplicateTitle(request.Title, survey.ProgrammeId);
            }

            _mapper.Map(request, survey);
            survey.UpdatedAt = DateTime.UtcNow;

            var learningTrackToRemove = await HandleLearningTrackUpdate(request.LearningTracks, assessmentId);

            UserActivity userActivity = AuditLog.UserActivity(survey, _webHelper.User().UserId, nameof(survey), $"Updated a Survey - {survey.Title}", survey.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetSurveyDTO>(survey);
            var learningTrackDto = learningTracks.Where(x => !learningTrackToRemove.Contains(x.Id)).ToList();
            response.LearningTracks = learningTrackDto;

            return new SuccessResponse<GetSurveyDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetApproveApplicantSurveyStatsDTO>> GetApprovedApplicantSurveyStats(Guid programId, Guid approvedApplicantId)
        {
            var applicantProgram = await _repository.ApprovedApplicantProgramme.Get(x => x.ProgrammeId == programId).FirstOrDefaultAsync();
            if (applicantProgram == null)
                throw new RestException(HttpStatusCode.NotFound, "Programmme does not exist");

            var learningTrackId = applicantProgram.LearningTrackId;
            var surveys = await _repository.Survey.Get(x => x.ProgrammeId == programId).Include(x => x.SurveyLearningTracks)
                .Where(x => x.SurveyLearningTracks.Any(l => l.LearningTrackId == learningTrackId))
                   .ToListAsync();

            var SurveyResponseCount = await _repository.SurveyResponse.CountAsync(x => x.ApprovedApplicantId == approvedApplicantId && x.ProgramId == programId);

            GetApproveApplicantSurveyStatsDTO approvedApplicantSurveyStats = new();
            approvedApplicantSurveyStats.Total = surveys.Count();
            approvedApplicantSurveyStats.Overdue = surveys.Where(x => x.EndDate > DateTime.Now).Count();
            approvedApplicantSurveyStats.Answered = SurveyResponseCount;
            approvedApplicantSurveyStats.NotAnswered = surveys.Count() - SurveyResponseCount;

            return new SuccessResponse<GetApproveApplicantSurveyStatsDTO>
            {
                Data = approvedApplicantSurveyStats,
                Success = true,
                Message = "Data retrieved successfully"
            };
        }
        #region reusuables
        private void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }
       
        public async Task DeleteSurvey(Guid surveyId)
        {
            var survey = await GetSurveyById(surveyId);
            
            CheckActivityDate(survey.EndDate);

            _repository.Survey.Remove(survey);

            UserActivity userActivity = AuditLog.UserActivity(survey, _webHelper.User().UserId, nameof(survey), $"Deleted a Survey - {survey.Title}", survey.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();
        }


        private async Task ProgrammeExist(Guid programmeId)
        {
            var programmeExist = await _repository.Programme.ExistsAsync(x => x.Id == programmeId);

            if (!programmeExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ProgrammeNotFound}");
            }
        }

        private async Task<Activity> GetActivity()
        {
            var activity = await _repository.Activity.FirstOrDefaultNoTracking(x => x.Type == EActivityType.Survey.ToString());

            if (activity == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ActivityNotFound}");
            }

            return activity;
        }

        private async Task CheckDuplicateTitle(string title, Guid programmeId)
        {
            var titleExist =
                await _repository.Survey.ExistsAsync(x => x.Title == title && x.ProgrammeId == programmeId);

            if (titleExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateTitle}");
            }
        }

        private async Task<List<SurveyLearningTrackDTO>> GetLearningTracks(List<Guid> learningTracksRequest)
        {
            var learningTracks = await _repository.LearningTrack.Get(x => learningTracksRequest.Contains(x.Id))
                .Select(x => new SurveyLearningTrackDTO { Id = x.Id, Title = x.Title })
                .ToListAsync();

            if (learningTracks.Count != learningTracksRequest.Count)
            {
                var learningTrackIds = learningTracks.Select(x => x.Id);
                var learningTrackIdsNotFound = learningTracksRequest.Except(learningTrackIds);
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.LearningTrackNotFound} {string.Join(",", learningTrackIdsNotFound)}");
            }

            return learningTracks;
        }
        private List<SurveyLearningTrack> GetSurveyLearningTrackForCreation(IEnumerable<Guid> learningTrackIds, Guid surveyId)
        {
            var assessmentLearningTracks = new List<SurveyLearningTrack>();
            foreach (var learningTrackId in learningTrackIds)
            {
                var assessmentLearningTrack = new SurveyLearningTrack
                {
                    LearningTrackId = learningTrackId,
                    SurveyId = surveyId
                };

                assessmentLearningTracks.Add(assessmentLearningTrack);
            }

            return assessmentLearningTracks;
        }

        private async Task<Survey> GetSurveyById(Guid surveyId)
        {
            var survey = await _repository.Survey.GetByIdAsync(surveyId);

            if (survey == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.SurveyNotFound}");
            }

            return survey;
        }

        private async Task<IEnumerable<Guid>> HandleLearningTrackUpdate(List<Guid> learningTrackRequest, Guid surveyId)
        {
            var assessmentLearningTracks = await _repository.SurveyLearningTrack.Get(x => x.SurveyId == surveyId)
                .ToListAsync();
            var learningTrackIds = assessmentLearningTracks.Select(x => x.LearningTrackId);

            var learningTrackForCreation = learningTrackRequest.Except(learningTrackIds);
            var assessmentLearningTracksForCreation = GetSurveyLearningTrackForCreation(learningTrackForCreation, surveyId);
            await _repository.SurveyLearningTrack.AddRangeAsync(assessmentLearningTracksForCreation);

            var learningTrackToRemove = learningTrackIds.Except(learningTrackRequest);
            var assessmentLearningTracksToRemove =
                assessmentLearningTracks.Where(x => learningTrackToRemove.Contains(x.LearningTrackId));
            _repository.SurveyLearningTrack.RemoveRange(assessmentLearningTracksToRemove);

            return learningTrackToRemove;

        }
        #endregion
    }
}

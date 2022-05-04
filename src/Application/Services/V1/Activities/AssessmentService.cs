using Application.Contracts.V1.Activities;
using Application.Enums;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Utils.ExternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Net;

namespace Application.Services.V1.Activities
{
    #region Settings
    public class AssessmentService: IAssessmentService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IWebHelper _webHelper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public AssessmentService(
            IMapper mapper,
            IRepositoryManager repository,
            IWebHelper webHelper,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["AssessmentBaseUrl"]);
            _configuration = configuration;
        }
        #endregion
        public async Task<SuccessResponse<GetAssessmentDTO>> CreateAssessment(CreateAssessmentRequest request)
        {
            var activity = await GetActivity();

            await ProgrammeExist(request.ProgrammeId);

            await CheckDuplicateTitle(request.Title, request.ProgrammeId);

            var learningTracks = await GetLearningTracks(request.LearningTracks);

            var assessmentToCreate = new CreateAssessmentDto
            {
                Title = request.Title,
                StartDateTime = request.StartDate,
                DueDateTime = request.DueDate,
                Duration = request.Duration,
                PassMark = request.PassMark,
                TotalObtainableScore = request.TotalObtainableScore,
                Status = EAssessmentStatus.DRAFT.ToString(),
            };

            var httpResponse = await _httpClient.PostAsJson("/api/v1/assessment/", assessmentToCreate, _configuration);
            var result = await httpResponse.ReadContentAs<AssessmentResult>();

            var assessment = _mapper.Map<Assessment>(request);
            assessment.ActivityId = activity.Id;
            assessment.AssessmentId = result.Id;
            assessment.Status = EAssessmentStatus.DRAFT.ToString();

            await _repository.Assessment.AddAsync(assessment);
            if (learningTracks.Any())
            {
                assessment.AssessmentLearningTracks =
                    GetAssessmentLearningTrackForCreation(request.LearningTracks, assessment.Id);
            }

            UserActivity userActivity = AuditLog.UserActivity(assessment, _webHelper.User().UserId, "Assessment", $"Created Assessment - {assessment.Title}", assessment.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetAssessmentDTO>(assessment);
            response.LearningTracks = learningTracks;

            return new SuccessResponse<GetAssessmentDTO>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetAssessmentDTO>> GetAssessment(Guid assessmentId)
        {
            var assessment = await _repository.Assessment.Get(x => x.Id == assessmentId)
                .Include(x => x.AssessmentLearningTracks)
                .ThenInclude(x => x.LearningTrack)
                .FirstOrDefaultAsync();

            if (assessment == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.AssessmentNotFound}");
            }

            var response = _mapper.Map<GetAssessmentDTO>(assessment);
            var learningTrack = assessment.AssessmentLearningTracks.Select(x => x.LearningTrack);
            response.LearningTracks = _mapper.Map<List<AssessmentLearningTrackDTO>>(learningTrack);

            return new SuccessResponse<GetAssessmentDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task DeleteAssessment(Guid assessmentId)
        {
            var assessment = await GetAssesssmentById(assessmentId);

            CheckActivityDate(assessment.EndDate);

            _repository.Assessment.Remove(assessment);

            UserActivity userActivity = AuditLog.UserActivity(assessment, _webHelper.User().UserId, "Assessment", $"Deleted Assessment - {assessment.Title}", assessment.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();
        }

        public async Task<SuccessResponse<GetAssessmentDTO>> UpdateAssessment(Guid assessmentId, UpdateAssessmentRequest request)
        {
            var assessment = await GetAssesssmentById(assessmentId);

            var learningTracks = await GetLearningTracks(request.LearningTracks);

            if (assessment.Title.ToLower() != request.Title.ToLower())
            {
                await CheckDuplicateTitle(request.Title, assessment.ProgrammeId);
            }

            _mapper.Map(request, assessment);
            assessment.UpdatedAt = DateTime.UtcNow;

            var learningTrackToRemove = await HandleLearningTrackUpdate(request.LearningTracks, assessmentId);

            UserActivity userActivity = AuditLog.UserActivity(assessment, _webHelper.User().UserId, "Assessment", $"Updated Assessment - {assessment.Title}", assessment.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetAssessmentDTO>(assessment);
            var learningTrackDto = learningTracks.Where(x => !learningTrackToRemove.Contains(x.Id)).ToList();
            response.LearningTracks = learningTrackDto;

            return new SuccessResponse<GetAssessmentDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }

        public async Task<AssessmentDto> GetAllAssessmentAsync(AssessmentParameters parameters)
        {
            var response = await _httpClient.GetAsJson("/api/v1/assessment", _configuration, parameters);

            return await response.ReadContentAs<AssessmentDto>();
        }

        public async Task<SuccessResponse<AssessmentSessionDto>> StartAssessment(Guid assessmentId, Guid approvedApplicantId)
        {
            var assessment = await _repository.Assessment.Get(x => x.Id == assessmentId)
                .Include(z => z.Programme)
                    .ThenInclude(l => l.LearningTracks)
                .Include(l => l.AssessmentLearningTracks)
                .FirstOrDefaultAsync();

            ValidateAssessment(assessment);

            var approvedApplicantProgram = _repository.ApprovedApplicantProgramme.Get(x => x.ProgrammeId == assessment.ProgrammeId)
                .Include(a => a.ApprovedApplicant)
                    .ThenInclude(u => u.User)
                .Include(l => l.learningTrack)
                .FirstOrDefault();

            //verifying if user belongs to the program that the Assessment is ment for
            if (approvedApplicantProgram == null)
                throw new RestException(HttpStatusCode.NotFound, "This Assessment does not fall within your programme");
            
            //verifying if user learning track is allowed to take this assessment
            if (!assessment.AssessmentLearningTracks.Any(l => l.LearningTrackId == approvedApplicantProgram.LearningTrackId))
                throw new RestException(HttpStatusCode.NotFound, "You learningTrack not permitted to take this assessment");

            CreateAssessmentSessionDto requestBody = new()
            {
                Name = $"{approvedApplicantProgram.ApprovedApplicant.User.FirstName} {approvedApplicantProgram.ApprovedApplicant.User.LastName}",
                Email = approvedApplicantProgram.ApprovedApplicant.User.Email,
                ApprovedApplicantId = approvedApplicantProgram.ApprovedApplicantId.ToString(),
                AssessmentId = assessment.AssessmentId
            };

            //making a syncroneous request to Assessment service to create a assessment sesion for user
            var jsonResponse = await _httpClient.PostAsJson<CreateAssessmentSessionDto>("api/v1/assessment/session/start/", requestBody, _configuration);
            if (!jsonResponse.IsSuccessStatusCode)
                throw new RestException(jsonResponse.StatusCode, jsonResponse.ReasonPhrase, "Unable to start this assessment");

            var response = await jsonResponse.ReadContentAs<AssessmentSessionDto>();
            response.AssessmentId = assessment.Id;

            //checking if the session has already been created, if not create it
            var assessmentSession = await _repository.AssessmentSession.Get(x => x.SessionId == response.Id).FirstOrDefaultAsync();
            if (assessmentSession is null)
            {
                AssessmentSession session = _mapper.Map<AssessmentSession>(response);
                await _repository.AssessmentSession.AddAsync(session);
            }
            
            UserActivity userActivity = AuditLog.UserActivity(assessment, _webHelper.User().UserId, "Assessment", $"Started an Assessment - {assessment.Title}", assessment.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<AssessmentSessionDto>()
            {
                Message = "Assessment Started",
                Data = response,
                Success = true
            };

        }
        public  async Task<SuccessResponse<QuestionAnswerDto>> SubmitAssessmentQuestionAnswer(Guid approvedApplicantId,QuestionAnswerDto model)
        {
            var session = await _repository.AssessmentSession.Get(x => x.SessionId == model.SessionId && x.ApprovedApplicantId == approvedApplicantId)
                .Include(a => a.Assessment)
                .FirstOrDefaultAsync();

            if (session is null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid session");

            if (session.Assessment.EndDate < DateTime.Now)
                throw new RestException(HttpStatusCode.BadRequest, "Assessment has been closed");

            QuestionAnswerDto requestBody = new()
            {
                SessionId = session.SessionId,
                QuestionId = model.QuestionId,
                OptionSelectedId = model.OptionSelectedId,
            };
            //making a syncroneous request to Assessment service to create a assessment sesion for user
            var jsonResponse = await _httpClient.PostAsJson<QuestionAnswerDto>("api/v1/assessment/session/submit/", requestBody, _configuration);

            if (!jsonResponse.IsSuccessStatusCode)
                throw new RestException(jsonResponse.StatusCode, jsonResponse.ReasonPhrase, "Unable to submit your Answer");

            var response = await jsonResponse.ReadContentAs<QuestionAnswerDto>();

            return new SuccessResponse<QuestionAnswerDto>
            {
                Message = "Question successfuly submitted",
                Success = true,
                Data = response
            };

        }
        public async Task<SuccessResponse<string>> CompleteAssessmentSession(Guid approvedApplicantId, SessionCompletionDto model)
        {
            var session = await _repository.AssessmentSession.Get(x => x.SessionId == model.SessionId && x.ApprovedApplicantId == approvedApplicantId)
                .Include(a => a.Assessment)
                .FirstOrDefaultAsync();

            if (session is null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid session");

            if (session.Assessment.EndDate < DateTime.Now)
                throw new RestException(HttpStatusCode.BadRequest, "Assessment has been closed");

            //making a syncroneous request to Assessment service to complete an assessment session
            var jsonResponse = await _httpClient.PostAsJson<QuestionAnswerDto>($"api/v1/assessment/session/{model.SessionId}/complete/",null,  _configuration);

            if (!jsonResponse.IsSuccessStatusCode)
                throw new RestException(jsonResponse.StatusCode, jsonResponse.ReasonPhrase, "Unable to submit your Answer");

            var response = await jsonResponse.ReadContentAs<QuestionAnswerDto>();

            return new SuccessResponse<string>
            {
                Message = "Session successfuly completed",
                Success = true,
                Data = ""
            };
        }

        public async Task<SuccessResponse<CreateQuestionResponseDto>> CreateSingleQuestion(Guid id, CreateQuestionDto input)
        {
            var assessment = await _repository.Assessment.FirstOrDefault(x => x.Id == id);
            if (assessment is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.AssessmentNotFound);

            var question = new QuestionDto
            {
                Options = input.Options,
                Body = input.Body,
                Type = EQuestionType.SINGLE_CHOICE.ToString(),
                Assessment = assessment.AssessmentId,
                Score = input.Score
            };

            var httpRespones = await _httpClient.PostAsJson("api/v1/assessment/data/question/", question, _configuration);
            if (!httpRespones.IsSuccessStatusCode)
                throw new RestException(HttpStatusCode.BadRequest, httpRespones.ReasonPhrase);

            var response = await httpRespones.ReadContentAs<CreateQuestionResponseDto>();

            return new SuccessResponse<CreateQuestionResponseDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<AnswersPreviewDto>> GetAssessmentSessionSummary(Guid approvedApplicantId, SessionCompletionDto model)
        {
            var session = await _repository.AssessmentSession.Get(x => x.SessionId == model.SessionId && x.ApprovedApplicantId == approvedApplicantId)
                .Include(a => a.Assessment)
                .FirstOrDefaultAsync();

            if (session is null)
                throw new RestException(HttpStatusCode.BadRequest, "Invalid session");

            if (session.Assessment.EndDate < DateTime.Now)
                throw new RestException(HttpStatusCode.BadRequest, "Assessment has been closed");

            //making a syncroneous request to Assessment service to get session summary
            var jsonResponse = await _httpClient.GetAsJson($"api/v1/assessment/session/{model.SessionId}/result/", _configuration);

            if (!jsonResponse.IsSuccessStatusCode)
                throw new RestException(jsonResponse.StatusCode, jsonResponse.ReasonPhrase, "Unable to get your session summary");

            var response = await jsonResponse.ReadContentAs<AnswersPreviewDto>();


            return new SuccessResponse<AnswersPreviewDto>
            {
                Message = "Session successfuly completed",
                Success = true,
                Data = response
            };
        }
        #region Reusuables
        private static void ValidateAssessment(Assessment assessment)
        {
            if (assessment == null)
                throw new RestException(HttpStatusCode.NotFound, "Assessment not found");

            if (assessment.StartDate > DateTime.Now)
                throw new RestException(HttpStatusCode.Unauthorized, "This assessment is yet to start");

            if (assessment.EndDate < DateTime.Now)
                throw new RestException(HttpStatusCode.Unauthorized, "This assessment has closed");
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
            var activity = await _repository.Activity.FirstOrDefaultNoTracking(x => x.Type == EActivityType.Assessment.ToString());
            
            if (activity == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ActivityNotFound}");
            }

            return activity;
        }

        private async Task<Assessment> GetAssesssmentById(Guid assessmentId)
        {
            var assessment = await _repository.Assessment.GetByIdAsync(assessmentId);

            if (assessment == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.AssessmentNotFound}");
            }

            return assessment;
        }

        private async Task CheckDuplicateTitle(string title, Guid programmeId)
        {
            var titleExist =
                await _repository.Assessment.ExistsAsync(x => x.Title == title && x.ProgrammeId == programmeId);

            if (titleExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateTitle}");
            }
        }

        private async Task<List<AssessmentLearningTrackDTO>> GetLearningTracks(List<Guid> learningTracksRequest)
        {
            var learningTracks = await _repository.LearningTrack.Get(x => learningTracksRequest.Contains(x.Id))
                .Select(x => new AssessmentLearningTrackDTO{Id = x.Id, Title = x.Title})
                .ToListAsync();

            if (learningTracks.Count != learningTracksRequest.Count)
            {
                var learningTrackIds = learningTracks.Select(x => x.Id);
                var learningTrackIdsNotFound = learningTracksRequest.Except(learningTrackIds);
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.LearningTrackNotFound} {string.Join(",", learningTrackIdsNotFound)}");
            }

            return learningTracks;
        }

        private List<AssessmentLearningTrack> GetAssessmentLearningTrackForCreation(IEnumerable<Guid> learningTrackIds, Guid assessmentId)
        {
            var assessmentLearningTracks = new List<AssessmentLearningTrack>();
            foreach (var learningTrackId in learningTrackIds)
            {
                var assessmentLearningTrack = new AssessmentLearningTrack
                {
                    LearningTrackId = learningTrackId,
                    AssessmentId = assessmentId
                };

                assessmentLearningTracks.Add(assessmentLearningTrack);
            }

            return assessmentLearningTracks;
        }

        private async Task<IEnumerable<Guid>> HandleLearningTrackUpdate(List<Guid> learningTrackRequest, Guid assessmentId)
        {
            var assessmentLearningTracks = await _repository.AssessmentLearningTrack.Get(x => x.AssessmentId == assessmentId)
                .ToListAsync();
            var learningTrackIds = assessmentLearningTracks.Select(x => x.LearningTrackId);

            var learningTrackForCreation = learningTrackRequest.Except(learningTrackIds);
            var assessmentLearningTracksForCreation = GetAssessmentLearningTrackForCreation(learningTrackForCreation, assessmentId);
            await _repository.AssessmentLearningTrack.AddRangeAsync(assessmentLearningTracksForCreation);

            var learningTrackToRemove = learningTrackIds.Except(learningTrackRequest);
            var assessmentLearningTracksToRemove =
                assessmentLearningTracks.Where(x => learningTrackToRemove.Contains(x.LearningTrackId));
            _repository.AssessmentLearningTrack.RemoveRange(assessmentLearningTracksToRemove);

            return learningTrackToRemove;
        }

        private void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }
        #endregion
    }
}

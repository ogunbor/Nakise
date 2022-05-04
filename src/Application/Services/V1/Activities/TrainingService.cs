using Application.Contracts.V1.Activities;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Activities
{
    public class TrainingService : ITrainingService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;

        public TrainingService(IMapper mapper,
            IWebHelper webHelper, 
            IRepositoryManager repository)
        {
            _mapper = mapper;
            _webHelper = webHelper;
            _repository = repository;
        }

        private Guid ValidateUser()
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);
            return organizationId;
        }

        private async Task AddTrainingLearningTracks(Guid trainingId, List<LearningTrack> learningTracks)
        {
            var trainingLearningTracks = new List<TrainingLearningTrack>();
            foreach (var learningTrack in learningTracks)
            {
                var trainingLearningTrack = new TrainingLearningTrack
                {
                    TrainingId = trainingId,
                    LearningTrackId = learningTrack.Id
                };
                trainingLearningTracks.Add(trainingLearningTrack);
            }
            await _repository.TrainingLearningTrack.AddRangeAsync(trainingLearningTracks);
        }

        private async Task<List<LearningTrack>> ValidateTrainingLearningTracks(ICollection<Guid> learningTrackIds)
        {
            var learningTracks = await _repository.LearningTrack.Get(x => learningTrackIds.Contains(x.Id)).ToListAsync();
            var notFoundIds = learningTrackIds.Except(learningTracks.Select(x => x.Id));
            if (notFoundIds.Any())
            {
                var notFoundId = notFoundIds.FirstOrDefault();
                var errorMsg = $"{ResponseMessages.LearningTrackIdNotFound}";
                throw new RestException(HttpStatusCode.NotFound, string.Format(errorMsg, notFoundId));
            }

            return learningTracks;
        }

        private async Task IsUserAuthorized(Guid organizationId, Guid programmeId)
        {
            var programme = await _repository.Programme.FirstOrDefaultNoTracking(x => x.Id == programmeId);
            if (programme.OrganizationId != organizationId)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);
        }

        public async Task<SuccessResponse<GetTrainingDto>> CreateTrainingActivity(CreateTrainingInputDto model)
        {
            Guid organizationId = ValidateUser();

            var titleExist = await _repository.Training.ExistsAsync(x => x.Title.ToLower() == model.Title.ToLower());
            if (titleExist)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DuplicateTitle);

            var activity = await _repository.Activity.FirstOrDefault(x => x.Type == EActivityType.Training.ToString());
            if (activity == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

            var programme = await _repository.Programme.FirstOrDefault(x => x.Id == model.ProgrammeId && x.OrganizationId == organizationId);
            if (programme == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ProgrammeNotFound);

            var training = _mapper.Map<Training>(model);
            training.ActivityId = activity.Id;
            await _repository.Training.AddAsync(training);

            var learningTracks = await ValidateTrainingLearningTracks(model.LearningTrackIds);
            await AddTrainingLearningTracks(training.Id, learningTracks);

            UserActivity userActivity = AuditLog.UserActivity(training, _webHelper.User().UserId, nameof(training), $"Created a Training - {training.Title}", training.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetTrainingDto>(training);
            response.Programme = _mapper.Map<TrainingProgrammeDto>(programme);
            response.LearningTracks = _mapper.Map<ICollection<TrainingLearningTrackDto>>(learningTracks);

            return new SuccessResponse<GetTrainingDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetTrainingDto>> GetTrainingActivity(Guid id)
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            var training = await _repository.Training.Get(x => x.Id == id)
                .Include(x => x.Programme)
                .Include(x => x.TrainingLearningTracks)
                .ThenInclude(x => x.LearningTrack)
                .FirstOrDefaultAsync();

            if (training == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.TrainingNotFound);

            var response = _mapper.Map<GetTrainingDto>(training);
            response.Programme = _mapper.Map<TrainingProgrammeDto>(training.Programme);
            response.LearningTracks = _mapper.Map<ICollection<TrainingLearningTrackDto>>(training.TrainingLearningTracks);

            return new SuccessResponse<GetTrainingDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task DeleteTrainingActivity(Guid id)
        {
            var organizationId = ValidateUser();

            var training = await _repository.Training.Get(x => x.Id == id)
               .Include(x => x.TrainingLearningTracks)
               .FirstOrDefaultAsync();

            if (training == null)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.TrainingNotFound);

            await IsUserAuthorized(organizationId, training.ProgrammeId);

            UserActivity userActivity = AuditLog.UserActivity(training, _webHelper.User().UserId, nameof(training), $"Deleted a Training - {training.Title}", training.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            _repository.Training.Remove(training);
            await _repository.SaveChangesAsync();
        }

        public async Task<SuccessResponse<GetTrainingDto>> UpdateTrainingActivity(Guid id, UpdateTrainingInputDto model)
        {
            var organizationId = ValidateUser();

            var training = await _repository.Training.Get(x => x.Id == id)
                .Include(x => x.TrainingLearningTracks)
                .Include(x => x.Programme)
                .FirstOrDefaultAsync();

            if (training == null)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.TrainingNotFound);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     

            if (training.Programme.OrganizationId != organizationId)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            if (!model.Title.Equals(training.Title, StringComparison.CurrentCultureIgnoreCase))
            {
                var titleExist = await _repository.Training.ExistsAsync(x => x.Title.ToLower() == model.Title.ToLower() && x.Id != training.Id);
                if (titleExist)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.DuplicateTitle);
            }

            await IsUserAuthorized(organizationId, training.ProgrammeId);

            _repository.TrainingLearningTrack.RemoveRange(training.TrainingLearningTracks);

            _mapper.Map(model, training);

            var learningTracks = await ValidateTrainingLearningTracks(model.LearningTrackIds);
            await AddTrainingLearningTracks(training.Id, learningTracks);

            UserActivity userActivity = AuditLog.UserActivity(training, _webHelper.User().UserId, nameof(training), $"Updated a Training - {training.Title}", training.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetTrainingDto>(training);
            response.Programme = _mapper.Map<TrainingProgrammeDto>(training.Programme);
            response.LearningTracks = _mapper.Map<ICollection<TrainingLearningTrackDto>>(learningTracks);

            return new SuccessResponse<GetTrainingDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }
    }
}

using Application.Contracts.V1.Activities;
using Application.DTOs;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;

namespace Application.Services.V1.Activities
{
    public class FormService : IFormService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;
        public FormService(
            IMapper mapper,
            IRepositoryManager repository, 
            IWebHelper webHelper)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
        }

        //create form
        public async Task<SuccessResponse<GetFormDTO>> CreateForm(CreateFormDTO formDTO)
        {
            var activity = await GetFormActivity();

            await ProgrammeExist(formDTO.ProgrammeId);

            await CheckDuplicateTitle(formDTO.Title, formDTO.ProgrammeId);

            var learningTracks = await GetLearningTracks(formDTO.LearningTracks);

            var form = _mapper.Map<Form>(formDTO);
            form.ActivityId = activity.Id;

            await _repository.Form.AddAsync(form);
            if (learningTracks.Any())
            {
                form.FormLearningTracks = GetFormLearningTracksForCreation(formDTO.LearningTracks, form.Id);
            }

            UserActivity userActivity = AuditLog.UserActivity(form, _webHelper.User().UserId, nameof(form), $"Created a form - {form.Title}", form.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var getFormDTO = _mapper.Map<GetFormDTO>(form);
            getFormDTO.LearningTracks = learningTracks;

            return new SuccessResponse<GetFormDTO>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = getFormDTO
            };
        }

        public async Task<SuccessResponse<GetFormDTO>> UpdateForm(Guid formId, UpdateFormDTO formDTO)
        {
            var form = await GetFormById(formId);

            var learningTracks = await GetLearningTracks(formDTO.LearningTracks);

            if (form.Title.Equals(formDTO.Title, StringComparison.CurrentCultureIgnoreCase))
            {
                await CheckDuplicateTitle(formDTO.Title, form.ProgrammeId);
            }

            _mapper.Map(formDTO, form);
            form.UpdatedAt = DateTime.UtcNow;

            var formLearningTracks = await _repository.FormLearningTrack.Get(x => x.FormId == formId).ToListAsync();
            var learningTrackIds = formLearningTracks.Select(x => x.LearningTrackId);

            var learningTrackForCreation = formDTO.LearningTracks.Except(learningTrackIds);
            var formLearningTracksForCreation = GetFormLearningTracksForCreation(learningTrackForCreation, formId);
            await _repository.FormLearningTrack.AddRangeAsync(formLearningTracksForCreation);

            var learningTrackToRemove = learningTrackIds.Except(formDTO.LearningTracks);
            var assessmentLearningTracksToRemove =
                formLearningTracks.Where(x => learningTrackToRemove.Contains(x.LearningTrackId));
            _repository.FormLearningTrack.RemoveRange(assessmentLearningTracksToRemove);


            UserActivity userActivity = AuditLog.UserActivity(form, _webHelper.User().UserId, nameof(form), $"Created a form - {form.Title}", form.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetFormDTO>(form);
            var learningTrackDto = learningTracks.Where(x => !learningTrackToRemove.Contains(x.Id)).ToList();
            response.LearningTracks = learningTrackDto;

            return new SuccessResponse<GetFormDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }        

        public async Task<SuccessResponse<GetFormDTO>> GetForm(Guid formId)
        {
            var form = await _repository.Form.Get(x => x.Id == formId)
                .Include(x => x.FormLearningTracks)
                .ThenInclude(x => x.LearningTrack)
                .FirstOrDefaultAsync();

            if (form == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.FormNotFound}");
            }

            var getFormDTO = _mapper.Map<GetFormDTO>(form);
            return new SuccessResponse<GetFormDTO>
            {
                Message = ResponseMessages.DeletionSuccessResponse,
                Data = getFormDTO
            };
        }

        private void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }

        public async Task<SuccessResponse<object>> DeleteForm(Guid formId)
        {
            var form = await GetFormById(formId);

            CheckActivityDate(form.EndDate);

            _repository.Form.Remove(form);

            UserActivity userActivity = AuditLog.UserActivity(form, _webHelper.User().UserId, nameof(form), $"Deleted a form - {form.Title}", form.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeletionSuccessResponse
            };
        }
        
        private async Task ActivityExist(Guid activityId)
        {
            var activityExists = await _repository.Activity.ExistsAsync(x => x.Id == activityId);
            if (activityExists)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidActivity);
        }

        private async Task ProgrammeExist(Guid programmeId)
        {
            var programmeExists = await _repository.Programme.ExistsAsync(x => x.Id == programmeId);
            if (!programmeExists)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.ProgrammeNotFound);
        }


        private async Task CheckDuplicateTitle(string title, Guid programmeId)
        {
            var titleExist =
                await _repository.Form.ExistsAsync(x => x.Title == title && x.ProgrammeId == programmeId);
 
            if (titleExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateTitle}");
            }
        }
 
        private async Task<List<GetLearningTrackFormDTO>> GetLearningTracks(List<Guid> learningTracksRequest)
        {
            var learningTracks = await _repository.LearningTrack.Get(x => learningTracksRequest.Contains(x.Id))
                .Select(x => new GetLearningTrackFormDTO{Id = x.Id, Title = x.Title})
                .ToListAsync();
 
            if (learningTracks.Count != learningTracksRequest.Count)
            {
                var learningTrackIds = learningTracks.Select(x => x.Id);
                var learningTrackIdsNotFound = learningTracksRequest.Except(learningTrackIds);
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.LearningTrackNotFound} {string.Join(",", learningTrackIdsNotFound)}");
            }
 
            return learningTracks;
        }

        private List<FormLearningTrack> GetFormLearningTracksForCreation(IEnumerable<Guid> learningTrackIds, Guid formId)
        {
            var formLearningTracks = new List<FormLearningTrack>();

            foreach (var learningTrackId in learningTrackIds)
            {
                formLearningTracks.Add(new FormLearningTrack{FormId =  formId, LearningTrackId = learningTrackId});
            }

            return formLearningTracks;
        }

        private async Task<Form> GetFormById(Guid formId)
        {
            var form = await _repository.Form.GetByIdAsync(formId);

            if (form == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.FormNotFound}");
            }

            return form;
        }

        private async Task<Activity> GetFormActivity()
        {
            var activity = await _repository.Activity.FirstOrDefault(x => x.Type == EActivityType.Forms.ToString());
            
            if (activity == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ActivityNotFound}");
            }

            return activity;
        }
    }
}
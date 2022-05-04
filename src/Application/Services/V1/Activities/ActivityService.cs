using Application.Contracts.V1.Activities;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Activities
{
    public class ActivityService : IActivityService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public ActivityService(
            IMapper mapper, IRepositoryManager repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        //get list of all activities
        public async Task<SuccessResponse<IEnumerable<GetActivityDTO>>> GetActivities()
        {
            var activities = await _repository.Activity.GetAllAsync();
            var activityDTO = _mapper.Map<List<GetActivityDTO>>(activities);

            return new SuccessResponse<IEnumerable<GetActivityDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = activityDTO
            };           
        }

        //get activity by Id
        public async Task<SuccessResponse<GetActivityDTO>> GetActivityById(Guid id)
        {
            var activity = await _repository.Activity.GetByIdAsync(id);
            if (activity == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

            var activityDTO = _mapper.Map<GetActivityDTO>(activity);
            return new SuccessResponse<GetActivityDTO>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = activityDTO
            };
        }

        public async Task<PagedResponse<IEnumerable<GetProgrammeActivitiesDTO>>> GetProgrammeActivities(Guid programmeId, ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var folderName = Path.Combine("wwwroot", "DataAccess", "dbo.activities.sql");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (!File.Exists(filepath))
                return new PagedResponse<IEnumerable<GetProgrammeActivitiesDTO>>();

            var query = File.ReadAllText(filepath).Replace("\n", " ").Replace("\r", " ");
            var id = new SqlParameter("@programmeId", programmeId);
            var activityQuery = _repository.ProgrammeActivity.FromSqlRaw(query, id).Select(x =>
                new GetProgrammeActivitiesDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Activity = new ActivityDTO
                    {
                        Id = x.ActivityId,
                        Name = x.Name,
                        Type = x.Type
                    }
                }).OrderByDescending(x => x.StartDate);

            var programmeActivities = await PagedList<GetProgrammeActivitiesDTO>.Create(activityQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);

            var page = PageUtility<GetProgrammeActivitiesDTO>.CreateResourcePageUrl(parameter, name, programmeActivities, urlHelper);

            var response = new PagedResponse<IEnumerable<GetProgrammeActivitiesDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = programmeActivities,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        public void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }
    }
}

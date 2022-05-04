using AutoMapper;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    class ActivityMapper : Profile
    {
        public ActivityMapper()
        {
            CreateMap<Activity, GetActivityDTO>();

            CreateMap<CallForApplication, GetProgrammeActivitiesDTO>();

            CreateMap<Activity, ActivityDTO>();
        }
    }
}

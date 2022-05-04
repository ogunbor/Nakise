using AutoMapper;
using Domain.Entities.Actvities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<CreateEventDTO, Event>();

            CreateMap<Event, GetEventDTO>().ReverseMap();
            CreateMap<UpdateEventDTO, Event>();
            CreateMap<Event, GetEventDTO>()
                .ForMember(dest => dest.LearningTracks, opt => opt.MapFrom((src, dest) => src.EventLearningTracks));
        }
    }
}
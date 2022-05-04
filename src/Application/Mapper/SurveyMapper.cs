using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    class SurveyMapper: Profile
    {
        public SurveyMapper()
        {

            CreateMap<CreateSurveyRequest, Survey>();
            CreateMap<Survey, GetSurveyDTO>()
                .ForMember(dest => dest.LearningTracks, opt => opt.MapFrom(src => src.SurveyLearningTracks));

            CreateMap<UpdateSurveyRequest, Survey>();

            CreateMap<SurveyLearningTrack, SurveyLearningTrackDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LearningTrack.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningTrack.Title));

            CreateMap<LearningTrack, SurveyLearningTrack>();
        }
    }
}

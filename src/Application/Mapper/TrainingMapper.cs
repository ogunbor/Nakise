using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class TrainingMapper : Profile
    {
        public TrainingMapper()
        {
            CreateMap<CreateTrainingInputDto, Training>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom((src, dest) => dest.Title = src.Title.Trim()))
                .ForMember(dest => dest.TrainingLearningTracks, opt => opt.Ignore());

            CreateMap<Training, GetTrainingDto>()
                .ForMember(dest => dest.Programme, opt => opt.Ignore())
                .ForMember(dest => dest.LearningTracks, opt => opt.Ignore());

            CreateMap<LearningTrack, TrainingLearningTrackDto>();
            CreateMap<Programme, TrainingProgrammeDto>();
            CreateMap<TrainingLearningTrack, TrainingLearningTrackDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom((src, dest) => dest.Id = src.LearningTrack.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom((src, dest) => dest.Title = src.LearningTrack.Title));
            CreateMap<UpdateTrainingInputDto, Training>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom((src, dest) => src.Title.Trim() ?? dest.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom((src, dest) => src.Description ?? dest.Description))
                .ForMember(dest => dest.TrainingLearningTracks, opt => opt.Ignore());
        }
    }
}

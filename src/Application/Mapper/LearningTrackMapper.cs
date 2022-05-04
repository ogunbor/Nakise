using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Actvities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class LearningTrackMapper : Profile
    {
        public LearningTrackMapper()
        {
             CreateMap<LearningTrack, GetLearningTrackDTO>();
            CreateMap<CreateLearningTrackRequest, LearningTrack>();

            CreateMap<UpdateLearningTrackRequest, LearningTrack>();

            CreateMap<LearningTrack, GetLearningTrackDTO>()
                .ForMember(dest => dest.Facilitators, opt => opt.MapFrom(src => src.LearningTrackFacilitators));
                
            CreateMap<LearningTrackFacilitator, FacilitatorDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Facilitator.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Facilitator.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Facilitator.LastName))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Facilitator.Picture));
            CreateMap<LearningTrack, GetLearningTrackDto>();
            CreateMap<LearningTrackFacilitator, GetAllFacilitatorsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Facilitator.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Facilitator.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Facilitator.LastName))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Facilitator.Picture))
                .ForMember(dest => dest.LearningTrack, opt => opt.MapFrom(src => src.LearningTrack));
            CreateMap<ApplicantDetail, GetLearningTrackApplicantsDTO>();
            CreateMap<EventLearningTrack, GetLearningTrackEventDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LearningTrackId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningTrack.Title));
        }
    }
}

using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class AssessmentMapper: Profile
    {
        public AssessmentMapper()
        {
            CreateMap<CreateAssessmentRequest, Assessment>();
            CreateMap<Assessment, GetAssessmentDTO>();
            CreateMap<UpdateAssessmentRequest, Assessment>();

            CreateMap<AssessmentLearningTrack, AssessmentLearningTrackDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LearningTrack.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LearningTrack.Title));

            CreateMap<LearningTrack, AssessmentLearningTrackDTO>();
            CreateMap<AssessmentSessionDto, AssessmentSession>()
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id));
        }
    }
}

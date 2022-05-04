using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domian.Entities.ActivityForms;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class CallForApplicationMapper : Profile
    {
        public CallForApplicationMapper()
        {
            CreateMap<CreateCallForApplicationInputDto, CallForApplication>()
                .ForMember(dest => dest.Stages, opt => opt.Ignore());
            CreateMap<CreateStageDto, Stage>();
            CreateMap<Programme, GetProgrammeDto>();
            CreateMap<Stage, GetStageDto>();
            CreateMap<CallForApplication, GetCallForApplicationDto>()
                .ForMember(dest => dest.Stages, opt => opt.Ignore())
                .ForMember(dest => dest.Programme, opt => opt.Ignore())
                .ForMember(dest => dest.ActivityForm, opt => opt.Ignore());
            CreateMap<UpdateCallForApplicationInputDto, CallForApplication>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom((src, dest) => src.Title ?? dest.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom((src, dest) => src.Description ?? dest.Description))
                .ForMember(dest => dest.Target, opt => opt.MapFrom((src, dest) => src.Target ?? dest.Target))
                .ForMember(dest => dest.SuccessMessageBody  , opt => opt.MapFrom((src, dest) => src.SuccessMessageBody ?? dest.SuccessMessageBody))
                .ForMember(dest => dest.SuccessMessageTitle, opt => opt.MapFrom((src, dest) => src.SuccessMessageTitle ?? dest.SuccessMessageTitle))
                .ForMember(dest => dest.Stages, opt => opt.Ignore());
            CreateMap<ActivityForm, GetActivityFormDto>();
            CreateMap<CallForApplication, GetCallForApplicationStatusDto>();
        }
    }
}

using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class ProgrammeMapper: Profile
    {
        public ProgrammeMapper()
        {
            CreateMap<CreateProgrammeRequest, Programme>().AfterMap((src, dest) =>
            {
                dest.GenderOption = src.GenderOption != null ? string.Join(",", src.GenderOption) : null;
                dest.Status = EProgrammeStatus.NotStarted.GetDescription();
            });
            CreateMap<Programme, GetProgrammeDTO>()
                .ForMember(dest => dest.GenderOption, opt => opt.MapFrom((src, dest) => !string.IsNullOrWhiteSpace(src.GenderOption) ? src.GenderOption.Split(",") : null));
            CreateMap<User, ManagerDTO>();
            CreateMap<UpdateProgrammeRequest, Programme>().AfterMap((src, dest) =>
            {
                dest.GenderOption = src.GenderOption != null ? string.Join(",", src.GenderOption) : null;
            });
            CreateMap<Programme, GetAllProgrammmeDTO>()
                .ForMember(dest => dest.Managers, opt => opt.MapFrom(src => src.ProgrammeManagers))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
            CreateMap<ProgrammeManager, ManagerDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Manager.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Manager.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Manager.LastName))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Manager.Picture));

            CreateMap<ProgrammeCategory, GetProgrammeCategory>();

            CreateMap<ProgrammeSponsor, GetProgrammeSponsor>();
            CreateMap<ProgrammeManager, GetAllProgrammeManagerDto>()
                .ForMember(dest => dest.Programme, opt => opt.MapFrom(src => src.Programme))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Manager.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Manager.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Manager.LastName))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Manager.Picture));
            CreateMap<Programme, ProgrammeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

            CreateMap<Programme, ProgrammePreviewDTO>()
                .ForMember(dest => dest.Managers, opt => opt.MapFrom(src => src.ProgrammeManagers))
                .ForMember(dest => dest.LearningTrackCount, opt => opt.MapFrom(src => src.LearningTracks.Count));
        }
    }
}
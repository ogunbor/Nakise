using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class PeopleMapper : Profile
    {
        public PeopleMapper()
        {
            CreateMap<User, GetProgrammeManagerDto>()
                .ForMember(dest => dest.Programmes, opt => opt.MapFrom(src => src.ProgrammeManagers));
            CreateMap<User, GetSingleProgrammeManagerDto>();
            CreateMap<ProgrammeManager, ProgrammeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProgrammeId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Programme.Title));
            CreateMap<ProgrammeManager, GetUserProgrammeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProgrammeId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Programme.Title))
                .ForMember(dest => dest.Sponsor, opt => opt.MapFrom(src => src.Programme.Sponsor))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Programme.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Programme.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom((src, dest) =>
                {
                    var status = string.Empty;
                    if (src.Programme.StartDate.Date < DateTime.Now.Date)
                        dest.Status = "Not Started";
                    if (src.Programme.EndDate.Date < DateTime.Now.Date)
                        dest.Status = "Ongoing";
                    if (src.Programme.EndDate.Date > DateTime.Now.Date)
                        dest.Status = "Completed";

                    return dest.Status;
                }))
                .AfterMap((src, dest, context) =>
                {
                    dest.ProgrammeManagers = context.Mapper.Map<ICollection<ProgrammeManagerDto>>(src.Programme.ProgrammeManagers);
                });
            CreateMap<ProgrammeManager, ProgrammeManagerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Manager.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Manager.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Manager.LastName))
                .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.Manager.Picture));
        }
    }
}

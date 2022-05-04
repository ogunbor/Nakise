using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class ApprovedApplicantMapper : Profile
    {
        public ApprovedApplicantMapper()
        {
            CreateMap<ApplicantDetail, ApprovedApplicant>();
            CreateMap<UpdateBeneficiaryDTO, ApprovedApplicant>()
                .ForMember(dest => dest.Country, opt => opt.MapFrom((src, dest) => src.Country ?? dest.Country))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom((src, dest) => src.Nationality ?? dest.Nationality))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom((src, dest) => src.MaritalStatus ?? dest.MaritalStatus))
                .ForMember(dest => dest.AccessToLaptop, opt => opt.MapFrom((src, dest) => src.AccessToLaptop ?? dest.AccessToLaptop))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom((src, dest) => src.Gender ?? dest.Gender))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom((src, dest) => src.Religion ?? dest.Religion))
                .ForMember(dest => dest.SocialMedia, opt => opt.MapFrom((src, dest) => src.SocialMedia ?? dest.SocialMedia))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom((src, dest) => !src.DateOfBirth.HasValue ? dest.DateOfBirth : src.DateOfBirth))
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ApprovedApplicant, BeneficiaryDTO>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.User.Category))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status))
               .ForMember(dest => dest.Programmes, opt => opt.MapFrom(src => src.ApprovedApplicantProgrammes))
               .ForMember(dest => dest.Picture, opt => opt.MapFrom(src => src.User.Picture));
                
            CreateMap<ApprovedApplicantProgramme, BeneficiaryProgrammeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Programme.Id))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Programme.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Programme.EndDate))
                .ForMember(dest => dest.Sponsor, opt => opt.MapFrom(src => src.Programme.Sponsor))
                .ForMember(dest => dest.LearningTrack, opt => opt.MapFrom(src => src.learningTrack.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Programme.Status))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Programme.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Programme.Category))
                .ForMember(dest => dest.TimeLine, opt => opt.MapFrom(src => DateTime.Now < src.Programme.EndDate ? (int)((DateTime.Now - src.Programme.StartDate).TotalSeconds * 100 / 
                   (src.Programme.EndDate - src.Programme.StartDate).TotalSeconds) : 100))
                .ForMember(dest => dest.ProgrammeManagers, opt => opt.MapFrom(src => src.Programme.ProgrammeManagers));

        }
    }
}

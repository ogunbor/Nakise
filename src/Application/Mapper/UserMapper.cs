using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;
using static Shared.DataTransferObjects.UpdateAdmininistratorDTO;

namespace Application.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, CreateUserResponse>();

            CreateMap<CreateUserInputDTO, User>().AfterMap((src, dest) =>
            {
                dest.Email = src.Email.Trim().ToLower();
            });

            CreateMap<User, UserLoginResponse>().ReverseMap();

            CreateMap<User, GetAllUserDto>()
                .ForMember(dest => dest.ProgrammeCount, opt => opt.MapFrom(src => src.ProgrammeManagers.Count));

            CreateMap<ProgrammeManager, GetProgrammesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProgrammeId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Programme.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Programme.Category));

            CreateMap<User, GetSetPasswordDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<User, FacilitatorDTO>();

            CreateMap<User, GetProgrammesByUserDto>()
                .ForMember(dest => dest.Programmes, opt => opt.MapFrom(src => src.ProgrammeManagers));

            CreateMap<User, CreatedBy>();
            CreateMap<User, GetAllAdminUserDto>();
            CreateMap<User, UpdateAdminResponse>();

            CreateMap<UpdateAdmininistratorDTO, User>().ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
            CreateMap<User, GetAdminUserDto>();
            CreateMap<UserActivity, GetUserActivityDto>();
            CreateMap<User, GetUserProfileDto>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.UserInformation.Gender))
                .ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.UserInformation.Industry))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.UserInformation.Languages))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserInformation.Bio))
                .ForMember(dest => dest.ExperienceDetail, opt => opt.MapFrom(src => src.UserInformation.ExperienceDetail))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.UserInformation.Address))
                .ForMember(dest => dest.LinkedInUrl, opt => opt.MapFrom(src => src.UserInformation.LinkedInUrl))
                .ForMember(dest => dest.PortfolioUrl, opt => opt.MapFrom(src => src.UserInformation.PortfolioUrl))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.UserInformation.Skills))
                .ForMember(dest => dest.AccessToPower, opt => opt.MapFrom(src => src.UserInformation.AccessToPower))
                .ForMember(dest => dest.AccessToLaptop, opt => opt.MapFrom(src => src.UserInformation.AccessToLaptop))
                .ForMember(dest => dest.SalaryExpectation, opt => opt.MapFrom(src => src.UserInformation.SalaryExpectation))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.UserInformation.DateOfBirth))
                .ForMember(dest => dest.Ethnicity, opt => opt.MapFrom(src => src.UserInformation.Ethnicity))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.UserInformation.MaritalStatus))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.UserInformation.ContractType))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.UserInformation.Nationality))
                .ForMember(dest => dest.NextOfKinFirstName, opt => opt.MapFrom(src => src.UserInformation.NextOfKinFirstName))
                .ForMember(dest => dest.NextOfKinLastName, opt => opt.MapFrom(src => src.UserInformation.NextOfKinLastName))
                .ForMember(dest => dest.NextOfKinEmail, opt => opt.MapFrom(src => src.UserInformation.NextOfKinEmail))
                .ForMember(dest => dest.NextOfKinPhoneNumber, opt => opt.MapFrom(src => src.UserInformation.NextOfKinPhoneNumber))
                .ForMember(dest => dest.NextOfKinAddress, opt => opt.MapFrom(src => src.UserInformation.NextOfKinAddress))
                .ForMember(dest => dest.NextOfKinRelationship, opt => opt.MapFrom(src => src.UserInformation.NextOfKinRelationship));
            CreateMap<UserDocument, GetUserDocumentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.DocumentUrl));
            CreateMap<UserDocument, UserDocumentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DocumentName))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.DocumentUrl));
        }
    }
}

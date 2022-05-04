using Application.Enums;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domian.Entities.ActivityForms;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class ActivityFormMapper : Profile
    {
        public ActivityFormMapper()
        {
            CreateMap<CreateFormInputDto, ActivityForm>()
                .ForMember(dest => dest.FormFields, opt => opt.Ignore());

            CreateMap<FormFieldInputDto, FormField>()
                .ForMember(dest => dest.Options, opt => opt.Ignore())
                .ForMember(dest => dest.FormFieldValue, opt => opt.Ignore());

            CreateMap<FieldOptionInputDto, FieldOption>();
            CreateMap<Activity, GetActivityDto>();
            CreateMap<CallForApplication, GetActivityDto>();
            CreateMap<Assessment, GetActivityDto>();
            CreateMap<Form, GetActivityDto>();
            CreateMap<Survey, GetActivityDto>();
            CreateMap<Training, GetActivityDto>();
            CreateMap<Event, GetActivityDto>();
            CreateMap<CallForApplication, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<Assessment, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<Form, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<Survey, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<Training, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<Event, ActivityFormDto>()
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom((src, dest) => dest.OrganizationId = src.Programme.OrganizationId));
            CreateMap<ActivityFormDto, GetActivityDto>();
            CreateMap<ActivityForm, GetDefaultFormDto>();
            CreateMap<FormField, FileDto>()
                .AfterMap((src, dest) => dest.SingleFileSizeLimit = src.SingleFileSizeLimit)
                .AfterMap((src, dest) => dest.NumberLimit = src.FileNumberLimit)
                .AfterMap((src, dest) => dest.Type = src.FileType);
            CreateMap<FormField, GetFormFieldDto>()
                .AfterMap((src, dest, context) =>
                {
                    if (src.FormType == EFormType.Custom.ToString() && 
                        src.FileNumberLimit is null &&
                        src.FileType is null &&
                        src.SingleFileSizeLimit is null)
                    {
                        dest.File = null;
                        return;
                    }

                    if (src.FormType == EFormType.Custom.ToString() &&
                        (src.FileNumberLimit is not null ||
                        src.FileType is not null ||
                        src.SingleFileSizeLimit is not null))
                    {
                        dest.File = context.Mapper.Map<FormField, FileDto>(src);
                        return;
                    }

                    if (src.FormType == EFormType.Default.ToString())
                    {
                        dest.File = null;
                        return;
                    }
                });
            CreateMap<FormFieldValue, GetFormFieldValueDto>();
            CreateMap<FieldOption, GetFieldOptionDto>();
            CreateMap<CreateFieldValueDto, FormFieldValue>();
            CreateMap<ApplicantDetail, GetApplicantFormStatusDto>();
        }
    }
}

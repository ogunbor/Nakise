
using Application.DTOs;
using AutoMapper;
using Domain.Entities.Actvities;

namespace Application.Mapper
{
    public class FormMapper : Profile
    {
        public FormMapper()
        {
            CreateMap<CreateFormDTO, Form>();

            CreateMap<Form, GetFormDTO>().ReverseMap();
            CreateMap<UpdateFormDTO, Form>();
        }
    }
}
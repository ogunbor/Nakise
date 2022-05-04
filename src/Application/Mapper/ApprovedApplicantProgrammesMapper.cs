using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapper
{
    internal class ApprovedApplicantProgrammesMapper : Profile
    {
        public ApprovedApplicantProgrammesMapper()
        {
            CreateMap<ApprovedApplicantProgramme, ProgrammeDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Programme.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Programme.Title));
        }
    }
}

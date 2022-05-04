using AutoMapper;
using Domain.Entities;
using Shared.DataTransferObjects;

namespace Application.Mapper
{
    public class OrganizationMapper: Profile
    {
        public OrganizationMapper()
        {
            CreateMap<Organization, OrganizationResponse>();
            CreateMap<OrganizationDTO, Organization>();
        }
    }
}

using Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace Application.Contracts.V1
{
    public interface IOrganizationService: IAutoDependencyService
    {
        Task<PagedResponse<IEnumerable<OrganizationResponse>>> GetOrganizations(ResourceParameter parameter, string name, IUrlHelper urlHelper);
        Task<SuccessResponse<OrganizationResponse>> GetOrganizationById(Guid id);
        Task<SuccessResponse<OrganizationResponse>> CreateOrganization(OrganizationDTO model);
        Task<SuccessResponse<OrganizationResponse>> UpdateOrganization(Guid id, OrganizationDTO model);
        Task<SuccessResponse<OrganizationResponse>> DeleteOrganization(Guid id);
    }
}
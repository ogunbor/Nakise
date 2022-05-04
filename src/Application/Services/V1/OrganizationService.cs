using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DataTransferObjects;
using System.Net;

namespace Application.Services.V1.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;

        public OrganizationService(
            IMapper mapper,
            IRepositoryManager repository, 
            IWebHelper webHelper)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
        }


        //get list of organizations
        public async Task<PagedResponse<IEnumerable<OrganizationResponse>>> GetOrganizations(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var organizationQuery = _repository.Organization.QueryAll();
            organizationQuery = !string.IsNullOrWhiteSpace(parameter.Search) ? _repository.Organization.Get(x => x.Name.Contains(parameter.Search)) : organizationQuery;
            var organizationResponses = organizationQuery.ProjectTo<OrganizationResponse>(_mapper.ConfigurationProvider);
            var organizations = await PagedList<OrganizationResponse>.Create(organizationResponses, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<OrganizationResponse>.CreateResourcePageUrl(parameter, name, organizations, urlHelper);

            var response = new PagedResponse<IEnumerable<OrganizationResponse>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = organizations,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
            return response;
        }

        //get organization by Id
        public async Task<SuccessResponse<OrganizationResponse>> GetOrganizationById(Guid id)
        {
            var organization = await _repository.Organization.GetByIdAsync(id);
            if (organization == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.OrganizationDoesNotExist);

            var organizationResponse = _mapper.Map<OrganizationResponse>(organization);
            return new SuccessResponse<OrganizationResponse>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = organizationResponse
            };
        }

        //create organization
        public async Task<SuccessResponse<OrganizationResponse>> CreateOrganization(OrganizationDTO model)
        {
            var organizationExists = await _repository.Organization.ExistsAsync(x => x.Name == model.Name.Trim());
            if (organizationExists)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.DuplicateOrganization);

            var organization = _mapper.Map<Organization>(model);
            organization.OrganizationRef = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            await _repository.Organization.AddAsync(organization);

            UserActivity userActivity = AuditLog.UserActivity(organization, _webHelper.User().UserId, nameof(organization), $"Created a Organization - {organization.Name}", organization.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var organizationResponse = _mapper.Map<OrganizationResponse>(organization);
            return new SuccessResponse<OrganizationResponse>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = organizationResponse
            };
        }

        //update organization 
        public async Task<SuccessResponse<OrganizationResponse>> UpdateOrganization(Guid id, OrganizationDTO model)
        {
            var organization = await _repository.Organization.GetByIdAsync(id);
            if (organization == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.OrganizationDoesNotExist);
            if (organization.Name.ToLower() != model.Name?.ToLower())
            {
                var organizationNameExists = await _repository.Organization.ExistsAsync(x => x.Name == model.Name);
                if (organizationNameExists)
                    throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.DuplicateOrganization);
            }

            organization = _mapper.Map(model, organization);
            organization.UpdatedAt = DateTime.UtcNow;

            UserActivity userActivity = AuditLog.UserActivity(organization, _webHelper.User().UserId, nameof(organization), $"Updated an Organization - {organization.Name}", organization.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var organizationResponse = _mapper.Map<OrganizationResponse>(organization);
            return new SuccessResponse<OrganizationResponse>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = organizationResponse
            };
        }

        //delete organization
        public async Task<SuccessResponse<OrganizationResponse>> DeleteOrganization(Guid id)
        {
            var organization = await _repository.Organization.GetByIdAsync(id);
            if (organization == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.OrganizationDoesNotExist);
            
            organization.IsActive = false;
            organization.UpdatedAt = DateTime.UtcNow;

            UserActivity userActivity = AuditLog.UserActivity(organization, _webHelper.User().UserId, nameof(organization), $"Deleted an Organization - {organization.Name}", organization.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            var organizationResponse = _mapper.Map<OrganizationResponse>(organization);
            return new SuccessResponse<OrganizationResponse>
            {
                Message = ResponseMessages.DeleteSuccessResponse,
                Data = organizationResponse
            };
        }
    }
}
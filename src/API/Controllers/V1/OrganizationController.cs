using Application.Contracts.V1;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace API.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organization")]
    public class OrganizationController : ControllerBase
    {
        private readonly IServiceManager _service;
        public OrganizationController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to get all organizations
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetOrganizations))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<OrganizationResponse>>), 200)]
        public async Task<IActionResult> GetOrganizations([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.OrganizationService.GetOrganizations(parameter, nameof(GetOrganizations), Url);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get organization by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<OrganizationResponse>), 200)]
        public async Task<IActionResult> GetOrganizationById(Guid id)
        {
            var response = await _service.OrganizationService.GetOrganizationById(id);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to create organization
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(SuccessResponse<OrganizationResponse>), 200)]
        public async Task<IActionResult> CreateOrganization(OrganizationDTO model)
        {
            var response = await _service.OrganizationService.CreateOrganization(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to update organization
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<OrganizationResponse>), 200)]
        public async Task<IActionResult> UpdateOrganization(Guid id, OrganizationDTO model)
        {
            var response = await _service.OrganizationService.UpdateOrganization(id, model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete organization
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<OrganizationResponse>), 200)]
        public async Task<IActionResult> DeleteOrganization(Guid id)
        {
            var response = await _service.OrganizationService.DeleteOrganization(id);

            return Ok(response);
        }
    }
}
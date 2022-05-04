using Application.Contracts.V1;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;
using static Shared.DataTransferObjects.UpdateAdmininistratorDTO;

namespace API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UsersController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Endpoint to register a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(SuccessResponse<CreateUserResponse>), 200)]
        public async Task<IActionResult> RegisterUser(CreateUserInputDTO model)
        {
            var response = await _service.UserService.InviteUser(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _service.UserService.GetUserById(id);

            return Ok(response);
        }            

        /// <summary>
        /// Endpoints to get the user's statistics
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager, Admin")]
        [HttpGet("stats")]
        [ProducesResponseType(typeof(SuccessResponse<GetUserStatsDto>), 200)]
        public async Task<IActionResult> GetUsersStat()
        {
            var response = await _service.UserService.GetUsersStat();
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all users
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager, Admin")]
        [HttpGet(Name = nameof(GetAllUsers))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllUserDto>>), 200)]
        public async Task<IActionResult> GetAllUsers([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.UserService.GetAllUsers(parameter, nameof(GetAllUsers), Url);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all program by a user
        /// </summary> 
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "ProgramManager, Admin")]
        [HttpGet("{id}/programmes")]
        [ProducesResponseType(typeof(PagedResponse<GetProgrammesByUserDto>), 200)]
        public async Task<IActionResult> GetProgrammesByUser(Guid id)
        {
            var response = await _service.UserService.GetAllProgrammesByUser(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all admin users
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("admins", Name = nameof(GetAllAdminUser))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAllAdminUserDto>>), 200)]
        public async Task<IActionResult> GetAllAdminUser([FromQuery] ResourceParameter parameter)
        {
            var response = await _service.UserService.GetAllAdminUsers(parameter, nameof(GetAllAdminUser), Url);
            
            return Ok(response);
        }
        
        /// Endpoint to get admin Stat
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-stats")]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetAdministratorStatsDTO>>), 200)]
        public async Task<IActionResult> GetAdministratorStat()
        {
            var response = await _service.UserService.GetAdmininistratorStat();

            return Ok(response);
        }
        /// <summary>
        /// Endpoint to edit admin info
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("edit-admin/{id}")]
        [ProducesResponseType(typeof(SuccessResponse<UpdateAdminResponse>), 200)]
        public async Task<IActionResult> UpdateAdministrator(Guid id, UpdateAdmininistratorDTO adminDTO)
        {
            var response = await _service.UserService.EditAdmin(id,adminDTO);

            return Ok(response);
        }

        /// <summary>
        /// Endpoints to activate an admin user account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/activate")]
        [ProducesResponseType(typeof(SuccessResponse<GetAdminUserDto>), 200)]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            var response = await _service.UserService.ActivateUser(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to disable an admin user account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/suspend")]
        [ProducesResponseType(typeof(SuccessResponse<GetAdminUserDto>), 200)]
        public async Task<IActionResult> SuspendUser(Guid id)
        {
            var response = await _service.UserService.SuspendUser(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoints to get all user's activity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("{id}/activities", Name = nameof(GetAllUsersActivity))]
        [ProducesResponseType(typeof(PagedResponse<IEnumerable<GetUserActivityDto>>), 200)]
        public async Task<IActionResult> GetAllUsersActivity(Guid id, [FromQuery] ResourceParameter parameter)
        {
            var response = await _service.UserService.GetUserActivitiesAsync(id, parameter, nameof(GetAllUsersActivity), Url);
            return Ok(response);
        }

        [Obsolete]
        [AllowAnonymous]
        [HttpPost("upload-user-image")]
        public async Task<IActionResult> UploadUserImageTest(IFormFile file)
        {
            var response = await _service.UserService.UploadFile(file);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to upload user documents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/documents")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<UserDocumentDto>>), 200)]
        public async Task<IActionResult> UploadDocuments(Guid id, ICollection<IFormFile> documents)
        {
            var response = await _service.UserService.UploadDocuments(id, documents);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to get all user's documents
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}/documents", Name = nameof(GetUserDocuments))]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<GetUserDocumentDto>>), 200)]
        public async Task<IActionResult> GetUserDocuments(Guid id)
        {
            var response = await _service.UserService.GetUserDocuments(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint to delete user's document
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("documents/{id}", Name = nameof(DeleteDocument))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            await _service.UserService.DeleteDocument(id);
            return NoContent();
        }
    }
}

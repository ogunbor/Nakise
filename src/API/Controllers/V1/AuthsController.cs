using Application.Contracts.V1;
using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataTransferObjects;

namespace API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auths")]
    public class AuthsController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AuthsController(IServiceManager service)
        {
            _service = service;
        }

        // <summary>
        /// Endpoint to login a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(SuccessResponse<UserLoginResponse>), 200)]
        public async Task<IActionResult> LoginUser(UserLoginDTO model)
        {
            var response = await _service.AuthenticationService.Login(model);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to generate a new access and refresh token
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), 200)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO mdoel)
        {
            var response = await _service.AuthenticationService.GetRefreshToken(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to initializes password reset
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(SuccessResponse<object>), 200)]
        public async Task<IActionResult> ForgotPassword(ResetPasswordDTO mdoel)
        {
            var response = await _service.AuthenticationService.ResetPassword(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to verify token
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verify-token")]
        [ProducesResponseType(typeof(SuccessResponse<GetConifrmedTokenUserDto>), 200)]
        public async Task<IActionResult> VerifyToken(VerifyTokenDTO mdoel)
        {
            var response = await _service.AuthenticationService.ConfirmToken(mdoel);

            return Ok(response);
        }

        /// <summary>
        /// Endpoint to set password
        /// </summary>
        /// <param name="mdoel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("set-password")]
        [ProducesResponseType(typeof(SuccessResponse<GetSetPasswordDto>), 200)]
        public async Task<IActionResult> SetPassword([FromForm] SetPasswordDTO mdoel)
        {
            var response = await _service.AuthenticationService.SetPassword(mdoel);

            return Ok(response);
        }
    }
}

using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Contracts;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.DataTransferObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Application.Services.V1
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailManager _mailerService;
        private readonly IRepositoryManager _repository;
        private readonly IAwsS3Client _awsS3Client;
        private readonly IWebHelper _webHelper;

        public AuthenticationService(IMapper mapper, 
            IConfiguration configuration, 
            UserManager<User> userManager, 
            RoleManager<Role> roleManager, 
            IEmailManager mailerService, 
            IRepositoryManager repository, 
            IAwsS3Client awsS3Client, 
            IWebHelper webHelper)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailerService = mailerService;
            _repository = repository;
            _awsS3Client = awsS3Client;
            _webHelper = webHelper;
        }

        public async Task<SuccessResponse<UserLoginResponse>> Login(UserLoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            var approvedApplicant = _repository.ApprovedApplicant.Get(x => x.UserId == user.Id).FirstOrDefault();

            if (user.Status.Equals(EUserStatus.Disabled.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UserIsDisabled);

            if (!user.IsActive || !user.EmailConfirmed || user.Status != EUserStatus.Active.ToString() || !user.Verified)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            var isUserValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isUserValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.WrongEmailOrPassword);

            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            UserActivity userActivity = AuditLog.UserActivity(user, user.Id, nameof(user), $"Logged in", user.Id);
            var roles = await _userManager.GetRolesAsync(user);
            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var userViewModel = _mapper.Map<UserLoginResponse>(user);

            var tokenResponse = Authenticate(user, approvedApplicant, roles);
            userViewModel.AccessToken = tokenResponse.AccessToken;
            userViewModel.ExpiresIn = tokenResponse.ExpiresIn;
            userViewModel.RefreshToken = GenerateRefreshToken(user.Id);

            return new SuccessResponse<UserLoginResponse>
            {
                Message = ResponseMessages.LoginSuccessResponse,
                Data = userViewModel
            };
        }

        public async Task<SuccessResponse<RefreshTokenResponse>> GetRefreshToken(RefreshTokenDTO model)
        {
            var userId = GetUserIdFromAccessToken(model.AccessToken);

            var user = await _repository.User.GetByIdAsync(userId);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var approvedApplicant = _repository.ApprovedApplicant.Get(x => x.UserId == user.Id).FirstOrDefault();

            var isRefreshTokenValid = ValidateRefreshToken(model.RefreshToken);
            if (!isRefreshTokenValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            var roles = await _userManager.GetRolesAsync(user);
            var tokenResponse = Authenticate(user, approvedApplicant, roles);

            var newRefreshToken = GenerateRefreshToken(user.Id);

            var tokenViewModel = new RefreshTokenResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn
            };

            return new SuccessResponse<RefreshTokenResponse>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = tokenViewModel
            };
        }

        public async Task<SuccessResponse<GetSetPasswordDto>> SetPassword(SetPasswordDTO model)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var token = await _repository.Token.FirstOrDefault(x => x.Value == model.Token);
            if (token == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidExpiredToken);

            var isValid = CustomToken.IsTokenValid(token);
            if (!isValid)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            var user = await _repository.User.GetByIdAsync(token.UserId);
            if (user.Email != model.Email)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidToken);

            string profilePictureUrl = string.Empty;
            if (model.ProfilePicture != null)
                profilePictureUrl = await _awsS3Client.UploadFileAsync(model.ProfilePicture);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Password = _userManager.PasswordHasher.HashPassword(user, model.Password);
            user.PasswordHash = user.Password;
            user.UpdatedAt = DateTime.UtcNow;
            user.Picture = profilePictureUrl;

            if (token.TokenType == ETokenType.InviteUser.ToString())
            {
                user.IsActive = true;
                user.Status = EUserStatus.Active.ToString();
                user.EmailConfirmed = true;
                user.Verified = true;
            }
            _repository.User.Update(user);

            UserActivity userActivity = AuditLog.UserActivity(user, user.Id, nameof(user), $"Signed up", user.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            _repository.Token.Remove(token);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetSetPasswordDto>
            {
                Message = ResponseMessages.PasswordSetSuccessfully,
                Data = _mapper.Map<GetSetPasswordDto>(user)
            };
        }

        public async Task<SuccessResponse<object>> ResetPassword(ResetPasswordDTO model)
        {
            var user = await _repository.User.FirstOrDefault(x => x.Email == model.Email);
            if (user == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var token = CustomToken.GenerateRandomString(128);
            var tokenEntity = new Token
            {
                UserId = user.Id,
                TokenType = ETokenType.ResetPassword.ToString(),
                Value = token
            };
            await _repository.Token.AddAsync(tokenEntity);

            UserActivity userActivity = AuditLog.UserActivity(user, user.Id, nameof(user), $"Requested for password reset", user.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();

            string emailLink = $"{_configuration["CLIENT_URL"]}/reset-password?token={token}";
            var message = _mailerService.GetResetPasswordEmailTemplate(emailLink, user.Email);
            string subject = "Reset Password";

            _mailerService.SendSingleEmail(user.Email, message, subject);

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.PasswordResetSuccessfully,
            };
        }

        public async Task<SuccessResponse<GetConifrmedTokenUserDto>> ConfirmToken(VerifyTokenDTO model)
        {
            var token = await _repository.Token.FirstOrDefault(x => x.Value == model.Token);
            if (token == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.InvalidExpiredToken);

            if (DateTime.Now >= token.ExpiresAt)
            {
                _repository.Token.Remove(token);
                await _repository.SaveChangesAsync();

                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.TokenExpired);
            }

            var user = await _repository.User.FirstOrDefault(x => x.Id == token.UserId);
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidToken);

            if (token.TokenType == ETokenType.InviteUser.ToString() &&
                (token.ExpiresAt - DateTime.Now) <= TimeSpan.FromMinutes(30))
            {
                token.ExpiresAt = token.ExpiresAt.AddMinutes(30);
                _repository.Token.Update(token);
                await _repository.SaveChangesAsync();
            }

            return new SuccessResponse<GetConifrmedTokenUserDto>
            {
                Message = ResponseMessages.TokenConfirmedSuccessfully,
                Data = new GetConifrmedTokenUserDto
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
        }

        #region Private methods to manage Authentication 
        private TokenReturnHelper Authenticate(User user, ApprovedApplicant approvedApplicant, IList<string> roles)
        {
            var roleClaims = new List<Claim>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypeHelper.Email, user.Email),
                new Claim(ClaimTypeHelper.UserId, user.Id.ToString()),
                new Claim(ClaimTypeHelper.FullName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypeHelper.OrganizationId, user.OrganizationId.ToString()),
                new Claim(ClaimTypeHelper.ApprovedApplicantId, approvedApplicant?.Id.ToString() ?? ""),
            };

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            claims.AddRange(roleClaims);

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;
            var tokenExpireIn = string.IsNullOrEmpty(jwtSettings.GetSection("TokenLifespan").Value) ? int.Parse(jwtSettings.GetSection("TokenLifespan").Value) : 7;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtUserSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(tokenExpireIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return new TokenReturnHelper
            {
                ExpiresIn = tokenDescriptor.Expires,
                AccessToken = jwt
            };
        }
        private string GenerateRefreshToken(Guid userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtUserSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypeHelper.UserId, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }
        private bool ValidateRefreshToken(string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            var expiryAt = jwtSecurityToken.ValidTo;
            if (DateTime.UtcNow > expiryAt)
                return false;
            return true;
        }
        private Guid GetUserIdFromAccessToken(string accessToken)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidToken);
            }

            var userId = principal.FindFirst(ClaimTypeHelper.UserId)?.Value;

            if (userId == null)
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.MissingClaim} {ClaimTypeHelper.UserId}");

            return Guid.Parse(userId);
        }
        #endregion
    }
}

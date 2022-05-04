using System;

namespace Shared.DataTransferObjects
{
    public class CreateUserResponse 
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
    public class UserLoginResponse
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }

    public class UserByIdResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool Verified { get; set; }
    }
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
    }
    public class UpdateAdminResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class TokenReturnHelper
    {
        public string AccessToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
    }
}

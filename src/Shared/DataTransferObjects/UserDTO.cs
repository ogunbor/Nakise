using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects
{
    public class CreateUserInputDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Category { get; set; }
    }
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
    }
    public class VerifyTokenDTO
    {
        public string Token { get; set; }
    }
    public class SetPasswordDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }

    public class GetUserStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Active { get; set; }
        public int Disabled { get; set; }
    }

    public class GetAllUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public int ProgrammeCount { get; set; }
    }

    public class GetAllAdminUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Permission { get; set; }
    }

      public class GetAdminUserDto : GetAllAdminUserDto
    {
       
    }

    public class GetProgrammesDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
    }

    public class GetProgrammesByUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<GetProgrammesDto> Programmes { get; set; }
    }

    public class GetConifrmedTokenUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GetSetPasswordDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
    }

    public class CreatedBy : ManagerDTO
    {

    }
    public class GetAdministratorStatsDTO
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int PendingCount { get; set; }
        public int DisabledCount { get; set; }
    }

    public class GetUserActivityDto
    {
        public Guid Id { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetUserDocumentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class UpdateAdmininistratorDTO
    {
        private string _firstName;
        private string _lastName;
        private string _email;

        public string FirstName
        {
            get => _firstName;
            set => _firstName = ReplaceEmptyWithNull(value);
        }
        public string LastName 
        {
            get => _lastName;
            set => _lastName = ReplaceEmptyWithNull(value);
        }
        public string Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        // helpers

        private static string ReplaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public record GetUserProfileDto
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Location { get; set; }
            public string PhoneNumber { get; set; }
            public string Picture { get; set; }
            public string Status { get; set; }
            public string Gender { get; set; }
            public string Bio { get; set; }
            public string ExperienceDetail { get; set; }
            public string Language { get; set; }
            public string Skills { get; set; }
            public string Address { get; set; }
            public string Nationality { get; set; }
            public string LinkedInUrl { get; set; }
            public string PortfolioUrl { get; set; }
            public bool? AccessToPower { get; set; }
            public bool? AccessToLaptop { get; set; }
            public decimal? SalaryExpectation { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Ethnicity { get; set; }
            public string MaritalStatus { get; set; }
            public string Industry { get; set; }
            public string ContractType { get; set; }
            public string Hobby { get; set; }
            public string EmploymentStatus { get; set; }
            public string NextOfKinFirstName { get; set; }
            public string NextOfKinLastName { get; set; }
            public string NextOfKinEmail { get; set; }
            public string NextOfKinPhoneNumber { get; set; }
            public string NextOfKinAddress { get; set; }
            public string NextOfKinRelationship { get; set; }
        }

        public class UserDocumentDto : GetUserDocumentDto {}
    }
}

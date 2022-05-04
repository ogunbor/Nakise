using System;
using System.Security.Claims;

namespace Application.Helpers
{
    public class ClaimTypeHelper
    {
        public static string UserId { get; set; } = "UserId";
        public static string Email { get; set; } = "Email";
        public static string FullName { get; set; } = "FullName";
        public static string OrganizationId { get; set; } = "OrganizationId";
        public static string ApprovedApplicantId { get; set; } = "ApprovedApplicantId";
    }
}

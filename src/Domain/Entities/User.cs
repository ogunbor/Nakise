using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User: IdentityUser<Guid>, IAuditableEntity
    {
        public User()
        {
            ProgrammeManagers = new HashSet<ProgrammeManager>();
            Documents = new HashSet<UserDocument>();
        }
        public Guid OrganizationId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public bool Verified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public string Picture { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProgrammeManager> ProgrammeManagers { get; set; }
        public ICollection<LearningTrackFacilitator> LearningTrackFacilitators { get; set; }
        public Guid? UserInformationId { get; set; }
        public UserInformation UserInformation { get; set; }
        public ICollection<UserDocument> Documents { get; set; }
    }
}

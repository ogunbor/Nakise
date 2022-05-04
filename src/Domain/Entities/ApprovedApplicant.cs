using Domain.Common;

namespace Domain.Entities
{
    public class ApprovedApplicant : AuditableEntity
    {
        public ApprovedApplicant()
        {
            ApprovedApplicantProgrammes = new HashSet<ApprovedApplicantProgramme>();
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string AccessToLaptop { get; set; }
        public string SocialMedia { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string ActivityType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public ICollection<ApprovedApplicantProgramme> ApprovedApplicantProgrammes { get; set; }
    }
}

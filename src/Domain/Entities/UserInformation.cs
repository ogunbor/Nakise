using Domain.Common;

namespace Domain.Entities
{
    public class UserInformation : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public string ExperienceDetail { get; set; }
        public string Languages { get; set; }
        public string LinkedInUrl { get; set; }
        public string PortfolioUrl { get; set; }
        public string Skills { get; set; }
        public bool? AccessToPower { get; set; }
        public bool? AccessToLaptop { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Ethnicity { get; set; }
        public string MaritalStatus { get; set; }
        public string Industry { get; set; }
        public string Nationality { get; set; }
        public string ContractType { get; set; }
        public string NextOfKinFirstName { get; set; }
        public string NextOfKinLastName { get; set; }
        public string NextOfKinEmail { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinRelationship { get; set; }
        public User User { get; set; }
    }
}

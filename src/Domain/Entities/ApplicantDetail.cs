using Domain.Entities.Activities;
using Domian.Entities.ActivityForms;

namespace Domain.Entities
{
    public class ApplicantDetail
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string AccessToLaptop { get; set; }
        public string SocialMedia { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? LearningTrackId { get; set; }
        public Guid? StageId { get; set; }
        public string ActivityType { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Status { get; set; }
        public Guid FormId { get; set; }
        public ActivityForm Form { get; set; }
        public Stage Stage { get; set; }
        public LearningTrack LearningTrack { get; set; }
    }
}

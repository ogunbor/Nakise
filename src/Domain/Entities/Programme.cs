using Domain.Common;
using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Programme : AuditableEntity
    {
        public Programme()
        {
            ProgrammeManagers = new HashSet<ProgrammeManager>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DeliveryMethod { get; set; }
        public bool HasLearningTrack { get; set; }
        public string Sponsor { get; set; }
        public string GenderOption { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public string Status { get; set; } = EProgrammeStatus.NotStarted.ToString();
        public Guid CreatedById { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public User CreatedBy { get; set; }
        public ICollection<ProgrammeManager> ProgrammeManagers { get; set; }
        public ICollection<LearningTrack> LearningTracks { get; set; }
    }
}

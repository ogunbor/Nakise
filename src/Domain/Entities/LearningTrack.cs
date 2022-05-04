using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class LearningTrack : AuditableEntity
    {
        public LearningTrack()
        {
            LearningTrackFacilitators = new HashSet<LearningTrackFacilitator>();
        }

        public Guid Id { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public Programme Programme { get; set; }
        public ICollection<LearningTrackFacilitator> LearningTrackFacilitators { get; set; }
    }
}

using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Activities
{
    public class Training : AuditableEntity
    {
        public Training()
        {
            TrainingLearningTracks = new HashSet<TrainingLearningTrack>();
        }

        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Activity Activity { get; set; }
        public Programme Programme { get; set; }
        public ICollection<TrainingLearningTrack> TrainingLearningTracks { get; set; }
    }
}

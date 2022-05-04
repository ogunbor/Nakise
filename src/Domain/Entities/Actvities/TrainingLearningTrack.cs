using System;

namespace Domain.Entities.Activities
{
    public class TrainingLearningTrack
    {
        public Guid LearningTrackId { get; set; }
        public Guid TrainingId { get; set; }
        public LearningTrack LearningTrack { get; set; }
        public Training Training { get; set; }
    }
}

using System;

namespace Domain.Entities.Activities
{
    public class SurveyLearningTrack
    {
        public Guid LearningTrackId { get; set; }
        public Guid SurveyId { get; set; }
        public LearningTrack LearningTrack { get; set; }
        public Survey Survey { get; set; }
    }
}

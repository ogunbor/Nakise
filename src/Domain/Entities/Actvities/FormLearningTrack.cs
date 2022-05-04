using Domain.Entities.Actvities;
using System;

namespace Domain.Entities.Activities
{
    public class FormLearningTrack
    {
        public Guid FormId { get; set; }
        public Guid LearningTrackId { get; set; }
        public Form Form { get; set; }
        public LearningTrack LearningTrack { get; set; }
    }
}

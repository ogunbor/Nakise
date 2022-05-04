using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects
{
    public class CreateTrainingInputDto
    {
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Guid> LearningTrackIds { get; set; }
    }

    public class UpdateTrainingInputDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Guid> LearningTrackIds { get; set; }
    }

    public class GetTrainingDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TrainingProgrammeDto Programme { get; set; }
        public ICollection<TrainingLearningTrackDto> LearningTracks { get; set; }
    }

    public class TrainingLearningTrackDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class TrainingProgrammeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}

using System;

namespace Shared.DataTransferObjects
{
    public class GetActivityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class GetProgrammeActivitiesDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ActivityDTO Activity { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ActivityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}

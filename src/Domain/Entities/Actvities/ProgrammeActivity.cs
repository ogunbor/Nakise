namespace Domain.Entities.Activities
{
    public class ProgrammeActivity
    {
        public Guid ActivityId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}

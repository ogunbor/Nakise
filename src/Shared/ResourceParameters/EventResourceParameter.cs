namespace Shared.ResourceParameters
{
    public class EventResourceParameter : Parameters
    {
        public string EventType { get; set; }
        public bool Upcoming { get; set; }
        public bool Ongoing { get; set; }
        public bool Completed { get; set; }
        public bool Cancelled { get; set; }
        public bool Past { get; set; }
    }
}

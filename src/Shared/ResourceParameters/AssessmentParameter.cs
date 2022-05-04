namespace Shared.ResourceParameters
{
    public record AssessmentParameters
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string Search { get; set; }
    }
}

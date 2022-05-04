namespace Shared.DataTransferObjects
{
    public record ApplicantDTO : CallForApplicationParticipantDTO
    {
        public string Gender { get; set; }
        public string Country { get; set; }
        public string StageName { get; set; } 
        public Guid? StageId { get; init; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
    }

    public record ApplicantStatusDto
    {
        public string Status { get; set; } 
    }

    public record BulkApplicantStatusDto
    {
        public Guid[] Ids { get; set; }
        public string Status { get; set; } 
    }

    public record BulkApplicantStageDto
    {
        public Guid[] Ids { get; set; }
        public Guid StageId { get; set; } 
    }
}

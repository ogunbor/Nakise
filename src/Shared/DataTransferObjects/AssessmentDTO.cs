using Shared.DataTransferObjects;
using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects
{
    public class CreateAssessmentRequest
    {
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public int Duration { get; set; }
        public int TotalObtainableScore { get; set; }
        public int PassMark { get; set; }
        public string CompletionTitle { get; set; }
        public string CompletionMessage { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public class GetAssessmentDTO
    {
        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int TotalObtainableScore { get; set; }
        public int PassMark { get; set; }
        public int NoOfSubmission { get; set; }
        public string CompletionTitle { get; set; }
        public string CompletionMessage { get; set; }
        public List<AssessmentLearningTrackDTO> LearningTracks { get; set; }
    }

    public class UpdateAssessmentRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CompletionTitle { get; set; }
        public string CompletionMessage { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public class AssessmentLearningTrackDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public record Links
    {
        public string Next { get; set; }
        public string Previous { get; set; }
    }

    public record AssessmentDto
    {
        [JsonPropertyName("links")]
        public Links Links { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
        [JsonPropertyName("results")]
        public List<AssessmentResult> Results { get; set; }
    }

    public record AssessmentResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("start_datetime")]
        public DateTime StartDateTime { get; set; }
        [JsonPropertyName("due_datetime")]
        public DateTime DueDateTime { get; set; }
        [JsonPropertyName("duration")]
        public long Duration { get; set; }
        [JsonPropertyName("pass_mark")]
        public int PassMark { get; set; }
        [JsonPropertyName("max_retake")]
        public int MaxRetake { get; set; }
        [JsonPropertyName("total_obtainable_score")]
        public int TotalObtainableScore { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public record AssessmentSessionDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("reference")]
        public Guid ApprovedApplicantId { get; set; }
        [JsonPropertyName("assessment")]
        public Guid AssessmentId { get; set; }
    }

    public record CreateAssessmentSessionDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("reference")]
        public string ApprovedApplicantId { get; set; }
        [JsonPropertyName("assessment")]
        public string AssessmentId { get; set; }
    }

    public record QuestionAnswerDto
    {
        [JsonPropertyName("session")]
        public string SessionId { get; set; }
        [JsonPropertyName("question")]
        public string QuestionId { get; set; }
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
        [JsonPropertyName("option_selected")]
        public string OptionSelectedId { get; set; }
    }

    public record SessionCompletionDto
    {
        public string SessionId { get; set; }
    }

    public record CreateAssessmentDto
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("start_datetime")]
        public DateTime StartDateTime { get; set; }
        [JsonPropertyName("due_datetime")]
        public DateTime DueDateTime { get; set; }
        [JsonPropertyName("duration")]
        public long Duration { get; set; }
        [JsonPropertyName("pass_mark")]
        public int PassMark { get; set; }
        [JsonPropertyName("max_retake")]
        public int MaxRetake { get; set; }
        [JsonPropertyName("total_obtainable_score")]
        public int TotalObtainableScore { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public record CreateQuestionDto
    {
        public ICollection<QuestionOptionDto> Options { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
    }

    public record QuestionDto
    {
        [JsonPropertyName("options")]
        public ICollection<QuestionOptionDto> Options { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("assessment")]
        public string Assessment { get; set; }
    }

    public record QuestionOptionDto
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("is_answer")]
        public bool IsAnswer { get; set; }
    }

    public record CreateQuestionResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("options")]
        public ICollection<QuestionOptionResponseDto> Options { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreateAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("assessment")]
        public string Assessment { get; set; }
    }

    public record QuestionOptionResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("is_answer")]
        public bool IsAnswer { get; set; }
        [JsonPropertyName("question")]
        public string Question { get; set; }
    }


    public record AnswersPreviewDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("reference")]
        public string ApprovedApplicantId { get; set; }
        [JsonPropertyName("retake_count")]
        public double RetakeCount { get; set; }
        //[JsonPropertyName("score")]
        //public double Score { get; set; }
        //[JsonPropertyName("score_percentage")]
        //public double ScorePercentage { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("grade_status")]
        public string GradeStatus { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("account")]
        public Guid Account { get; set; }
        [JsonPropertyName("assessment")]
        public AssessmentResult Assessment { get; set; }
        [JsonPropertyName("questions")]
        public IList<AnsweredQuestionDto> Questions { get; set; }
    }

    public record AnsweredQuestionDto
    {
        //[JsonPropertyName("score")]
        //public double Score { get; set; }
        [JsonPropertyName("option_selected")]
        public Guid OptionSelected { get; set; }
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
        [JsonPropertyName("question")]
        public SingleQuestionDto Question { get; set; }
    }

    public record SingleQuestionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("options")]
        public IList<GetQuestionOptionDto> Options { get; set; }
    }

    public record GetQuestionOptionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public record Option
    {
        public string Id { get; set; }
        public string Body { get; set; }
    }


    public record Question
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("options")]
        public IEnumerable<Option> Options { get; set; }

    }
    public record QuestionAnswer
    {
        [JsonPropertyName("question")]
        public Question Question { get; set; }
        [JsonPropertyName("answer")]
        public string Answer { get; set; }
        [JsonPropertyName("option_selected")]
        public string OptionSelected { get; set; }
        [JsonPropertyName("score")]
        public string Score { get; set; }
    }
    public record AssessmentResultDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("assessment")]
        public AssessmentResult Assessment { get; set; }
        [JsonPropertyName("questions")]
        public List<QuestionAnswer> Questions { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
        [JsonPropertyName("retake_count")]
        public int RetakeCount { get; set; }
        [JsonPropertyName("score")]
        public string Score { get; set; }
        [JsonPropertyName("score_percentage")]
        public int ScorePercentage { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("grade_status")]
        public string GradeStatus { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("account")]
        public string Account { get; set; }
    }
}
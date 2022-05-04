using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects
{
    public class GetActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class ActivityFormDto : GetActivityDto
    {
        public Guid OrganizationId { get; set; }
    }

    public class GetFormStatusDto
    {
        public bool IsActivated { get; set; }
    }

    public class GetDefaultFormDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public GetActivityDto Activity { get; set; }
        public ICollection<GetFormFieldDto> FormFields { get; set; }
    }

    public class GetApplicantFormDetailsDto
    {
        public Guid FormId { get; set; }
        public IEnumerable<GetFormFieldDto> DefaultFormFields { get; set; }
        public IEnumerable<GetFormFieldDto> CustomFormFields { get; set; }
    }

    public class GetFormFieldDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string FormType { get; set; }
        public string Placeholder { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public int Index { get; set; }
        public int? RatingLevel { get; set; }

        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FileDto File { get; set; }
        public GetFormFieldValueDto FormFieldValue { get; set; }
        public ICollection<GetFieldOptionDto> Options { get; set; }
    }

    public class GetFormFieldValueDto
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public class GetFieldOptionDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public class CreateFormInputDto
    {
        public Guid FormId { get; set; }
        public string ActivityType { get; set; }
        public ICollection<FormFieldInputDto> FormFields { get; set; }       
    }

    public class FormFieldInputDto
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Placeholder { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public int? RatingLevel { get; set; }
        public int Index { get; set; }
        public FileDto File { get; set; }
        public ICollection<FieldOptionInputDto> Options { get; set; }
    }

    public class FileDto
    {
        public string Type { get; set; }
        public int? NumberLimit { get; set; }
        public int? SingleFileSizeLimit { get; set; }
    }

    public class FieldOptionInputDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
    }

    public class CreateFormFieldValueInputDto
    {
        public Guid LearningTrackId { get; set; }
        public ICollection<CreateFieldValueDto> FieldValues { get; set; }
    }

    public class CreateFieldValueDto
    {
        public CreateFieldValueDto()
        {
            Files = new List<IFormFile>();
        }
        public Guid FormFieldId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public List<IFormFile> Files { get; set; }
    }

    public class GetApplicantFormStatusDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? StageId { get; set; }
        public string Status { get; set; }
    }

    public class GetBulkApplicantForStatusDto
    {
        public Guid[] InvalidApplicantIds { get; set; }
        public GetApplicantFormStatusDto[] Applicants { get; set; }
    }

    public class SubmitActivityFormFieldValueDto
    {
        public Guid ActivityId { get; set; }
        public ICollection<CreateFieldValueDto> FieldValues { get; set; }
    }
}
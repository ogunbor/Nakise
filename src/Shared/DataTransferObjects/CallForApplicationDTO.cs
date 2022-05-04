using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects
{
    public class CreateCallForApplicationInputDto
    {
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public int TargetNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStage { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public ICollection<CreateStageDto> Stages { get; set; }
    }

    public class UpdateCallForApplicationInputDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public int TargetNumber { get; set; }
        public bool IsStage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public ICollection<CreateStageDto> Stages { get; set; }
    }

    public class CreateStageDto
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class GetCallForApplicationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public int TargetNumber { get; set; }
        public bool IsStage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public string Status { get; set; }
        public bool IsClosed { get; set; }
        public string OtherStatus { get; set; }
        public ICollection<GetStageDto> Stages { get; set; }
        public GetProgrammeDto Programme { get; set; }
        public GetActivityFormDto ActivityForm { get; set; }
    }

    public class GetActivityFormDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class GetStageDto
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class GetProgrammeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetCallForApplicationStatusDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public int TargetNumber { get; set; }
        public bool IsStage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public bool IsClosed { get; set; }
    }
    public class CfaGetStageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class CfaGetStageStatDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class GetCfaSubmissionStatDto
    {
        public int SubmissionCount { get; set; }
        public int TargetNumber { get; set; }
    }

    public class ExtendCallForApplication
    {
        public DateTime Date { get; set; }
    }
}

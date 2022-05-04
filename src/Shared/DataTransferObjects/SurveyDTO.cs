using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public class CreateSurveyRequest
    {
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public class GetSurveyDTO
    {
        public Guid Id { get; set; }
        public Guid ActivityId { get; set; }
        public Guid ProgrammeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<SurveyLearningTrackDTO> LearningTracks { get; set; }
    }

    public class UpdateSurveyRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SuccessMessageTitle { get; set; }
        public string SuccessMessageBody { get; set; }
        public List<Guid> LearningTracks { get; set; } = new List<Guid>();
    }

    public class SurveyLearningTrackDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public record GetApproveApplicantSurveyStatsDTO
    {
        public int Total { get; set; }
        public int Answered { get; set; }
        public int NotAnswered { get; set; }
        public int Overdue { get; set; }
    }

    public class GetApprovedApplicantSurveysDto : GetSurveyDTO
    {
      public GetActivityDTO ActivityForm { get; set; }
      public string ProgramTitle { get; set; }
      public string Status { get; set; }
    }
}

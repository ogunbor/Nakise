
using System;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Entities.Activities;

namespace Application.DTOs
{
    public class CreateFormDTO
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

    public class UpdateFormDTO
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

    public class GetLearningTrackFormDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetFormDTO
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
        public List<GetLearningTrackFormDTO> LearningTracks { get; set; }
    }
}
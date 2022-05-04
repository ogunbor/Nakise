using Infrastructure.Contracts.ActivitiyForms;
using Infrastructure.Contracts.Assessments;
using Infrastructure.Contracts.Events;
using Infrastructure.Contracts.Forms;
using Infrastructure.Contracts.Identities;
using Infrastructure.Contracts.Learnings;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Contracts.Surveys;
using Infrastructure.Contracts.Trainings;
using Infrastructure.Contracts.UserProfile;

namespace Infrastructure.Contracts
{
    public interface IRepositoryManager
    {
        IActivityFormRepository ActivityForm { get; }
        IFieldOptionRepository FieldOption { get; }
        IFormFieldRepository FormField { get; }
        IFormFieldValueRepository FormFieldValue { get; }
        IAssessmentLearningTrackRepository AssessmentLearningTrack { get; }
        IAssessmentRepository Assessment { get;  }
        IEventLearningTrackRepository EventLearningTrack { get; }
        IEventRepository Event { get; }
        IFormLearningTrackRepository FormLearningTrack { get; }
        IFormRepository Form { get; }   
        IRoleRepository Role { get; }
        IUserRepository User { get; }
        IUserRoleRepository UserRole { get; }
        ILearningTrackFacilitatorRepository LearningTrackFacilitator { get; }
        ILearningTrackRepository LearningTrack { get; }
        IProgrammeActivityRepository ProgrammeActivity { get; }
        IProgrammeCategoryRepository ProgrammeCategory { get; }
        IProgrammeManagerRepository ProgrammeManager { get; }
        IProgrammeSponsorRepository ProgrammeSponsor { get; }
        IProgrammeRepository Programme { get; }
        ISurveyLearningTrackRepository SurveyLearningTrack { get; }
        ISurveyRepository Survey { get; }
        ITrainingLearningTrackRepository TrainingLearningTrack { get;  }
        ITrainingRepository Training { get;  }
        IActivityRepository Activity { get;  }
        IApplicantRepository Applicant { get;  }
        ICallForApplicationRepository CallForApplication { get; }
        IOrganizationRepository Organization { get; }
        IStageRepository Stage { get; }
        ITokenRepository Token { get; }
        IUserActivityRepository UserActivity { get; }
        IApprovedApplicantRepository ApprovedApplicant { get; }
        IApprovedApplicantProgrammeRepository ApprovedApplicantProgramme { get; }
        IUserInformationRepository UserInformation { get; }
        IUserDocumentRepository UserDocument { get; }
        IEventRegistrationRepository EventRegistration { get; }
        ISurveyResponseRepository SurveyResponse { get; }
        IAssessmentSessionRepository AssessmentSession { get; }
        Task SaveChangesAsync();
        Task BeginTransaction(Func<Task> action);
    }
}

using Application.Contracts.V1.Activities;

namespace Application.Contracts.V1
{
    public interface IServiceManager
    {
        IActivityService ActivityService { get; }
        IAssessmentService AssessmentService { get; }
        ICallForApplicationService CallForApplicationService { get; }
        IEventService EventService { get; }
        IFormService FormService { get; }
        ITrainingService TrainingService { get; }
        IActivityFormService ActivityFormService { get; }
        ILearningTrackService LearningTrackService { get; }
        IOrganizationService OrganizationService { get; }
        IProgrammeService ProgrammeService { get; }
        ISurveyService SurveyService { get; }
        IUserService UserService { get; }
        IAuthenticationService AuthenticationService { get; }
        IPeopleService PeopleService { get; }
    }
}

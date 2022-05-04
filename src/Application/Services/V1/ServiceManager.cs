using Application.Contracts.V1;
using Application.Contracts.V1.Activities;
using Application.Helpers;
using Application.Resources;
using Application.Services.V1.Activities;
using Application.Services.V1.Implementations;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services.V1
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IActivityService> _activityService;
        private readonly Lazy<IAssessmentService> _assessmentService;
        private readonly Lazy<ICallForApplicationService> _callForApplicationService;
        private readonly Lazy<IEventService> _eventService;
        private readonly Lazy<IFormService> _formService;
        private readonly Lazy<ISurveyService> _surveyService;
        private readonly Lazy<ITrainingService> _trainingService;
        private readonly Lazy<IActivityFormService> _activityFromService;
        private readonly Lazy<ILearningTrackService> _learningTrackService;
        private readonly Lazy<IOrganizationService> _organizationService;
        private readonly Lazy<IProgrammeService> _programmeService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IPeopleService> _peopleService;

        public ServiceManager(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper,
            IAwsS3Client awsS3Client,
            IRestErrorLocalizerService localizer,
            IConfiguration configuration,
            IEmailManager emailManager,
            UserManager<User> userManager,
            IWebHelper webHelper,
            RoleManager<Role> roleManager,
            HttpClient httpClient)
        {
            _activityService = new Lazy<IActivityService>(() => 
                new ActivityService(mapper, repository));
            _assessmentService = new Lazy<IAssessmentService>(() => 
                new AssessmentService(mapper, repository, webHelper, httpClient, configuration));
            _callForApplicationService = new Lazy<ICallForApplicationService>(() => 
                new CallForApplicationService(mapper, repository, webHelper));
            _eventService = new Lazy<IEventService>(() => 
                new EventService(mapper, repository, webHelper, awsS3Client, emailManager, userManager));
            _formService = new Lazy<IFormService>(() => 
                new FormService(mapper, repository, webHelper));
            _surveyService = new Lazy<ISurveyService>(() => 
                new SurveyService(mapper, webHelper, repository));
            _trainingService = new Lazy<ITrainingService>(() => 
                new TrainingService(mapper, webHelper, repository));    
            _activityFromService = new Lazy<IActivityFormService>(() => 
                new ActivityFormService(mapper, awsS3Client, webHelper, repository, emailManager, userManager, configuration));
            _learningTrackService = new Lazy<ILearningTrackService>(() => 
                new LearningTrackService(mapper, repository, webHelper));
            _organizationService = new Lazy<IOrganizationService>(() => 
                new OrganizationService(mapper, repository, webHelper));
            _programmeService = new Lazy<IProgrammeService>(() => 
                new ProgrammeService(mapper, repository, webHelper));
            _userService = new Lazy<IUserService>(() => 
                new UserService(mapper, configuration, userManager, roleManager, emailManager, awsS3Client, 
                    webHelper, repository));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(mapper, configuration, userManager, roleManager, emailManager, repository, awsS3Client, webHelper));
            _peopleService = new Lazy<IPeopleService>(() => new PeopleService(mapper, repository, userManager, webHelper));
        }

        public IActivityService ActivityService => _activityService.Value;

        public IAssessmentService AssessmentService => _assessmentService.Value;

        public ICallForApplicationService CallForApplicationService => _callForApplicationService.Value;

        public IEventService EventService => _eventService.Value;

        public IFormService FormService => _formService.Value;

        public ITrainingService TrainingService => _trainingService.Value;

        public IActivityFormService ActivityFormService => _activityFromService.Value;

        public ILearningTrackService LearningTrackService => _learningTrackService.Value;
        public IOrganizationService OrganizationService => _organizationService.Value;

        public IProgrammeService ProgrammeService => _programmeService.Value;

        public ISurveyService SurveyService => _surveyService.Value;

        public IUserService UserService => _userService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IPeopleService PeopleService => _peopleService.Value;
    }
}

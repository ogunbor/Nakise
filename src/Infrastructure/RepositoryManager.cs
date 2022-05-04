using Infrastructure.Contracts;
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
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories;
using Infrastructure.Repositories.ActivityForms;
using Infrastructure.Repositories.Assessments;
using Infrastructure.Repositories.Events;
using Infrastructure.Repositories.Forms;
using Infrastructure.Repositories.Identities;
using Infrastructure.Repositories.Learnings;
using Infrastructure.Repositories.Programmes;
using Infrastructure.Repositories.Surveys;
using Infrastructure.Repositories.Trainings;
using Infrastructure.Repositories.UserProfile;

namespace Infrastructure
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _appDbContext;
        private readonly Lazy<IActivityFormRepository> _activityFormRepository;
        private readonly Lazy<IFieldOptionRepository> _fieldOptionRepository;
        private readonly Lazy<IFormFieldRepository> _formFieldRepository;
        private readonly Lazy<IFormFieldValueRepository> _formFieldValueRepository;
        private readonly Lazy<IAssessmentLearningTrackRepository> _assessmentLearningTrackRepository;
        private readonly Lazy<IAssessmentRepository> _assessmentRepository;
        private readonly Lazy<IEventLearningTrackRepository> _eventLearningTrackRepository;
        private readonly Lazy<IEventRepository> _eventRepository;
        private readonly Lazy<IFormLearningTrackRepository> _formLearningTrackRepository;
        private readonly Lazy<IFormRepository> _formRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IUserRoleRepository> _userRoleRepository;
        private readonly Lazy<ILearningTrackFacilitatorRepository> _learningTrackFacilitatorRepository;
        private readonly Lazy<ILearningTrackRepository> _learningTrackRepository;
        private readonly Lazy<IProgrammeActivityRepository> _programmeActivityRepository;
        private readonly Lazy<IProgrammeCategoryRepository> _programmeCategoryRepository;
        private readonly Lazy<IProgrammeManagerRepository> _programmeManagerRepository;
        private readonly Lazy<IProgrammeRepository> _programmeRepository;
        private readonly Lazy<IProgrammeSponsorRepository> _programmeSponsorRepository;
        private readonly Lazy<ISurveyLearningTrackRepository> _surveyLearningTrackRepository;
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<ITrainingLearningTrackRepository> _trainingLearningTrackRepository;
        private readonly Lazy<ITrainingRepository> _trainingRepository;
        private readonly Lazy<IActivityRepository> _activityRepository;
        private readonly Lazy<IApplicantRepository> _applicantRepository;
        private readonly Lazy<ICallForApplicationRepository> _callForApplicationRepository;
        private readonly Lazy<IOrganizationRepository> _organizationRepository;
        private readonly Lazy<IStageRepository> _stageRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<IUserActivityRepository> _userActvitiyRepository;
        private readonly Lazy<IApprovedApplicantRepository> _approvedApplicantRepository;
        private readonly Lazy<IApprovedApplicantProgrammeRepository> _approvedApplicantProgrammeRepository;
        private readonly Lazy<IUserInformationRepository> _userInformationRepository;
        private readonly Lazy<IUserDocumentRepository> _userDocumentRepository;
        private readonly Lazy<IEventRegistrationRepository> _eventRegistrationRepository;
        private readonly Lazy<ISurveyResponseRepository> _surveyResponseRepository;
        private readonly Lazy<IAssessmentSessionRepository> _assessmentSessionRepository;
        public RepositoryManager(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _activityFormRepository = new Lazy<IActivityFormRepository>(() => new ActivityFormRepository(appDbContext));
            _fieldOptionRepository = new Lazy<IFieldOptionRepository>(() => new FieldOptionRepository(appDbContext));
            _formFieldRepository = new Lazy<IFormFieldRepository>(() => new FormFieldRepository(appDbContext));
            _formFieldValueRepository = new Lazy<IFormFieldValueRepository>(() => new FormFieldValueRepository(appDbContext));
            _assessmentLearningTrackRepository = new Lazy<IAssessmentLearningTrackRepository>(() => new AssessmentLearningTrackRepository(appDbContext));
            _assessmentRepository = new Lazy<IAssessmentRepository>(() => new AssessmentRepository(appDbContext));
            _eventLearningTrackRepository = new Lazy<IEventLearningTrackRepository>(() => new EventLearningTrackRepository(appDbContext));
            _eventRepository = new Lazy<IEventRepository>(() => new EventRepository(appDbContext));
            _formLearningTrackRepository = new Lazy<IFormLearningTrackRepository>(() => new FormLearningTrackRepository(appDbContext));
            _formRepository = new Lazy<IFormRepository>(() => new FormRepository(appDbContext));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(appDbContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(appDbContext));
            _userRoleRepository = new Lazy<IUserRoleRepository>(() => new UserRoleRepository(appDbContext));
            _learningTrackFacilitatorRepository = new Lazy<ILearningTrackFacilitatorRepository>(() => new LearningTrackFacilitatorRepository(appDbContext));
            _learningTrackRepository = new Lazy<ILearningTrackRepository>(() => new LearningTrackRepository(appDbContext));
            _programmeActivityRepository = new Lazy<IProgrammeActivityRepository>(() => new ProgrammeActivityRepository(appDbContext));
            _programmeCategoryRepository = new Lazy<IProgrammeCategoryRepository>(() => new ProgrammeCategoryRepository(appDbContext));
            _programmeManagerRepository = new Lazy<IProgrammeManagerRepository>(() => new ProgrammeManagerRepository(appDbContext));
            _programmeRepository = new Lazy<IProgrammeRepository>(() => new ProgrammeRepository(appDbContext));
            _programmeSponsorRepository = new Lazy<IProgrammeSponsorRepository>(() => new ProgrammeSponsorRepository(appDbContext));
            _surveyLearningTrackRepository = new Lazy<ISurveyLearningTrackRepository>(() => new SurveyLearningTrackRepository(appDbContext));
            _surveyRepository = new Lazy<ISurveyRepository>(() => new SurveyRepository(appDbContext));
            _trainingLearningTrackRepository = new Lazy<ITrainingLearningTrackRepository>(() => new TrainingLearningTrackRepository(appDbContext));
            _trainingRepository = new Lazy<ITrainingRepository>(() => new TrainingRepository(appDbContext));
            _activityRepository = new Lazy<IActivityRepository>(() => new ActivityRepository(appDbContext));
            _applicantRepository = new Lazy<IApplicantRepository>(() => new ApplicantRepository(appDbContext));
            _callForApplicationRepository = new Lazy<ICallForApplicationRepository>(() => new CallForApplicationRepository(appDbContext));
            _organizationRepository = new Lazy<IOrganizationRepository>(() => new OrganizationRepository(appDbContext));
            _stageRepository = new Lazy<IStageRepository>(() => new StageRepository(appDbContext));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(appDbContext));
            _userActvitiyRepository = new Lazy<IUserActivityRepository>(() => new UserActivitityRepository(appDbContext));
            _approvedApplicantRepository = new Lazy<IApprovedApplicantRepository>(() => new ApprovedApplicantRepository(appDbContext));
            _approvedApplicantProgrammeRepository = new Lazy<IApprovedApplicantProgrammeRepository>(() => new ApprovedApplicantProgrammeRepository(appDbContext));
            _userInformationRepository = new Lazy<IUserInformationRepository>(() => new UserInformationRepository(appDbContext));
            _userDocumentRepository = new Lazy<IUserDocumentRepository>(() => new UserDocumentRepository(appDbContext));
            _eventRegistrationRepository = new Lazy<IEventRegistrationRepository>(() => new EventRegistrationRepository(appDbContext));
            _surveyResponseRepository = new Lazy<ISurveyResponseRepository>(() => new SurveyResponseRepository(appDbContext));
            _assessmentSessionRepository = new Lazy<IAssessmentSessionRepository>(() => new AssessmentSessionRepository(appDbContext));
        }

        public IActivityFormRepository ActivityForm => _activityFormRepository.Value;
        public IFieldOptionRepository FieldOption => _fieldOptionRepository.Value;
        public IFormFieldRepository FormField => _formFieldRepository.Value;
        public IFormFieldValueRepository FormFieldValue => _formFieldValueRepository.Value;
        public IAssessmentLearningTrackRepository AssessmentLearningTrack => _assessmentLearningTrackRepository.Value;
        public IAssessmentRepository Assessment => _assessmentRepository.Value;
        public IEventLearningTrackRepository EventLearningTrack => _eventLearningTrackRepository.Value;
        public IEventRepository Event => _eventRepository.Value;
        public IFormLearningTrackRepository FormLearningTrack => _formLearningTrackRepository.Value;
        public IFormRepository Form => _formRepository.Value;
        public IRoleRepository Role => _roleRepository.Value;
        public IUserRepository User => _userRepository.Value;
        public IUserRoleRepository UserRole => _userRoleRepository.Value;
        public ILearningTrackFacilitatorRepository LearningTrackFacilitator => _learningTrackFacilitatorRepository.Value;
        public ILearningTrackRepository LearningTrack => _learningTrackRepository.Value;
        public IProgrammeActivityRepository ProgrammeActivity => _programmeActivityRepository.Value;
        public IProgrammeCategoryRepository ProgrammeCategory => _programmeCategoryRepository.Value;
        public IProgrammeManagerRepository ProgrammeManager => _programmeManagerRepository.Value;
        public IProgrammeSponsorRepository ProgrammeSponsor => _programmeSponsorRepository.Value;
        public IProgrammeRepository Programme => _programmeRepository.Value;
        public ISurveyLearningTrackRepository SurveyLearningTrack => _surveyLearningTrackRepository.Value;
        public ISurveyRepository Survey => _surveyRepository.Value;
        public ITrainingLearningTrackRepository TrainingLearningTrack => _trainingLearningTrackRepository.Value;
        public ITrainingRepository Training => _trainingRepository.Value;
        public IActivityRepository Activity => _activityRepository.Value;
        public IApplicantRepository Applicant => _applicantRepository.Value;
        public ICallForApplicationRepository CallForApplication => _callForApplicationRepository.Value;
        public IOrganizationRepository Organization => _organizationRepository.Value;
        public IStageRepository Stage => _stageRepository.Value;
        public ITokenRepository Token => _tokenRepository.Value;
        public IUserActivityRepository UserActivity => _userActvitiyRepository.Value;
        public IApprovedApplicantRepository ApprovedApplicant => _approvedApplicantRepository.Value;
        public IApprovedApplicantProgrammeRepository ApprovedApplicantProgramme => _approvedApplicantProgrammeRepository.Value;
        public IUserInformationRepository UserInformation => _userInformationRepository.Value;
        public IUserDocumentRepository UserDocument => _userDocumentRepository.Value;
        public IEventRegistrationRepository EventRegistration => _eventRegistrationRepository.Value;
        public ISurveyResponseRepository SurveyResponse => _surveyResponseRepository.Value;
        public IAssessmentSessionRepository AssessmentSession => _assessmentSessionRepository.Value;
        public async Task BeginTransaction(Func<Task> action)
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                await action();

                await SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SaveChangesAsync() => await _appDbContext.SaveChangesAsync();
    }
}

using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domian.Entities.ActivityForms;
using Infrastructure.Data.DbContext.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.DbContext
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid,
        UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }

        public override DbSet<User> Users { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<ProgrammeActivity> ProgrammeActivities { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LearningTrack> LearningTracks { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<ProgrammeManager> ProgrammeManagers { get; set; }
        public DbSet<LearningTrackFacilitator> LearningTrackFacilitators { get; set; }
        public DbSet<ProgrammeCategory> ProgrammeCategories { get; set; }
        public DbSet<ProgrammeSponsor> ProgrammeSponsors { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentLearningTrack> AssessmentLearningTracks { get; set; }
        public DbSet<CallForApplication> CallForApplications { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventLearningTrack> EventLearningTracks { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormLearningTrack> FormLearningTracks { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyLearningTrack> SurveyLearningTracks { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<TrainingLearningTrack> TrainingLearningTracks { get; set; }
        public DbSet<ActivityForm> ActivityForms { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FieldOption> FieldOptions { get; set; }
        public DbSet<FormFieldValue> FormFieldValues { get; set; }
        public DbSet<ApplicantDetail> ApplicantDetails { get; set; }
        public DbSet<ApprovedApplicant> ApprovedApplicants { get; set; }
        public DbSet<ApprovedApplicantProgramme> ApprovedApplicantProgrammes { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<AssessmentSession> AssessmentSessions { get; set; }
    }
}

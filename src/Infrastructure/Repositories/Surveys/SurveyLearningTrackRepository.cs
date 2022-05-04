using Domain.Entities.Activities;
using Infrastructure.Contracts.Surveys;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Surveys
{
    public class SurveyLearningTrackRepository : Repository<SurveyLearningTrack>, ISurveyLearningTrackRepository
    {
        public SurveyLearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

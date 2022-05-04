using Domain.Entities.Activities;
using Infrastructure.Contracts.Surveys;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Surveys
{
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        public SurveyRepository(AppDbContext context) : base(context)
        {
        }
    }
}

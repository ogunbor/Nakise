using Domain.Entities.Actvities;
using Infrastructure.Contracts.Surveys;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Surveys
{
    public class SurveyResponseRepository : Repository<SurveyResponse>, ISurveyResponseRepository
    {
        public SurveyResponseRepository(AppDbContext context) : base(context)
        {
        }
    }
}

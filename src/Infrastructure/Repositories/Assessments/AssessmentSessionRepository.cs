using Domain.Entities.Actvities;
using Infrastructure.Contracts.Assessments;
using Infrastructure.Data.DbContext;


namespace Infrastructure.Repositories.Assessments
{
    public class AssessmentSessionRepository : Repository<AssessmentSession>, IAssessmentSessionRepository
    {
        public AssessmentSessionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using Domain.Entities.Activities;
using Infrastructure.Contracts.Assessments;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Assessments
{
    public class AssessmentRepository : Repository<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(AppDbContext context) : base(context)
        {
        }
    }
}

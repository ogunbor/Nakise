using Domain.Entities.Actvities;
using Infrastructure.Contracts.Assessments;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Assessments
{
    public class AssessmentLearningTrackRepository : Repository<AssessmentLearningTrack>, IAssessmentLearningTrackRepository
    {
        public AssessmentLearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

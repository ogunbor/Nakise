using Domain.Entities;
using Infrastructure.Contracts.Learnings;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Learnings
{
    public class LearningTrackRepository : Repository<LearningTrack>, ILearningTrackRepository
    {
        public LearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

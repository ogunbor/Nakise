using Domain.Entities;
using Infrastructure.Contracts.Learnings;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Learnings
{
    public class LearningTrackFacilitatorRepository : Repository<LearningTrackFacilitator>, ILearningTrackFacilitatorRepository
    {
        public LearningTrackFacilitatorRepository(AppDbContext context) : base(context)
        {
        }
    }
}

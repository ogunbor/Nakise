using Domain.Entities.Activities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class StageRepository : Repository<Stage>, IStageRepository
    {
        public StageRepository(AppDbContext context) : base(context)
        {
        }
    }
}

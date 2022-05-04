using Domain.Entities.Activities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Programmes
{
    public class ProgrammeActivityRepository : Repository<ProgrammeActivity>, IProgrammeActivityRepository
    {
        public ProgrammeActivityRepository(AppDbContext context) : base(context)
        {
        }
    }
}

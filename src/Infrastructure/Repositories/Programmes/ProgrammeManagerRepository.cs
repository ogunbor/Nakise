using Domain.Entities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Programmes
{
    public class ProgrammeManagerRepository : Repository<ProgrammeManager>, IProgrammeManagerRepository
    {
        public ProgrammeManagerRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using Domain.Entities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Programmes
{
    public class ProgrammeRepository : Repository<Programme>, IProgrammeRepository
    {
        public ProgrammeRepository(AppDbContext context) : base(context)
        {
        }
    }
}

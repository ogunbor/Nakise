using Domain.Entities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Programmes
{
    public class ProgrammeSponsorRepository : Repository<ProgrammeSponsor>, IProgrammeSponsorRepository
    {
        public ProgrammeSponsorRepository(AppDbContext context) : base(context)
        {
        }
    }
}

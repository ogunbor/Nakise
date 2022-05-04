using Domain.Entities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Programmes
{
    public class ProgrammeCategoryRepository : Repository<ProgrammeCategory>, IProgrammeCategoryRepository
    {
        public ProgrammeCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}

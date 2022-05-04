using Domain.Entities.Actvities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class CallForApplicationRepository : Repository<CallForApplication>, ICallForApplicationRepository
    {
        public CallForApplicationRepository(AppDbContext context) : base(context)
        {
        }
    }
}

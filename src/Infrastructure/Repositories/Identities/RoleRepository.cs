using Domain.Entities;
using Infrastructure.Contracts.Identities;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Identities
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }
    }
}

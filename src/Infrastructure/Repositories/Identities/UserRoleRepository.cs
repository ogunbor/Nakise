using Domain.Entities;
using Infrastructure.Contracts.Identities;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Identities
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using Domain.Entities;
using Infrastructure.Contracts.Identities;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Identities
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}

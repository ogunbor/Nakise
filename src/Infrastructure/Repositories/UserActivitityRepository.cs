using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class UserActivitityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivitityRepository(AppDbContext context) : base(context)
        {
        }
    }
}

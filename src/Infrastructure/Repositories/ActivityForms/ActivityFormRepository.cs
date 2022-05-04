using Domian.Entities.ActivityForms;
using Infrastructure.Contracts.ActivitiyForms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.ActivityForms
{
    public class ActivityFormRepository : Repository<ActivityForm>, IActivityFormRepository
    {
        public ActivityFormRepository(AppDbContext context) : base(context)
        {
        }
    }
}

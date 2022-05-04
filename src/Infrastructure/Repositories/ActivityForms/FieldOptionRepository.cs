using Domian.Entities.ActivityForms;
using Infrastructure.Contracts.ActivitiyForms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.ActivityForms
{
    public class FieldOptionRepository : Repository<FieldOption>, IFieldOptionRepository
    {
        public FieldOptionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

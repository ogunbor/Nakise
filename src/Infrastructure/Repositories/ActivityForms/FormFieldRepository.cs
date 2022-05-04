using Domian.Entities.ActivityForms;
using Infrastructure.Contracts.ActivitiyForms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.ActivityForms
{
    public class FormFieldRepository : Repository<FormField>, IFormFieldRepository
    {
        public FormFieldRepository(AppDbContext context) : base(context)
        {
        }
    }
}

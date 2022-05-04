using Domian.Entities.ActivityForms;
using Infrastructure.Contracts.ActivitiyForms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.ActivityForms
{
    public class FormFieldValueRepository : Repository<FormFieldValue>, IFormFieldValueRepository
    {
        public FormFieldValueRepository(AppDbContext context) : base(context)
        {
        }
    }
}

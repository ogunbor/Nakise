using Domain.Entities.Actvities;
using Infrastructure.Contracts.Forms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Forms
{
    public class FormRepository : Repository<Form>, IFormRepository
    {
        public FormRepository(AppDbContext context) : base(context)
        {
        }
    }
}

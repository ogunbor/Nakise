using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class UserDocumentRepository : Repository<UserDocument>, IUserDocumentRepository
    {
        public UserDocumentRepository(AppDbContext context) : base(context)
        {
        }
    }
}

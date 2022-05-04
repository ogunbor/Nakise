using Domain.Entities;
using Infrastructure.Contracts.UserProfile;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.UserProfile
{
    public class UserInformationRepository : Repository<UserInformation>, IUserInformationRepository
    {
        public UserInformationRepository(AppDbContext context) : base(context)
        {
        }
    }
}

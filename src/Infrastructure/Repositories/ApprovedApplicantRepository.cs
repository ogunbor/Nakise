using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ApprovedApplicantRepository : Repository<ApprovedApplicant>, IApprovedApplicantRepository
    {
        public ApprovedApplicantRepository(AppDbContext context) : base(context)
        {

        }
    }
}

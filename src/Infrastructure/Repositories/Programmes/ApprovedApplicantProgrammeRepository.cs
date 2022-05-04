using Domain.Entities;
using Infrastructure.Contracts.Programmes;
using Infrastructure.Data.DbContext;


namespace Infrastructure.Repositories.Programmes
{
    public class ApprovedApplicantProgrammeRepository : Repository<ApprovedApplicantProgramme>, IApprovedApplicantProgrammeRepository
    {
        public ApprovedApplicantProgrammeRepository(AppDbContext context) : base(context)
        {

        }
    }
}

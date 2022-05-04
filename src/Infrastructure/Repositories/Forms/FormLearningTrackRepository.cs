using Domain.Entities.Activities;
using Infrastructure.Contracts.Forms;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Forms
{
    public class FormLearningTrackRepository : Repository<FormLearningTrack>, IFormLearningTrackRepository
    {
        public FormLearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using Domain.Entities.Activities;
using Infrastructure.Contracts.Trainings;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Trainings
{
    public class TrainingRepository : Repository<Training>, ITrainingRepository
    {
        public TrainingRepository(AppDbContext context) : base(context)
        {
        }
    }
}

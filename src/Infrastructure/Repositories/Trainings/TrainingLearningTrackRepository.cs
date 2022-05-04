using Domain.Entities.Activities;
using Infrastructure.Contracts.Trainings;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Trainings
{
    public class TrainingLearningTrackRepository : Repository<TrainingLearningTrack>, ITrainingLearningTrackRepository
    {
        public TrainingLearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

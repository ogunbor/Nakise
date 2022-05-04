using Domain.Entities.Actvities;
using Infrastructure.Contracts.Events;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Events
{
    public class EventLearningTrackRepository : Repository<EventLearningTrack>, IEventLearningTrackRepository
    {
        public EventLearningTrackRepository(AppDbContext context) : base(context)
        {
        }
    }
}

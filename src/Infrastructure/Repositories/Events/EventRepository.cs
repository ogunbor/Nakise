using Domain.Entities.Actvities;
using Infrastructure.Contracts.Events;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories.Events
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context)
        {
        }
    }
}

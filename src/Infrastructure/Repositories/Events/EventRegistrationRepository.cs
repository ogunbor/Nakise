using Domain.Entities.Actvities;
using Infrastructure.Contracts.Events;
using Infrastructure.Data.DbContext;


namespace Infrastructure.Repositories.Events
{
    public class EventRegistrationRepository : Repository<EventRegistration>, IEventRegistrationRepository
    {
        public EventRegistrationRepository(AppDbContext context) : base(context)
        {
        }
    }
}

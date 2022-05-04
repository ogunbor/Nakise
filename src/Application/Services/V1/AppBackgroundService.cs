using Domain.Entities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Services.V1
{
    public class AppBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AppBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var _repository = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();

            await CloseCallForApplications(_repository);
            await UpdateProgrammeStatus(_repository);
            await UpdateEventStatus(_repository);
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        public static async Task CloseCallForApplications(IRepositoryManager repository)
        {
            var cfaList = new List<CallForApplication>();
            var cfas = repository.CallForApplication.Get(x => x.EndDate.Date < DateTime.Now.Date && !x.IsClosed);

            foreach (var cfa in cfas)
            {
                cfa.IsClosed = true;
                cfa.UpdatedAt = DateTime.Now;

                cfaList.Add(cfa);
            }

            repository.CallForApplication.UpdateRange(cfaList);
            await repository.SaveChangesAsync();
        }

        public static async Task UpdateProgrammeStatus(IRepositoryManager repository)
        {
            var programmeList = new List<Programme>();
            var programmes = await repository.Programme.GetAllAsync();

            foreach (var programme in programmes)
            {
                if (DateTime.Today.Date > programme.StartDate.Date
                    && programme.StartDate.Date < DateTime.Today.Date
                    && !programme.Status.Equals(EProgrammeStatus.Ongoing.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    programme.Status = EProgrammeStatus.Ongoing.ToString();
                    programme.UpdatedAt = DateTime.UtcNow;
                    programmeList.Add(programme);
                    continue;
                }

                if (DateTime.Today.Date > programme.EndDate.Date 
                    && !programme.Status.Equals(EProgrammeStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    programme.Status = EProgrammeStatus.Completed.ToString();
                    programme.UpdatedAt = DateTime.UtcNow;
                    programmeList.Add(programme);
                }
            }

            repository.Programme.UpdateRange(programmeList);
            await repository.SaveChangesAsync();
        }

        public static async Task UpdateEventStatus(IRepositoryManager repository)
        {
            var eventList = new List<Event>();
            var events = await repository.Event.GetAllAsync();

            foreach (var @event in events)
            {
                if (DateTime.Today.Date > @event.StartDate.Date
                    && @event.StartDate.Date < @event.EndDate.Date
                    && !@event.Status.Equals(EProgrammeStatus.Ongoing.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    @event.Status = EProgrammeStatus.Ongoing.ToString();
                    @event.UpdatedAt = DateTime.UtcNow;
                    eventList.Add(@event);
                    continue;
                }

                if (DateTime.Today.Date > @event.EndDate.Date && !@event.Status.Equals(EProgrammeStatus.Completed.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    @event.Status = EProgrammeStatus.Completed.ToString();
                    @event.UpdatedAt = DateTime.UtcNow;
                    eventList.Add(@event);
                }
            }

            repository.Event.UpdateRange(eventList);
            await repository.SaveChangesAsync();
        }
    }
}

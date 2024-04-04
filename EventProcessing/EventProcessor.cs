using System.Text.Json;
using System.Windows.Markup;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        //Cannot inject IRepository instance directly in EventProcessor, since Event published is defined as Singleton instance whereas IRepository instance are either scoped/transient(lifetime is shorter)
        public EventProcessor(IServiceScopeFactory scopeFactory, AutoMapper.IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch(eventType)
            {
                //Add platform to db if Platform_Published event detected
                case EventType.Platform_Published:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }
        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.Platform_Published;
                default:
                    Console.WriteLine("--> Could not determine event type!");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists!");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not add platform to DB {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        Platform_Published,
        Undetermined
    }
}
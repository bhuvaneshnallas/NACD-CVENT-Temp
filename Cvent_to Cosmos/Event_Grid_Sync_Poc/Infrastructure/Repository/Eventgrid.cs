using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Event_Grid_Sync_Poc.Domain.Context.Model;

namespace Event_Grid_Sync_Poc.Infrastructure.Repository
{
    public class Eventgrid
    {
        private readonly IConfiguration _config;
        public Eventgrid(IConfiguration config)
        {
            _config = config;
        }
        public List<CloudEvent> TransformToCloudEvents(List<Event> events)
        {
            var cloudEvents = new List<CloudEvent>();

            foreach (var e in events)
            {
                var cloudEvent = new CloudEvent(
                    source: "https://your-app.com/cvent", 
                    type: "Cvent.Event.Created",
                    jsonSerializableData: e
                );

                cloudEvent.Subject = $"cvent/events/{e.id}";
                cloudEvents.Add(cloudEvent);
            }

            return cloudEvents;
        }


        public async Task PublishToEventGridAsync(List<CloudEvent> cloudEvents)
        {
            try
            {
                string topicEndpoint = _config["EventGrid:TopicEndpoint"];
                string topicKey = _config["EventGrid:TopicKey"];

                var credentials = new AzureKeyCredential(topicKey);
                var client = new EventGridPublisherClient(new Uri(topicEndpoint), credentials);

                await client.SendEventsAsync(cloudEvents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing to Event Grid: {ex.Message}");
            }
        }

    }
}

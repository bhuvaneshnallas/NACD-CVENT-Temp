using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Domain.Models;
using SyncBridge.Domain.Models.CVENT;

namespace SyncBridge.Infrastructure.Services
{
    public class EventGridService : IEventGridService
    {
        public EventGridService() { }
        public async Task<object?> GetEventGridData(CloudEvent cloudEvent)
        {
            try
            {
                return cloudEvent.Type switch
                {
                    var t when t == Constants.EVENT_OBJECT
                        => Deserialize<EventEntity>(cloudEvent),

                    var t when t == Constants.TICKETTYPE_OBJECT
                        => Deserialize<TicketType>(cloudEvent),

                    var t when t == Constants.ATTENDEE_OBJECT
                        => Deserialize<Attendee>(cloudEvent),

                    var t when t == Constants.CONTACT_OBJECT
                        => Deserialize<Contact>(cloudEvent),

                    var t when t == Constants.SALESORDER_OBJECT
                        => Deserialize<SalesOrder>(cloudEvent),

                    _ => null
                };
            }
            catch (Exception)
            {
                // TODO: log exception
                throw;
            }
        }



        private static TModel? Deserialize<TModel>(CloudEvent cloudEvent)
        {
            if (cloudEvent.Data == null)
                return default;

            // Convert BinaryData -> Stream -> Object
            return JsonSerializer.Deserialize<TModel>(
                cloudEvent.Data.ToStream(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }


    }
}

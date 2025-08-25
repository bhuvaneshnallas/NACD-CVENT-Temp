using Azure.Messaging;
using SyncBridge.Domain.Models;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Interfaces
{
    public interface IEventGridService
    {
        Task<object?> GetEventGridData(CloudEvent cloudEvent);

    }
}

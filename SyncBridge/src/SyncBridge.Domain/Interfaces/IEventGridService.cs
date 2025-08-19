using Azure.Messaging;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Interfaces
{
    public interface IEventGridService
    {
        Task<object?> GetEventGridData(CloudEvent cloudEvent);
    }
}

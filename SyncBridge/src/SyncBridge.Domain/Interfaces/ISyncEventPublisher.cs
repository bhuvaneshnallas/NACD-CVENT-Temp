namespace SyncBridge.Domain.Interfaces;

public interface ISyncEventPublisher
{
    Task<bool> PublishAsync<T>(T eventData, string id, string module, string eventType, string source);
}

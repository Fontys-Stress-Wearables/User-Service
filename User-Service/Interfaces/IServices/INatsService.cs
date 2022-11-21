using NATS.Client;

namespace User_Service.Interfaces.IServices;

public interface INatsService
{
    public IConnection Connect();
    public void Publish<T>(string topic, string tenantId, T data);
    public void Subscribe(string target);
}

namespace User_Service.Models
{
    public class NatsMessage<T>
    {
        public string Origin { get; set; } = "user-service";
        public string Target { get; set; }
        public string TenantId { get; set; }
        public T Message { get; set; }
    }
}

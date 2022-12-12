using NATS.Client;
using Newtonsoft.Json;
using System.Text;
using User_Service.Interfaces.IServices;
using User_Service.Models;

namespace User_Service.Services
{
    public class NatsService : INatsService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private IAsyncSubscription? _asyncSubscription;

        public NatsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = Connect();
        }

        public IConnection Connect()
        {
            ConnectionFactory cf = new ConnectionFactory();
            Options opts = ConnectionFactory.GetDefaultOptions();

            opts.Url = _configuration.GetConnectionString("NATSContext");

            return cf.CreateConnection(opts);
        }

        public void Publish<T>(string target, string tenantId, T data)
        {
            var message = new NatsMessage<T> { target = target, tenantId = tenantId, message = data };
            _connection?.Publish(target, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }

        public void Subscribe(string target)
        {
            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            {
                string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine(receivedMessage);
            };
            _asyncSubscription = _connection?.SubscribeAsync(target);
            if (_asyncSubscription != null)
            {
                _asyncSubscription.MessageHandler += h;
                _asyncSubscription.Start();
            }
        }
    }
}

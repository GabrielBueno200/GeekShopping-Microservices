using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace GeekShopping.MessageBus.RabbitMQMessageSender;

public abstract class BaseRabbitMQMessageSender
{
    private readonly IConfiguration _configuration;
    protected IConnection _connection;

    protected BaseRabbitMQMessageSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private void CreateConnection()
    {
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = connectionFactory.CreateConnection();
        }
        catch
        {
            //Log exception
            throw;
        }
    }

    public abstract void SendMessage(BaseMessage baseMessage, string queueName = default);

    protected byte[] GetMessageAsByteArray<TMessage>(BaseMessage baseMessage)
        where TMessage : BaseMessage
     =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize<TMessage>(
            baseMessage as TMessage,
            options: new() { WriteIndented = true }
        ));

    protected bool ConnectionExists()
    {
        if (_connection is not null) return true;
        CreateConnection();
        return _connection is not null;
    }
}

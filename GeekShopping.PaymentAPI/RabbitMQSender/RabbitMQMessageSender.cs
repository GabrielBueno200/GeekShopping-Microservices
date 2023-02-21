using System.Text;
using System.Text.Json;
using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;

    public RabbitMQMessageSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMessage(BaseMessage baseMessage, string queueName)
    {
        if (ConnectionExists())
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queueName,
                    basicProperties: null,
                    body: GetMessageAsByteArray(baseMessage)
                );
            }
        }
    }

    private byte[] GetMessageAsByteArray(BaseMessage baseMessage) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize<UpdatePaymentResultMessage>(
            baseMessage as UpdatePaymentResultMessage,
            options: new() { WriteIndented = true }
        ));

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
            throw;
        }
    }

    private bool ConnectionExists()
    {
        if (_connection is not null) return true;
        CreateConnection();
        return _connection is not null;
    }
}

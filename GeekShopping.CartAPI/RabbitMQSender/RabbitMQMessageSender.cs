using System.Text;
using System.Text.Json;
using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly IConfiguration _configuration;

    public RabbitMQMessageSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMessage(BaseMessage baseMessage, string queueName)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using (var connection = connectionFactory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
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
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize<CheckoutHeaderVO>(
            baseMessage as CheckoutHeaderVO,
            options: new() { WriteIndented = true }
        ));
}

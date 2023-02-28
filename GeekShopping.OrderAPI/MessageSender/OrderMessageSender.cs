using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageSender;
using GeekShopping.OrderAPI.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace GeekShopping.OrderAPI.MessageSender;

public class OrderMessageSender : BaseRabbitMQMessageSender
{
    public OrderMessageSender(IConfiguration configuration)
        : base(configuration) { }

    public override void SendMessage(BaseMessage baseMessage, string queueName = default)
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
                    body: GetMessageAsByteArray<PaymentVO>(baseMessage)
                );
            }
        }
    }
}

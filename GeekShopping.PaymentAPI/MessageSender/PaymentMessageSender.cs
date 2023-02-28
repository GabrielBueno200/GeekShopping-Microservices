using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageSender;
using GeekShopping.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace GeekShopping.PaymentAPI.MessageSender;

public class PaymentMessageSender : BaseRabbitMQMessageSender
{
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public PaymentMessageSender(IConfiguration configuration)
        : base(configuration) { }


    public override void SendMessage(BaseMessage baseMessage, string queueName = default)
    {
        if (ConnectionExists())
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);
                channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
                channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);

                channel.QueueBind(
                    PaymentEmailUpdateQueueName,
                    ExchangeName,
                    routingKey: "PaymentEmail"
                );

                channel.QueueBind(
                    PaymentOrderUpdateQueueName,
                    ExchangeName,
                    routingKey: "PaymentOrder"
                );

                var body = GetMessageAsByteArray<UpdatePaymentResultMessage>(baseMessage);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: "PaymentEmail", basicProperties: null, body: body);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: "PaymentOrder", basicProperties: null, body: body);
            }
        }
    }
}

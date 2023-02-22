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
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public RabbitMQMessageSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendMessage(BaseMessage baseMessage)
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

                var body = GetMessageAsByteArray(baseMessage);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: "PaymentEmail", basicProperties: null, body: body);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: "PaymentOrder", basicProperties: null, body: body);
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

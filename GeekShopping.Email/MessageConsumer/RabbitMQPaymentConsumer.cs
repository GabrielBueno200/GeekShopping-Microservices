using System.Threading.Tasks;
using GeekShopping.Email.Repository;
using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageConsumer;
using GeekShopping.OrderAPI.Messages;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BaseRabbitMQMessageConsumer<UpdatePaymentResultMessage>
{
    private readonly IEmailRepository _repository;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IEmailRepository repository
    ) : base(configuration)
    {
        _repository = repository;
        ConsumeCallback = ProcessLogs;

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, routingKey: "PaymentEmail");
    }

    protected override EventingBasicConsumer ConsumeMessage()
    {
        var consumer = base.ConsumeMessage();
        _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

        return consumer;
    }

    private async Task ProcessLogs(BaseMessage message)
    {
        try
        {
            await _repository.LogEmail(message as UpdatePaymentResultMessage);
        }
        catch
        {
            throw;
        }
    }
}
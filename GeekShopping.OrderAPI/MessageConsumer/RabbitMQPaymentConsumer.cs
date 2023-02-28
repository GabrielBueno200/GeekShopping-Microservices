using System.Threading.Tasks;
using GeekShopping.Email.Messages;
using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageConsumer;
using GeekShopping.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BaseRabbitMQMessageConsumer<UpdatePaymentResultVO>
{
    private readonly IOrderRepository _repository;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IOrderRepository repository
    ) : base(configuration)
    {
        _repository = repository;
        ConsumeCallback = ProcessOrder;

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, routingKey: "PaymentOrder");
    }

    protected override EventingBasicConsumer ConsumeMessage()
    {
        var consumer = base.ConsumeMessage();

        _channel.BasicConsume(PaymentOrderUpdateQueueName, false, consumer);

        return consumer;
    }

    private async Task ProcessOrder(BaseMessage updatePaymentResultVO)
    {
        try
        {
            await _repository.UpdateOrderPaymentStatus(
                (updatePaymentResultVO as UpdatePaymentResultVO).OrderId,
                (updatePaymentResultVO as UpdatePaymentResultVO).Status
            );
        }
        catch
        {
            throw;
        }
    }
}
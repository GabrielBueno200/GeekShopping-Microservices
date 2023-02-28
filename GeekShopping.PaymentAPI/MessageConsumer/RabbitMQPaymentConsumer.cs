using System.Threading.Tasks;
using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageConsumer;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.MessageSender;
using GeekShopping.PaymentProcessor;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BaseRabbitMQMessageConsumer<PaymentMessage>
{
    private readonly IProcessPayment _processPayment;
    private readonly PaymentMessageSender _messageSender;

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IProcessPayment processPayment,
        PaymentMessageSender messageSender
    ) : base(configuration)
    {
        _processPayment = processPayment;
        _messageSender = messageSender;
        ConsumeCallback = ProcessPayment;

        _channel.QueueDeclare(queue: "order_payment_process_queue", false, false, false, arguments: null);
    }

    protected override EventingBasicConsumer ConsumeMessage()
    {
        var consumer = base.ConsumeMessage();
        _channel.BasicConsume("order_payment_process_queue", false, consumer);

        return consumer;
    }

    private Task ProcessPayment(BaseMessage paymentMessage)
    {
        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage paymentResult = new()
        {
            Status = result,
            OrderId = (paymentMessage as PaymentMessage).OrderId,
            Email = (paymentMessage as PaymentMessage).Email
        };

        try
        {
            _messageSender.SendMessage(paymentResult);
        }
        catch
        {
            throw;
        }

        return Task.CompletedTask;
    }
}
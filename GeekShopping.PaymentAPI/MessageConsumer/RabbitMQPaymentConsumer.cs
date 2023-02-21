using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IProcessPayment _processPayment;
    private readonly IRabbitMQMessageSender _messageSender;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IProcessPayment processPayment,
        IRabbitMQMessageSender messageSender
    )
    {
        _configuration = configuration;
        _processPayment = processPayment;
        _messageSender = messageSender;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "order_payment_process_queue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var paymentMessage = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPayment(paymentMessage).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume("order_payment_process_queue", false, consumer);

        return Task.CompletedTask;
    }

    private Task ProcessPayment(PaymentMessage paymentMessage)
    {
        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage paymentResult = new()
        {
            Status = result,
            OrderId = paymentMessage.OrderId,
            Email = paymentMessage.Email
        };

        try
        {
            _messageSender.SendMessage(paymentResult, "order_payment_result_queue");
        }
        catch
        {
            throw;
        }

        return Task.CompletedTask;
    }
}
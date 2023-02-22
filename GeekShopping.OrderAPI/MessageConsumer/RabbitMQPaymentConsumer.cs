using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GeekShopping.Email.Messages;
using GeekShopping.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _repository;
    private IConnection _connection;
    private IModel _channel;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IOrderRepository repository
    )
    {
        _configuration = configuration;
        _repository = repository;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, routingKey: "PaymentOrder");
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var updatePaymentResultVO = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);
            ProcessOrder(updatePaymentResultVO).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(PaymentOrderUpdateQueueName, false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessOrder(UpdatePaymentResultVO updatePaymentResultVO)
    {
        try
        {
            await _repository.UpdateOrderPaymentStatus(
                updatePaymentResultVO.OrderId,
                updatePaymentResultVO.Status
            );
        }
        catch
        {
            throw;
        }
    }
}
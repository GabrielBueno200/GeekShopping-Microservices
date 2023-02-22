using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GeekShopping.Email.Repository;
using GeekShopping.OrderAPI.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailRepository _repository;
    private IConnection _connection;
    private IModel _channel;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

    public RabbitMQPaymentConsumer(
        IConfiguration configuration,
        IEmailRepository repository
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
        _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, routingKey: "PaymentEmail");
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLogs(message).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessLogs(UpdatePaymentResultMessage message)
    {
        try
        {
            await _repository.LogEmail(message);
        }
        catch
        {
            throw;
        }
    }
}
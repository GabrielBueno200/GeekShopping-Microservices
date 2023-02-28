using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.MessageBus.RabbitMQMessageConsumer;

public abstract class BaseRabbitMQMessageConsumer<TMessage> : BackgroundService
    where TMessage : BaseMessage
{
    protected readonly IConfiguration _configuration;
    protected IConnection _connection;
    protected IModel _channel;
    protected Func<BaseMessage, Task> ConsumeCallback;

    protected BaseRabbitMQMessageConsumer(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected virtual EventingBasicConsumer ConsumeMessage()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var message = JsonSerializer.Deserialize<TMessage>(content);
            ConsumeCallback(message).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        return consumer;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ConsumeMessage();

        return Task.CompletedTask;
    }
}
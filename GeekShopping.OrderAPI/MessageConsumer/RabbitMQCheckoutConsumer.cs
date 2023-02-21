using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _repository;
    private readonly IRabbitMQMessageSender _messageSender;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQCheckoutConsumer(
        IConfiguration configuration,
        IOrderRepository repository,
        IRabbitMQMessageSender messageSender
    )
    {
        _configuration = configuration;
        _repository = repository;
        _messageSender = messageSender;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "checkout_queue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
            ProcessOrder(vo).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume("checkout_queue", false, consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessOrder(CheckoutHeaderVO checkoutHeaderVo)
    {
        var orderHeader = new OrderHeader()
        {
            UserId = checkoutHeaderVo.UserId,
            FirstName = checkoutHeaderVo.FirstName,
            LastName = checkoutHeaderVo.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = checkoutHeaderVo.CardNumber,
            CouponCode = checkoutHeaderVo.CouponCode,
            CVV = checkoutHeaderVo.CVV,
            DiscountAmount = checkoutHeaderVo.DiscountAmount,
            Email = checkoutHeaderVo.Email,
            ExpiryMonthYear = checkoutHeaderVo.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = checkoutHeaderVo.PurchaseAmount,
            PaymentStatus = false,
            Phone = checkoutHeaderVo.Phone,
            DateTime = checkoutHeaderVo.Time
        };

        foreach (var cartDetail in checkoutHeaderVo.CartDetails)
        {
            orderHeader.TotalItens += cartDetail.Count;
            orderHeader.OrderDetails.Add(new OrderDetail()
            {
                ProductId = cartDetail.ProductId,
                ProductName = cartDetail.Product.Name,
                Price = cartDetail.Product.Price,
                Count = cartDetail.Count,
            });
        }

        await _repository.AddOrder(orderHeader);

        var payment = new PaymentVO
        {
            Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.Id,
            PurchaseAmount = orderHeader.PurchaseAmount,
            Email = orderHeader.Email
        };

        try
        {
            _messageSender.SendMessage(payment, "order_payment_process_queue");
        }
        catch
        {
            throw;
        }
    }
}
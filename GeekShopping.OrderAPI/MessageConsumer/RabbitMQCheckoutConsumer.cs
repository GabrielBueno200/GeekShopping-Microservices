using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeekShopping.MessageBus;
using GeekShopping.MessageBus.RabbitMQMessageConsumer;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.MessageSender;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQCheckoutConsumer : BaseRabbitMQMessageConsumer<CheckoutHeaderVO>
{
    private readonly IOrderRepository _repository;
    private readonly OrderMessageSender _messageSender;

    public RabbitMQCheckoutConsumer(
        IConfiguration configuration,
        IOrderRepository repository,
        OrderMessageSender messageSender
    ) : base(configuration)
    {
        _repository = repository;
        _messageSender = messageSender;
        ConsumeCallback = ProcessOrder;

        _channel.QueueDeclare(queue: "checkout_queue", false, false, false, arguments: null);
    }

    protected override EventingBasicConsumer ConsumeMessage()
    {
        var consumer = base.ConsumeMessage();

        _channel.BasicConsume("checkout_queue", false, consumer);

        return consumer;
    }

    private async Task ProcessOrder(BaseMessage message)
    {
        var checkoutHeaderVo = message as CheckoutHeaderVO;

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
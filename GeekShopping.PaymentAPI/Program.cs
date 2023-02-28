using GeekShopping.IoC.DependencyInjection;
using GeekShopping.PaymentAPI.MessageConsumer;
using GeekShopping.PaymentAPI.MessageSender;
using GeekShopping.PaymentProcessor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthConfigs(builder.Configuration);
builder.Services.AddSwaggerConfigs("GeekShopping.PaymentAPI");

builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
builder.Services.AddSingleton<PaymentMessageSender>();
builder.Services.AddHostedService<RabbitMQPaymentConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

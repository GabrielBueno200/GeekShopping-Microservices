using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Repository;
using GeekShopping.OrderAPI.MessageConsumer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GeekShopping.IoC.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbContextOptionsBuilder = builder.Services.AddDatabaseConfigs<MySQLContext>(
    builder.Configuration,
    useDbContextOptionsBuilder: true
);

builder.Services.AddAuthConfigs(builder.Configuration);
builder.Services.AddSwaggerConfigs("GeekShopping.Email");

builder.Services.AddSingleton<IEmailRepository>(new EmailRepository(dbContextOptionsBuilder.Options));
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

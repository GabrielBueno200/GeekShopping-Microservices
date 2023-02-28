using System.Reflection;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GeekShopping.CartAPI.Routes;
using GeekShopping.CartAPI;
using GeekShopping.CartAPI.Repository;
using GeekShopping.IoC.DependencyInjection;
using GeekShopping.CartAPI.MessageSender;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseConfigs<MySQLContext>(builder.Configuration);

builder.Services.AddAuthConfigs(builder.Configuration);
builder.Services.AddSwaggerConfigs("GeekShopping.CartAPI");

builder.Services.AddSingleton(MappingConfigurations.RegisterMaps().CreateMapper());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<CartMessageSender>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSignedHttpClient<ICouponRepository, CouponRepository>(
    baseUrl: builder.Configuration["ServicesUrls:CouponApi"]
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddRoutes();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

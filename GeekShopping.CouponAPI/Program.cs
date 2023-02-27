using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GeekShopping.CouponAPI.Configurations;
using GeekShopping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.Routes;
using GeekShopping.CouponAPI.Repository;
using GeekShopping.IoC.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseConfigs<MySQLContext>(builder.Configuration);

builder.Services.AddAuthConfigs(builder.Configuration);
builder.Services.AddSwaggerConfigs("GeekShopping.CouponAPI");

builder.Services.AddSingleton(MappingConfigurations.RegisterMaps().CreateMapper());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<ICouponRepository, CouponRepository>();

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

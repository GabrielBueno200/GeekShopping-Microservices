using System.Reflection;
using GeekShopping.IoC.DependencyInjection;
using GeekShopping.ProductAPI.Configurations;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseConfigs<MySQLContext>(builder.Configuration);

builder.Services.AddSingleton(MappingConfigurations.RegisterMaps().CreateMapper());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddAuthConfigs(builder.Configuration);
builder.Services.AddSwaggerConfigs("GeekShopping.ProductAPI");

builder.Services.AddScoped<IProductRepository, ProductRepository>();

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

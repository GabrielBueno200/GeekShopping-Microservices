using GeekShopping.ProductAPI.Repository;
using GeekShopping.ProductAPI.Utils;
using GeekShopping.ProductAPI.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Routes
{
    public static class Routes
    {
        private static string BaseRoute = "api/v1/product";

        public static void AddRoutes(this WebApplication app)
        {
            app.MapGet($"{BaseRoute}/get/{{productId}}", async (
                [FromRoute] long productId, [FromServices] IProductRepository repository
            ) =>
            {
                var product = await repository.FindById(productId);
                if (product is null) return Results.NotFound();

                return Results.Ok(product);
            }).RequireAuthorization();

            app.MapGet($"{BaseRoute}/get", async ([FromServices] IProductRepository repository) =>
                Results.Ok(await repository.FindAll())).RequireAuthorization();

            app.MapPost($"{BaseRoute}/save", async ([FromBody] ProductVO product, [FromServices] IProductRepository repository) =>
                Results.Ok(await repository.Create(product))).RequireAuthorization();

            app.MapPut($"{BaseRoute}/update", async ([FromBody] ProductVO product, [FromServices] IProductRepository repository) =>
                Results.Ok(await repository.Update(product))).RequireAuthorization();

            app.MapDelete($"{BaseRoute}/delete/{{productId}}", async ([FromRoute] long productId, [FromServices] IProductRepository repository) =>
            {
                var wasProductDeleted = await repository.Delete(productId);
                if (!wasProductDeleted) return Results.BadRequest(wasProductDeleted);
                return Results.Ok(wasProductDeleted);
            }).RequireAuthorization(Role.Admin);
        }
    }
}
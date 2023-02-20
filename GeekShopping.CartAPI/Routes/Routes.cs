using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeekShopping.CartAPI.Repository;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Routes
{
    public static class Routes
    {
        private static string BaseRoute = "api/v1/cart";

        public static void AddRoutes(this WebApplication app)
        {
            app.MapGet($"{BaseRoute}/find-cart/{{userId}}", async (
                [FromRoute] string userId, 
                [FromServices] ICartRepository repository
            ) =>
            {
                var cart = await repository.FindCartByUserId(userId);
                if (cart is null) return Results.NotFound();
                return Results.Ok(cart);
            });

            app.MapPost($"{BaseRoute}/add-cart", async (
                [FromBody] CartVO cartVo, 
                [FromServices] ICartRepository repository
            ) =>
            {
                var cart = await repository.SaveOrUpdateCart(cartVo);
                if (cart == null) return Results.NotFound();
                return Results.Ok(cart);
            });

            app.MapDelete($"{BaseRoute}/update-cart", async (
                [FromBody] CartVO cartVo, 
                [FromServices] ICartRepository repository
            ) =>
            {
                var cart = await repository.SaveOrUpdateCart(cartVo);
                if (cart == null) return Results.NotFound();
                return Results.Ok(cart);
            });

            app.MapDelete($"{BaseRoute}/remove-cart/{{cartDetailsId}}", async (
                [FromRoute] int cartDetailsId, 
                [FromServices] ICartRepository repository) 
            => {
                var status = await repository.RemoveFromCart(cartDetailsId);
                if (!status) return Results.BadRequest();
                return Results.Ok(status);
            });
        }
    }
}
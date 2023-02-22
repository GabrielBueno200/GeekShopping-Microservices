using System;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeekShopping.CartAPI.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;

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
            =>
            {
                var status = await repository.RemoveFromCart(cartDetailsId);
                if (!status) return Results.BadRequest();
                return Results.Ok(status);
            });

            app.MapPost($"{BaseRoute}/apply-coupon", async (
                [FromBody] CartVO cartVo,
                [FromServices] ICartRepository repository
            ) =>
            {
                var status = await repository.ApplyCoupon(cartVo.CartHeader.UserId, cartVo.CartHeader.CouponCode);

                if (!status) return Results.NotFound();
                return Results.Ok(status);
            });

            app.MapDelete($"{BaseRoute}/remove-coupon/{{userId}}", async (
                [FromRoute] string userId,
                [FromServices] ICartRepository repository
            ) =>
            {
                var status = await repository.RemoveCoupon(userId);

                if (!status) return Results.NotFound();
                return Results.Ok(status);
            });

            app.MapPost($"{BaseRoute}/checkout", async (
                [FromBody] CheckoutHeaderVO checkoutHeaderVo,
                [FromServices] ICartRepository cartRepository,
                [FromServices] ICouponRepository couponRepository,
                [FromServices] IRabbitMQMessageSender messageSender
            ) =>
            {
                if (checkoutHeaderVo.UserId is null) return Results.BadRequest();

                var cart = await cartRepository.FindCartByUserId(checkoutHeaderVo.UserId);

                if (cart is null) return Results.NotFound();

                if (!string.IsNullOrEmpty(checkoutHeaderVo.CouponCode))
                {
                    var coupon = await couponRepository.
                                    GetCouponByCouponCode(checkoutHeaderVo.CouponCode);
                    if (checkoutHeaderVo.DiscountAmount != coupon.DiscountAmount)
                        return Results.Json(statusCode: 412, data: null);
                }

                checkoutHeaderVo.CartDetails = cart.CartDetails;
                checkoutHeaderVo.Time = DateTime.Now;

                messageSender.SendMessage(checkoutHeaderVo, "checkout_queue");

                await cartRepository.ClearCart(checkoutHeaderVo.UserId);

                return Results.Ok(checkoutHeaderVo);
            });
        }
    }
}
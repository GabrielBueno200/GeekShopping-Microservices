using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.OrderAPI.Routes
{
    public static class Routes
    {
        private static string BaseRoute = "api/v1/coupon";

        public static void AddRoutes(this WebApplication app)
        {
            // app.MapGet($"{BaseRoute}/{{couponCode}}", async (
            //     [FromRoute] string couponCode, 
            //     [FromServices] ICouponRepository repository
            // ) =>
            // {
            //     var coupon = await repository.GetCouponByCouponCode(couponCode);
            //     if (coupon is null) return Results.NotFound();

            //     return Results.Ok(coupon);
            // });
        }
    }
}
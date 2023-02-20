using System.Linq;
using System.Threading.Tasks;
using GeekShopping.Web.Controllers.Base;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class CartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(ICartService cartService, ICouponService couponService)
    {
        _cartService = cartService;
        _couponService = couponService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        var cart = await FindUserCart();
        return View(cart);
    }

    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartViewModel cartViewModel)
    {
        var response = await _cartService.ApplyCoupon(cartViewModel);

        if (response) return RedirectToAction(nameof(CartIndex));
        
        return View();
    }

    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        var response = await _cartService.RemoveCoupon(UserId);

        if (response)
        {
            return RedirectToAction(nameof(CartIndex));
        }
        return View();
    }

    public async Task<IActionResult> Remove(int id)
    {
        var response = await _cartService.RemoveFromCart(id);

        if (response) return RedirectToAction(nameof(CartIndex));

        return View();
    }

    private async Task<CartViewModel> FindUserCart()
    {
        var response = await _cartService.FindCartByUserId(UserId);

        if (response?.CartHeader is not null)
        {
            if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode);

                if (coupon?.CouponCode is not null)
                    response.CartHeader.DiscountAmount = coupon.DiscountAmount;
            }

            var totalPrice = response.CartDetails.Sum(detail => detail.Product.Price * detail.Count);

            response.CartHeader.PurchaseAmount = totalPrice - response.CartHeader.DiscountAmount;
        }

        return response;
    }
}
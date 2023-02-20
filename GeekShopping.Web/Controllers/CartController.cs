using System;
using System.Linq;
using System.Threading.Tasks;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GeekShopping.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public CartController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> CartIndex()
    {
        var cart = await FindUserCart();
        return View(cart);
    }

    public async Task<IActionResult> Remove(int id)
    {
        var response = await _cartService.RemoveFromCart(id);

        if (response) return RedirectToAction(nameof(CartIndex));

        return View();
    }

    private async Task<CartViewModel> FindUserCart()
    {
        var userId = User.Claims.Where(claim => claim.Type == "sub")?.FirstOrDefault().Value;

        var response = await _cartService.FindCartByUserId(userId);
        
        if (response?.CartHeader is not null)
            response.CartDetails.ToList().ForEach(detail => 
                response.CartHeader.PurchaseAmount += detail.Product.Price * detail.Count
            );

        return response;
    }
}
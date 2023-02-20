using System.Linq;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using GeekShopping.Web.Controllers.Base;

namespace GeekShopping.Web.Controllers;
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productsService;
    private readonly ICartService _cartService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productsService,
        ICartService cartService
    )
    {
        _logger = logger;
        _productsService = productsService;
        _cartService = cartService;
    }

    [ActionName("Index")]
    public async Task<IActionResult> IndexAsync()
    {
        var products = await _productsService.FindAllProducts();
        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Details(long id)
    {
        var product = await _productsService.FindProductById(id);
        return View(product);
    }

    [HttpPost]
    [ActionName("Details")]
    public async Task<IActionResult> DetailsPost(ProductViewModel productViewModel)
    {
        var cart = new CartViewModel
        {
            CartHeader = new CartHeaderViewModel { UserId = UserId },
            CartDetails = new List<CartDetailViewModel> {
                new()
                {
                    Count = productViewModel.Count,
                    ProductId = productViewModel.Id,
                    Product = await _productsService.FindProductById(productViewModel.Id)
                }
            }
        };

        var response = await _cartService.AddItemToCart(cart);

        if (response is not null) return RedirectToAction("Index");

        return View(productViewModel);
    }

    [Authorize]
    public async Task<IActionResult> Login()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        return RedirectToAction("Index");
    }

    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}

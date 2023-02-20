using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GeekShopping.Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productsService;

    public HomeController(ILogger<HomeController> logger, IProductService productsService)
    {
        _logger = logger;
        _productsService = productsService;
    }

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

    [Authorize]
    public async Task<IActionResult> Login()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        return RedirectToAction(nameof(IndexAsync));
    }
    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}

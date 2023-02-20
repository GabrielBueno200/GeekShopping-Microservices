using System.Threading.Tasks;
using GeekShopping.Web.Controllers.Base;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers;

public class ProductController : BaseController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize]
    public async Task<IActionResult> ProductIndex()
    {
        var products = await _productService.FindAllProducts();
        return View(products);
    }

    public async Task<IActionResult> ProductUpdate(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product is not null) return View(product);
        return NotFound();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductUpdate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProduct(model);
            if (response is not null)
                return RedirectToAction(nameof(ProductIndex));
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> ProductDelete(int id)
    {
        var product = await _productService.FindProductById(id);
        if (product is not null) return View(product);
        return NotFound();        
    }


    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> ProductDelete(ProductViewModel model)
    {
        var response = await _productService.DeleteProductById(model.Id);
        if (response) return RedirectToAction(nameof(ProductIndex));
        return View(model);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ProductCreate(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProduct(model);
            if (response is not null)
                return RedirectToAction(nameof(ProductIndex));
        }

        return View(model);
    }
}

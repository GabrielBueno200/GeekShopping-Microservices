using System;
using System.Net.Http;
using System.Threading.Tasks;
using GeekShopping.Web.Extensions;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;

namespace GeekShopping.Web.Services;

public class CartService : ICartService
{
    private readonly HttpClient _client;

    private const string BaseUrl = "api/v1/cart";

    public CartService(HttpClient client)
    {
        _client = client;
    }

    public async Task<CartViewModel> AddItemToCart(CartViewModel cart)
    {
        var response = await _client.PostAsJson($"{BaseUrl}/add-cart", cart);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartViewModel>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> ApplyCoupon(CartViewModel cart, string couponCode)
    {
        throw new System.NotImplementedException();
    }

    public async Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader)
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new System.NotImplementedException();
    }

    public async Task<CartViewModel> FindCartByUserId(string userId)
    {
        var response = await _client.GetAsync($"{BaseUrl}/find-cart/{userId}");
        return await response.ReadContentAs<CartViewModel>();
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(long cartId)
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/remove-cart/{cartId}");
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<bool>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<CartViewModel> UpdateCart(CartViewModel cart)
    {
        var response = await _client.PutAsJson($"{BaseUrl}/update-cart", cart);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<CartViewModel>();
        else throw new Exception("Something went wrong when calling API");
    }
}

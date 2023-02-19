using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GeekShopping.Web.Extensions;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;

namespace GeekShopping.Web.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _client;

    public ProductService(HttpClient client)
    {
        _client = client;
    }

    private const string BaseUrl =  "api/v1/product";

    public async Task<ProductModel> CreateProduct(ProductModel productModel)
    {
        var response = await _client.PostAsJson($"{BaseUrl}/save", productModel);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductModel>();
        else throw new Exception("Something went wrong when calling API");
    }

    public async Task<bool> DeleteProductById(long id)
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/delete/{id}");
        return await response.ReadContentAs<bool>();
    }

    public async Task<IEnumerable<ProductModel>> FindAllProducts()
    {
        var response = await _client.GetAsync($"{BaseUrl}/get");
        return await response.ReadContentAs<IList<ProductModel>>();
    }

    public async Task<ProductModel> FindProductById(long id)
    {
        var response = await _client.GetAsync($"{BaseUrl}/get/{id}");
        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<ProductModel> UpdateProduct(ProductModel productModel)
    {
        var response = await _client.PutAsJson($"{BaseUrl}/update", productModel);
        if (response.IsSuccessStatusCode)
            return await response.ReadContentAs<ProductModel>();
        else throw new Exception("Something went wrong when calling API");
    }
}

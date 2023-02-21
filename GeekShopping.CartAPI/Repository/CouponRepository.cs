using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient _client;

    public CouponRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<CouponVO> GetCouponByCouponCode(string couponCode)
    {
        var response = await _client.GetAsync($"api/v1/coupon/{couponCode}");
        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode != HttpStatusCode.OK) return new CouponVO();

        return JsonSerializer.Deserialize<CouponVO>(
            content,
            options: new() { PropertyNameCaseInsensitive = true }
        );
    }
}

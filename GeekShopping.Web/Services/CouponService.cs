using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GeekShopping.Web.Extensions;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;

namespace GeekShopping.Web.Services;

public class CouponService : ICouponService
{
    private readonly HttpClient _client;

    private const string BaseUrl =  "api/v1/coupon";

    public CouponService(HttpClient client)
    {
        _client = client;
    }

    public async Task<CouponViewModel> GetCoupon(string couponCode)
    {
        var response = await _client.GetAsync($"{BaseUrl}/{couponCode}");
        if (response.StatusCode is not HttpStatusCode.OK) return new CouponViewModel();
        return await response.ReadContentAs<CouponViewModel>();
    }
}

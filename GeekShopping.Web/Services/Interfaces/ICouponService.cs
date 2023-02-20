using System.Threading.Tasks;
using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces;

public interface ICouponService
{
    Task<CouponViewModel> GetCoupon(string couponCode);
}

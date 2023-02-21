using System.Threading.Tasks;
using GeekShopping.CartAPI.ValueObjects;

namespace GeekShopping.CartAPI.Repository;

public interface ICouponRepository
{
    Task<CouponVO> GetCouponByCouponCode(string couponCode);
}

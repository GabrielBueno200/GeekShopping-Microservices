using System.Threading.Tasks;
using GeekShopping.CouponAPI.ValueObjects;

namespace GeekShopping.CouponAPI.Repository;

public interface ICouponRepository
{
    Task<CouponVO> GetCouponByCouponCode(string couponCode);
}

using System.Threading.Tasks;
using AutoMapper;
using GeekShopping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly MySQLContext _context;
    private readonly IMapper _mapper;

    public CouponRepository(MySQLContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CouponVO> GetCouponByCouponCode(string couponCode)
    {
        var coupon = await _context.Coupons.FirstOrDefaultAsync(coupon => coupon.CouponCode == couponCode);
        return _mapper.Map<CouponVO>(coupon);
    }
}

using AutoMapper;
using GeekShopping.CouponAPI.Model;
using GeekShopping.CouponAPI.ValueObjects;

namespace GeekShopping.CouponAPI.Configurations;

public class MappingConfigurations
{
    public static MapperConfiguration RegisterMaps() => new MapperConfiguration(configuration => {
        configuration.CreateMap<Coupon, CouponVO>().ReverseMap();
    });
}

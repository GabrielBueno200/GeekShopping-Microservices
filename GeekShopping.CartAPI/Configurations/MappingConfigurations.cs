using AutoMapper;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.ValueObjects;

namespace GeekShopping.CartAPI.Configurations;

public class MappingConfigurations
{
    public static MapperConfiguration RegisterMaps() => new MapperConfiguration(configuration =>
    {
        configuration.CreateMap<Product, ProductVO>().ReverseMap();
        configuration.CreateMap<CartHeader, CartHeaderVO>().ReverseMap();
        configuration.CreateMap<CartDetail, CartDetailVO>().ReverseMap();
        configuration.CreateMap<Cart, CartVO>().ReverseMap();
    });
}

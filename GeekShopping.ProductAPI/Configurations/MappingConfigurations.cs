using AutoMapper;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.ValueObjects;

namespace GeekShopping.ProductAPI.Configurations;

public class MappingConfigurations
{
    public static MapperConfiguration RegisterMaps() => new MapperConfiguration(configuration => {
        configuration.CreateMap<Product, ProductVO>().ReverseMap();
    });
}

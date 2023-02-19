using System.Collections.Generic;
using System.Threading.Tasks;
using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductModel>> FindAllProducts();
    Task<ProductModel> FindProductById(long id);
    Task<ProductModel> CreateProduct(ProductModel productModel);
    Task<ProductModel> UpdateProduct(ProductModel productModel);
    Task<bool> DeleteProductById(long id);
}

using System.Collections.Generic;
using System.Threading.Tasks;
using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> FindAllProducts();
    Task<ProductViewModel> FindProductById(long id);
    Task<ProductViewModel> CreateProduct(ProductViewModel productModel);
    Task<ProductViewModel> UpdateProduct(ProductViewModel productModel);
    Task<bool> DeleteProductById(long id);
}

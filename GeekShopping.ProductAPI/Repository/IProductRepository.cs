using System.Collections.Generic;
using System.Threading.Tasks;
using GeekShopping.ProductAPI.ValueObjects;

namespace GeekShopping.ProductAPI.Repository;

public interface IProductRepository
{
    Task<IEnumerable<ProductVO>> FindAll();
    Task<ProductVO> FindById(long productId);
    Task<ProductVO> Create(ProductVO productVO);
    Task<ProductVO> Update(ProductVO productVO);
    Task<bool> Delete(long productId);
}

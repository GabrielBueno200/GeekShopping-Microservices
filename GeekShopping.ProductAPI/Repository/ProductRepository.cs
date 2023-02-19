using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly MySQLContext _context;
    private readonly IMapper _mapper;

    public ProductRepository(MySQLContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductVO> Create(ProductVO productVO)
    {
        var mappedProduct = _mapper.Map<Product>(productVO);
        _context.Products.Add(mappedProduct);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductVO>(mappedProduct);
    }

    public async Task<bool> Delete(long productId)
    {
        try 
        {
            var product = await _context.Products.FirstAsync(product => product.Id == productId);
            if (product is null) return false;
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<ProductVO>> FindAll()
    {
        var products = await _context.Products.ToListAsync();
        return _mapper.Map<IList<ProductVO>>(products);
    }

    public async Task<ProductVO> FindById(long productId)
    {
        var product = await _context.Products.FirstAsync(product => product.Id == productId);
        return _mapper.Map<ProductVO>(product);
    }

    public async Task<ProductVO> Update(ProductVO productVO)
    {
        var mappedProduct = _mapper.Map<Product>(productVO);
        _context.Products.Update(mappedProduct);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductVO>(mappedProduct);
    }
}

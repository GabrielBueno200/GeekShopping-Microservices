using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Repository;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI;

public class CartRepository : ICartRepository
{
    private readonly MySQLContext _context;
    private readonly IMapper _mapper;

    public CartRepository(MySQLContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await _context.CartHeaders
                        .FirstOrDefaultAsync(cartHeader => cartHeader.UserId == userId);

        if (cartHeader is not null)
        {
            _context.CartDetails.RemoveRange(
                _context.CartDetails.Where(cartHeader => cartHeader.CartHeaderId == cartHeader.Id)
            );
            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        var cart = new Cart
        {
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId),
        };

        cart.CartDetails = _context.CartDetails
                            .Where(cartDetail => cartDetail.CartHeaderId == cart.CartHeader.Id)
                            .Include(cartDetail => cartDetail.Product);

        return _mapper.Map<CartVO>(cart);
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        try
        {
            var cartDetail = await _context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetailsId);

            var total = _context.CartDetails
                        .Where(cartDetail => cartDetail.CartHeaderId == cartDetail.CartHeaderId)
                        .Count();

            _context.CartDetails.Remove(cartDetail);

            if (total == 1)
            {
                var cartHeaderToRemove = await _context.CartHeaders
                    .FirstOrDefaultAsync(cartHeader => cartHeader.Id == cartDetail.CartHeaderId);
                _context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CartVO> SaveOrUpdateCart(CartVO cartVo)
    {
        var cart = _mapper.Map<Cart>(cartVo);

        //Checks if the product is already saved in the database if it does not exist then save
        var product = await _context.Products.FirstOrDefaultAsync(
            product => product.Id == cartVo.CartDetails.FirstOrDefault()!.ProductId
        );

        if (product is null)
        {
            _context.Products.Add(cart.CartDetails.First().Product);
            await _context.SaveChangesAsync();
        }

        //Check if CartHeader is null
        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
            cartHeader => cartHeader.UserId == cart.CartHeader.UserId
        );

        if (cartHeader is null)
        {
            //Create CartHeader and CartDetails
            _context.CartHeaders.Add(cart.CartHeader);
            await _context.SaveChangesAsync();

            cart.CartDetails.First().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.First().Product = null;
            _context.CartDetails.Add(cart.CartDetails.First());
            await _context.SaveChangesAsync();
        }
        else
        {
            //If CartHeader is not null
            //Check if CartDetails has same product
            var cartDetail = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                cartDetail => cartDetail.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                              cartDetail.CartHeaderId == cartHeader.Id
            );

            if (cartDetail is null)
            {
                //Create CartDetails
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                //Update product count and CartDetails
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
        }

        return _mapper.Map<CartVO>(cart);
    }
}

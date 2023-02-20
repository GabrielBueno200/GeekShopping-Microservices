using System.Threading.Tasks;
using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces;

public interface ICartService
{
    Task<CartViewModel> FindCartByUserId(string userId);
    Task<CartViewModel> AddItemToCart(CartViewModel cart);
    Task<CartViewModel> UpdateCart(CartViewModel cart);
    Task<bool> RemoveFromCart(long cartDetailsId);
    Task<bool> ApplyCoupon(CartViewModel cart);
    Task<bool> RemoveCoupon(string userId);
    Task<bool> ClearCart(string userId);
    Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader);
}

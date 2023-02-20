namespace GeekShopping.CartAPI.Model;

public class CartVO
{
    public CartHeaderVO CartHeader { get; set; }

    public IEnumerable<CartDetailVO> CartDetails { get; set; }
}

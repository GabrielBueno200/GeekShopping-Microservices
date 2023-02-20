using System.ComponentModel.DataAnnotations.Schema;
using GeekShopping.CartAPI.Model.Base;

namespace GeekShopping.CartAPI.Model;

public class CartDetail : BaseEntity
{
    public long CartHeaderId { get; set; }
    
    [ForeignKey(nameof(CartHeaderId))]
    public CartHeader CartHeader { get; set; }
    
    public long ProductId { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }

    [Column("count")]
    public int Count { get; set; }
}

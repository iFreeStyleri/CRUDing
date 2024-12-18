using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;

namespace CRUDing.Domain.Responses.Cart
{
    public record class GetCartProductResponse
    {
        public List<ProductCartResponse> Products { get; set; }
        public Money TotalCost { get; set; }
        public GetCartProductResponse(List<CartProduct> products, Money totalCost)
        {
            Products = new();
            products.ForEach(f => Products.Add(new ProductCartResponse
            {
                Id = f.Id,
                ProductId = f.ProductId,
                CartId = f.CartId,
                Count = f.Count,
                ProductName = f.Product.Name,
                Cost = f.Product.Cost
            }));
            TotalCost = totalCost;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;

namespace CRUDing.Domain.Responses.Order
{
    public record class GetOrderWithProductsResponse
    {
        public int Id { get; set; }
        public List<ProductItem> Products { get; set; }
        public Money TotalCost { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<DateTime> Completed { get; set; }
        public DateTime Created { get; set; }
        public GetOrderWithProductsResponse(Entities.Order order)
        {
            IsCompleted = order.IsCompleted;
            TotalCost = new Money(order.Products.First().CurrentCost.Currency,
                order.Products.Select(s => s.CurrentCost.Value * s.Count).Sum());
            Products = order.Products;
            Id = order.Id;
            Completed = order.Completed;
            Created = order.Created;
        }
    }
}

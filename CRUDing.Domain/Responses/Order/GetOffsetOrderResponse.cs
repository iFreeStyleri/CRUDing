using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;

namespace CRUDing.Domain.Responses.Order
{
    public record class GetOffsetOrderResponse
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public Nullable<DateTime> Completed { get; set; }
        public int ProductCount { get; set; }
        public Money TotalCost { get; set; }
        public GetOffsetOrderResponse(Entities.Order order)
        {
            Id = order.Id;
            Created = order.Created;
            Completed = order.Completed;
            ProductCount = order.Products.Count;
            TotalCost = new Money( order.Products.First().CurrentCost.Currency ,order.Products.Select(s => s.CurrentCost.Value * s.Count).Sum());
        }
    }
}

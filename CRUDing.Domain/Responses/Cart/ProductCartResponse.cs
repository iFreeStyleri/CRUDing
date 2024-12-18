using CRUDing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Responses.Cart
{
    public record class ProductCartResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CartId { get; set; }
        public int Count { get; set; }
        public Money Cost { get; set; }

    }
}

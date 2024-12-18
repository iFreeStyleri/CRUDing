using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;

namespace CRUDing.Domain.Entities
{
    public class ProductItem : Entity
    {
        public Order Order { get; set; }
        public Product Product { get; set; }
        public Money CurrentCost { get; set; }
        public int Count { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;

namespace CRUDing.Domain.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public List<CartProduct> Carts { get; set; }
        public Money Cost { get; set; }
    }
}

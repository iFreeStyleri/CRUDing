using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;

namespace CRUDing.Domain.Entities
{
    public class Cart : Entity
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public List<CartProduct> Products { get; set; }
    }
}

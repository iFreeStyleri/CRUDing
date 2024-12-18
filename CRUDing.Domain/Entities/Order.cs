using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;

namespace CRUDing.Domain.Entities
{
    public class Order : Entity
    {
        public User User { get; set; }
        public List<ProductItem> Products { get; set; }
        public DateTime Created { get; set; }
        public Nullable<DateTime> Completed { get; set; }
        public bool IsCompleted { get; set; }
    }
}

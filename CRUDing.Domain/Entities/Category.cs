using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Entities;

namespace CRUDing.Domain.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}

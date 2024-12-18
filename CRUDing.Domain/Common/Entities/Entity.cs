using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.Common.Entities
{
    public class Entity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}

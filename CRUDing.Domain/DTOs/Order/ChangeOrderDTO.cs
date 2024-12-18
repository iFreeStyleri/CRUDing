using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Order
{
    public class ChangeOrderDTO
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
    }
}

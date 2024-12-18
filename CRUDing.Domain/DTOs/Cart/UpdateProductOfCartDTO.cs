using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Cart
{
    public record class UpdateProductOfCartDTO(int productId, int count);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Entities;

namespace CRUDing.Domain.DTOs.Products
{
    public record class AddProductDTO(string Name, string Description, int categoryId, Money Cost);
}

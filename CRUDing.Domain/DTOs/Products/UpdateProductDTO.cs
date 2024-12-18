using CRUDing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Products
{
    public record class UpdateProductDTO(int Id,string Name, string Description, int categoryId, Money Cost);

}

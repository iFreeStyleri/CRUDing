using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDing.Domain.DTOs.Category
{
    public record class UpdateCategoryDTO(int Id, string Name);
}

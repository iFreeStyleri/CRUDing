using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Category;
using CRUDing.Domain.Entities;

namespace CRUDing.Core.Abstractions.Services
{
    public interface ICategoryService
    {
        public Task<IBaseResponse<Category>> GetCategory(int id);
        public Task<IBaseResponse<object>> AddCategory(AddCategoryDTO category);
        public Task<IBaseResponse<List<Category>>> GetOffsetCategory(int page);
        public Task<IBaseResponse<Category>> RemoveCategory(int id);
        Task<IBaseResponse<Category>> UpdateCategory(UpdateCategoryDTO category);
    }
}

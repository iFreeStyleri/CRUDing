using AutoMapper;
using CRUDing.Domain.DTOs.Category;
using CRUDing.Domain.DTOs.Products;
using CRUDing.Domain.Entities;

namespace CRUDing.API.Mapping
{
    public class CategoryProfileMap : Profile
    {
        public CategoryProfileMap()
        {
            CreateMap<AddCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();
        }
    }
}

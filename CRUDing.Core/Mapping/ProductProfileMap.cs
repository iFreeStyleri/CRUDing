using AutoMapper;
using CRUDing.Domain.DTOs.Products;
using CRUDing.Domain.Entities;

namespace CRUDing.API.Mapping
{
    public class ProductProfileMap : Profile
    {
        public ProductProfileMap()
        {
            CreateMap<AddProductDTO, Product>()
                .ForMember(
                    dest => dest.Category,
                    opt => opt.MapFrom(src => new Category{ Id = src.categoryId}));
            CreateMap<UpdateProductDTO, Product>()
                .ForMember(
                    dest => dest.Category,
                    opt => opt.MapFrom(src => new Category {Id = src.categoryId}));
        }
    }
}

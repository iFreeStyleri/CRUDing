using AutoMapper;
using CRUDing.Domain.DTOs.Users;
using CRUDing.Domain.Entities;

namespace CRUDing.API.Mapping
{
    public class UserProfileMap : Profile
    {
        public UserProfileMap()
        {
            CreateMap<AuthorizeDTO, User>();
            CreateMap<RegisterDTO, User>();
        }
    }
}

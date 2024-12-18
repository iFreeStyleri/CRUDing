using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Jwt;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Users;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Responses.User;

namespace CRUDing.Core.Abstractions.Services
{
    public interface IUserService
    {
        public Task<IBaseResponse<GetUserInfoResponse>> GetUserAsync(string email);
        public Task<IBaseResponse<JwtTokens>> AuthorizeAsync(AuthorizeDTO user, string ip);
        public Task<IBaseResponse<JwtTokens>> RegisterAsync(RegisterDTO user, string ip);
        public Task<IBaseResponse<JwtTokens>> ReAuthAsync(string refreshToken, string ip);
    }
}

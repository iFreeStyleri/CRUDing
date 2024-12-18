using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDing.Domain.Common.Jwt;
using CRUDing.Domain.Entities.Enums;

namespace CRUDing.Core.Abstractions.Services
{
    public interface IJwtService
    {
        public AccessToken GetAccessToken(string email, string ip, Role role);
        public Task<JwtTokens> GetNewTokensAsync(string oldRefreshToken, string ip);
        public Task<RefreshToken> GetRefreshTokenAsync(string ip, Role role, string email);
    }
}

using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Core.Exceptions;
using CRUDing.Domain.Common.Jwt;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRUDing.Core.Implementations.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _lifetime;
        private readonly int _refreshTokenLifetime;
        private readonly IDistributedCache _redisCache;
        private readonly IBaseRepository<User> _userRepository;

        public JwtService(IDistributedCache redisCache, IConfiguration config, IBaseRepository<User> userRepository)
        {
            _redisCache = redisCache;
            _userRepository = userRepository;
            var jwtSection = config.GetSection("JwtOptions");
            _issuer = jwtSection["ISSUER"];
            _audience = jwtSection["AUDIENCE"];
            _key = jwtSection["KEY"];
            _lifetime = int.Parse(jwtSection["LIFETIME"]);
            _refreshTokenLifetime = int.Parse(jwtSection["RefreshTokenLifetime"]);
        }

        public AccessToken GetAccessToken(string email, string ip, Role role)
        {
            var identity = GetIdentity(email, ip, role);
            var expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_lifetime));
            var jwt = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: identity.Claims,
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key)),
                    SecurityAlgorithms.HmacSha256)  
            );
            return new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                Expires = expires,
            };
        }

        public async Task<JwtTokens> GetNewTokensAsync(string oldRefreshToken, string ip)
        {
            var oldData = JsonConvert.DeserializeObject<JwtTokenData>(await _redisCache.GetStringAsync(oldRefreshToken));
            if (oldData == null)
                throw new JwtTokenException("Токен истёк");
            else if (oldData.Ip != ip)
            {
                await _redisCache.RemoveAsync(oldRefreshToken);
                throw new JwtTokenException("Доступ запрещён");
            }
            await _redisCache.RemoveAsync(oldRefreshToken);
            var user = await _userRepository.GetAll().SingleOrDefaultAsync(s => oldData.Email == s.Email);
            if (user.IsDeleted)
                throw new JwtTokenException("Доступ запрещён");
            var refreshToken = await GetRefreshTokenAsync(oldData.Ip, oldData.Role, oldData.Email);
            var accessToken = GetAccessToken(oldData.Email, oldData.Ip, oldData.Role);

            return new JwtTokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public async Task<RefreshToken> GetRefreshTokenAsync(string ip, Role role, string email)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(_refreshTokenLifetime)
            };
            var tokenData = new JwtTokenData
            {
                Email = email,
                Ip = ip,
                Role = role
            };

            await _redisCache.SetStringAsync(refreshToken.Token, JsonConvert.SerializeObject(tokenData) , new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_refreshTokenLifetime)
            });
            return refreshToken;
        }

        private ClaimsIdentity GetIdentity(string email, string ip, Role role)
        {
            var claims = new List<Claim>
            {
                new (ClaimsIdentity.DefaultNameClaimType, email),
                new (ClaimsIdentity.DefaultNameClaimType, ip),
                new (ClaimsIdentity.DefaultRoleClaimType, Enum.GetName(role))
            };
            return new ClaimsIdentity(claims, "AccessToken", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        }

        private string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}

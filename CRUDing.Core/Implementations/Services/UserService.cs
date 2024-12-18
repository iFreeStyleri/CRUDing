using AutoMapper;
using CRUDing.Core.Abstractions.Repositories;
using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.Common.Jwt;
using CRUDing.Domain.Common.Response;
using CRUDing.Domain.DTOs.Users;
using CRUDing.Domain.Entities;
using CRUDing.Domain.Entities.Enums;
using CRUDing.Domain.Responses.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CRUDing.Core.Implementations.Services
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<AuthorizeDTO> _authValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IJwtService jwtService, 
            IBaseRepository<User> userRepository,
            IBaseRepository<Order> orderRepository,
            IValidator<RegisterDTO> registerValidator,
            IValidator<AuthorizeDTO> authValidator,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _registerValidator = registerValidator;
            _authValidator = authValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IBaseResponse<GetUserInfoResponse>> GetUserAsync(string email)
        {
            try
            {
                _logger.LogInformation("Получение информации о пользователе: {email}", email);
                var user = await _userRepository.GetAll().SingleOrDefaultAsync(s => s.Email == email);

                if (user == null)
                {
                    _logger.LogWarning($"Пользователь: {email} не найден");
                    return new BaseResponse<GetUserInfoResponse>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                var countOrder = await _orderRepository.GetAll()
                    .Include(i => i.User)
                    .Where(w => w.User.Email == email)
                    .CountAsync();
                _logger.LogInformation($"Найдено: {countOrder} заказов у {email}");
                var countCompletedOrder = await _orderRepository.GetAll()
                    .Include(i => i.User)
                    .Where(w => w.User.Email == email && w.IsCompleted)
                    .CountAsync();
                _logger.LogInformation($"Найдено: {countCompletedOrder} завершённых заказов у {email}");

                return new BaseResponse<GetUserInfoResponse>
                {
                    Code = HttpStatusCode.OK,
                    Data = new GetUserInfoResponse(user, countOrder, countCompletedOrder)
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<GetUserInfoResponse>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<JwtTokens>> AuthorizeAsync(AuthorizeDTO dto, string ip)
        {
            try
            {
                _logger.LogInformation("Авторизация пользователя {@dto}", dto);
                var validResult = await _authValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {@errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<JwtTokens>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };
                }
                _logger.LogInformation("Поиск соли пользователя {@dto}", dto);
                var saltUser = await _userRepository.GetAll().SingleOrDefaultAsync(s => dto.Email == s.Email);
                var user = _mapper.Map<User>(dto);
                user.Password = GetHashPassword(user.Password, saltUser.Salt);
                var result = await _userRepository.GetAll()
                    .SingleOrDefaultAsync(s => s.Email == user.Email && s.Password == user.Password);
                if (result == null)
                {
                    _logger.LogWarning($"Пользователь не найден: {dto.Email} {dto.Password}");
                    return new BaseResponse<JwtTokens>
                    {
                        Code = HttpStatusCode.NotFound,
                        Message = "User not found"
                    };
                }

                var accessToken = _jwtService.GetAccessToken(result.Email, ip, result.Role);
                var refreshToken = await _jwtService.GetRefreshTokenAsync(ip, user.Role, user.Email);
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.OK,
                    Data = new JwtTokens {AccessToken = accessToken, RefreshToken = refreshToken}
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<JwtTokens>> RegisterAsync(RegisterDTO dto, string ip)
        {
            try
            {
                _logger.LogInformation("Регистрация пользователя {@dto}", dto);
                var validResult = await _registerValidator.ValidateAsync(dto);
                if (!validResult.IsValid)
                {
                    _logger.LogWarning(
                        "Неправильный запрос: {@dto}\n {@errors}", dto, validResult.Errors.Select(s => s.ErrorMessage));
                    return new BaseResponse<JwtTokens>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = string.Join('\n', validResult.Errors.Select(s => s.ErrorMessage))
                    };
                }
                var user = _mapper.Map<User>(dto);
                user.Role = Role.User;
                user.Created = DateTime.UtcNow;
                user.Salt = GenerateSalt();
                user.Password = GetHashPassword(user.Password, user.Salt);
                user.Cart = new Cart{ User = user};
                var result = await _userRepository.GetAll().SingleOrDefaultAsync(s => s.Email == user.Email);
                if (result != null)
                    return new BaseResponse<JwtTokens>
                    {
                        Code = HttpStatusCode.BadRequest,
                        Message = "User already exists"
                    };

                var accessToken = _jwtService.GetAccessToken(user.Email, ip, user.Role);
                var refreshToken = await _jwtService.GetRefreshTokenAsync(ip, user.Role, user.Email);
                await _userRepository.AddAsync(user);
                _logger.LogInformation($"Пользователь зарегистрирован {dto.Email}");
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.OK,
                    Data = new JwtTokens {AccessToken = accessToken, RefreshToken = refreshToken}
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<JwtTokens>> ReAuthAsync(string refreshToken, string ip)
        {
            try
            {
                _logger.LogInformation("Получение нового токена...");
                var newJwtTokens = await _jwtService.GetNewTokensAsync(refreshToken, ip);
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.OK,
                    Data = newJwtTokens
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Внутренняя ошибка:", ex);
                return new BaseResponse<JwtTokens>
                {
                    Code = HttpStatusCode.InternalServerError
                };
            }
        }

        private string GetHashPassword(string password, string salt)
        {
            password += salt;
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
            return Encoding.ASCII.GetString(hash);
        }

        private string GenerateSalt()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    }
}

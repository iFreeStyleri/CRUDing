using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CRUDing.API.Controllers
{
    [Route("/api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO request)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Получен {ip}");
            var response = await _userService.RegisterAsync(request, ip);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{ip} получил токены доступа");
                    return Ok(response);
                case HttpStatusCode.NotFound:
                    return NotFound();
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest(response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> AuthorizeAsync(AuthorizeDTO request)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Получен {ip}");
            var response = await _userService.AuthorizeAsync(request, ip);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{ip} получил токены доступа");
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [HttpPatch("refresh-token")]
        public async Task<IActionResult> RecreateRefreshToken(string refreshToken)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Получен {ip}");
            var response = await _userService.ReAuthAsync(refreshToken, ip);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{ip} получил токены доступа");
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _userService.GetUserAsync(userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation($"{userEmail} получил информацию о своём аккаунте");
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }
    }
}

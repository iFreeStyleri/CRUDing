using System.Net;
using CRUDing.Core.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CRUDing.Domain.DTOs.Order;

namespace CRUDing.API.Controllers
{
    [Route("/api")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] Nullable<int> page, [FromQuery] bool IsCompleted, [FromQuery] Nullable<int> id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            if (page.HasValue && IsCompleted)
            {
                var response = await _orderService.GetOffsetCompletedOrders(page.Value, userEmail);
                switch (response.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(500);
                    default:
                        return BadRequest();
                }
            }
            else if (page.HasValue)
            {
                var response = await _orderService.GetOffsetOrders(page.Value, userEmail);
                switch (response.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(500);
                    default:
                        return BadRequest();
                }
            }
            else if (id.HasValue)
            {
                var response = await _orderService.GetOrderWithProducts(id.Value, userEmail);
                switch (response.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.InternalServerError:
                        return StatusCode(500);
                    default:
                        return BadRequest();
                }
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder()
        {
            
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _orderService.CreateOrder(userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                case HttpStatusCode.NotFound:
                    return NotFound();
                default:
                    return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("orders")]
        public async Task<IActionResult> ChangeOrder(ChangeOrderDTO dto)
        {
            var response = await _orderService.ChangeStatusOrder(dto.Id, dto.IsCompleted);
            switch (response.Code)
            {
                case HttpStatusCode.NotFound:
                    return NotFound(response);
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("orders")]
        public async Task<IActionResult> RemoveOrder(int orderId)
        {
            var response = await _orderService.RemoveOrder(orderId);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                case HttpStatusCode.NotFound:
                    return NotFound(response);
                default:
                    return BadRequest();
            }
        }
    }
}

using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.DTOs.Cart;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CRUDing.API.Controllers
{
    [Route("/api/")]
    [Authorize]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IValidator<UpdateProductOfCartDTO> _updateProductValidator;

        public CartController(ICartService cartService, IValidator<UpdateProductOfCartDTO> updateProductValidator)
        {
            _cartService = cartService;
            _updateProductValidator = updateProductValidator;
        }


        [HttpGet("carts")]
        [Authorize]
        public async Task<IActionResult> GetCartOffset(int page)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _cartService.GetCartsOffset(page, userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                case HttpStatusCode.NotFound:
                    return NotFound(response);
                default:
                    return BadRequest();
            }
        }

        [HttpPost("carts")]
        [Authorize]
        public async Task<IActionResult> AddProductOfCart(AddProductOfCartDTO request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _cartService.AddProductCart(request.productId, userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("carts")]
        public async Task<IActionResult> RemoveProductOfCart(RemoveProductOfCartDTO request)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _cartService.RemoveProductCart(request.productId, userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [HttpPatch("carts")]
        [Authorize]
        public async Task<IActionResult> UpdateProductOfCart(UpdateProductOfCartDTO request)
        {
            var resultValidator = await _updateProductValidator.ValidateAsync(request);
            if (!resultValidator.IsValid)
                return BadRequest(resultValidator.Errors.Select(s => s.ErrorMessage));
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _cartService.UpdateProductOfCart(request.productId, request.count, userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

        [HttpDelete("carts/clear")]
        [Authorize]
        public async Task<IActionResult> ClearProductsOfCart()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userEmail = identity.Claims.First().Value;
            var response = await _cartService.ClearProductsOfCart(userEmail);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                case HttpStatusCode.InternalServerError:
                    return StatusCode(500);
                default:
                    return BadRequest();
            }
        }

    }
}

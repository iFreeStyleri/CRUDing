using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CRUDing.API.Controllers
{
    [Route("/api/")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(Nullable<int> id, Nullable<int> page, string? categoryName)
        {
            if (id.HasValue && !page.HasValue)
            {
                
                var result = await _productService.GetProduct(id.Value);
                switch (result.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(result);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(result);
                    default:
                        return BadRequest();
                }
            }
            else if (page.HasValue && !id.HasValue)
            {
                var result = await _productService.GetOffsetProduct(page.Value);
                switch (result.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(result);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(result);
                    default:
                        return BadRequest();
                }
            }
            else if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var result = await _productService.GetOffsetProductByCategory(page.HasValue ? page.Value : 0, categoryName);
                switch (result.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(result);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(result);
                    default:
                        return BadRequest();
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("products")]
        public async Task<IActionResult> AddProducts(AddProductDTO request)
        {
            var response = await _productService.AddProduct(request);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                default:
                    return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("products")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            var response = await _productService.RemoveProduct(id);
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
        [HttpPut("products")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDTO request)
        {
            var response = await _productService.UpdateProduct(request);
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
    }
}

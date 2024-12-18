using CRUDing.Core.Abstractions.Services;
using CRUDing.Domain.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CRUDing.API.Controllers
{
    [Route("/api/")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategory([FromQuery] Nullable<int> id, [FromQuery] Nullable<int> page)
        {
            if (id.HasValue && !page.HasValue)
            {
                var response = await _categoryService.GetCategory(id.Value);
                switch (response.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(response);
                    default:
                        return BadRequest();
                }
            }
            else if (page.HasValue && !id.HasValue)
            {
                var response = await _categoryService.GetOffsetCategory(page.Value);
                switch (response.Code)
                {
                    case HttpStatusCode.OK:
                        return Ok(response);
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest(response);
                    default:
                        return BadRequest();
                }
            }

            return NotFound();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory(AddCategoryDTO request)
        {
            var response = await _categoryService.AddCategory(request);
            switch (response.Code)
            {
                case HttpStatusCode.OK:
                    return Ok(response);
                case HttpStatusCode.BadRequest:
                    return BadRequest(response);
                default:
                    return BadRequest();
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("categories")]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var response = await _categoryService.RemoveCategory(id);
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

        [Authorize(Roles = "Admin")]
        [HttpPut("categories")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDTO request)
        {
            var response = await _categoryService.UpdateCategory(request);
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
    }
}

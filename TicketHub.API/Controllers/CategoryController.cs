using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TicketHub.API.DTOs.Category;
using TicketHub.API.Pagination;
using TicketHub.API.Services.Interfaces;

namespace TicketHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IValidator<CategoryPostDto> _postValidator;
        private readonly IValidator<CategoryPutDto> _putValidator;

        public CategoryController(ICategoryService service,
            IValidator<CategoryPostDto> postValidator,
            IValidator<CategoryPutDto> putValidator)
        {
            _service = service;
            _postValidator = postValidator;
            _putValidator = putValidator;
        }

        [HttpGet]
        [OutputCache]
        public async Task<ActionResult<PagedList<CategoryDto>>> GetCategories([FromQuery] PaginationParams pParams)
        {
            var pCategories = await _service.GetCategories(pParams);
            return Ok(pCategories);
        }

        [HttpGet("{id}")]
        [OutputCache]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var category = await _service.GetCategory(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryPostDto categoryPostDto)
        {
            var validation = await _postValidator.ValidateAsync(categoryPostDto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var category = await _service.CreateCategory(categoryPostDto);

            if (category == null)
                return Conflict();

            return CreatedAtAction(nameof(GetCategory), new { Id = category.Id }, category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateCategory(CategoryPutDto categoryPutDto)
        {
            var validation = await _putValidator.ValidateAsync(categoryPutDto);

            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            var result = await _service.UpdateCategory(categoryPutDto);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
                return BadRequest("The id must be greater than zero");

            var result = await _service.DeleteCategory(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
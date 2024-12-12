using Course_platform.Models.DTO;
using Course_platform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Course_platform.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly CoursePlatformDbContext _context;

        public CategoryController(CoursePlatformDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            if (await _context.Categories.AnyAsync(c => c.CategoryName == categoryDTO.CategoryName))
            {
                return Conflict("Category with this name already exists.");
            }

            var category = new CategoryEntity
            {
                CategoryName = categoryDTO.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return Ok(categories);
        }

        [Authorize]
        [HttpGet("course/{courseId}/categories")]
        public async Task<IActionResult> GetCategoriesForCourse(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Categories) 
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var categories = course.Categories.Select(c => new { c.CategoryId, c.CategoryName }).ToList();

            return Ok(categories);
        }


    }
}

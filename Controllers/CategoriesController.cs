using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPP.Models;
using Microsoft.AspNetCore.Authorization;
using MyWebAPP.Authorization;

namespace MyWebAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly LibraryContext _context;

        public CategoriesController(LibraryContext context)
        {
            _context = context;
        }

        [HttpPost]
        [CheckPermission(Permission.Add)]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddCategory), new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        [CheckPermission(Permission.Delete)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        // [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
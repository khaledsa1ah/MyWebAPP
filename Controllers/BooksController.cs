using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebAPP.Models;
using MyWebAPP.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWebAPP.Authorization;

namespace MyWebAPP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return books;
        }

        [HttpGet("{isbn}")]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<Book>> GetBookByISBN(string isbn)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        [CheckPermission(Permission.Add)]
        public async Task<ActionResult<Book>> AddBook(BookDTO bookDTO)
        {
            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                return BadRequest("Invalid author ID.");
            }

            var category = await _context.Categories.FindAsync(bookDTO.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid category ID.");
            }

            var book = new Book
            {
                Title = bookDTO.Title,
                ISBN = bookDTO.ISBN,
                AuthorId = bookDTO.AuthorId,
                CategoryId = bookDTO.CategoryId,
                Category = category,
                Author = author
                // You can add other properties here as needed
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookByISBN), new { isbn = book.ISBN }, book);
        }

        [HttpGet("author/id/{authorId}")]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(int authorId)
        {
            var books = await _context.Books
                .Where(b => b.AuthorId == authorId)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return books;
        }

        [HttpGet("author/name/{authorName}")]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string authorName)
        {
            var books = await _context.Books
                .Where(b => b.Author.Name == authorName)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return books;
        }

        [HttpGet("category/id/{categoryId}")]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId)
        {
            var books = await _context.Books
                .Where(b => b.CategoryId == categoryId)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return books;
        }

        [HttpGet("category/name/{categoryName}")]
        [CheckPermission(Permission.Read)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(string categoryName)
        {
            var books = await _context.Books
                .Where(b => b.Category.Name == categoryName)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();

            return books;
        }

        [HttpPut("{isbn}")]
        [CheckPermission(Permission.Edit)]
        public async Task<IActionResult> UpdateBook(string isbn, BookDTO bookDTO)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

            if (book == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                return BadRequest("Invalid author ID.");
            }

            var category = await _context.Categories.FindAsync(bookDTO.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid category ID.");
            }

            book.Title = bookDTO.Title;
            book.ISBN = bookDTO.ISBN;
            book.AuthorId = bookDTO.AuthorId;
            book.CategoryId = bookDTO.CategoryId;
            book.Category = category;
            book.Author = author;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{isbn}")]
        [CheckPermission(Permission.Delete)]
        public async Task<IActionResult> DeleteBook(string isbn)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

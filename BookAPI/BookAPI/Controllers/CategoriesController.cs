using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IBookRepository bookRepository;

        public CategoriesController(ICategoryRepository categoryRepository,
            IBookRepository bookRepository)
        {
            this.categoryRepository = categoryRepository;
            this.bookRepository = bookRepository;
        }

        // /api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = categoryRepository.GetCategories().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoriesDto);
        }

        // /api/categories/categoryId
        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var category = categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        // /api/categories/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetAllCategoriesForBook(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var categories = categoryRepository.GetAllCategoriesForBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoryDto.Add(new CategoryDto { 
                    Id = category.Id,
                    Name = category.Name
                });
            }

            return Ok(categoryDto);
        }

        // /api/categories/categoryId/books
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetAllBooksForCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var books = categoryRepository.GetAllBooksForCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach (var book in books)
            {
                booksDto.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished
                });
            }

            return Ok(booksDto);
        }

        // /api/categories
        [HttpPost]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Category))] //created
        public IActionResult CreateCategory([FromBody] Category creatingCategory)
        {
            if (creatingCategory == null)
                return BadRequest(ModelState);
            
            var category = categoryRepository.GetCategories().Where(c => c.Name.Trim().ToUpper() ==
                creatingCategory.Name.Trim().ToUpper()).FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {creatingCategory.Name} already exists.");
                return StatusCode(422, ModelState);
            }
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!categoryRepository.CreateCategory(creatingCategory))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't create {creatingCategory.Name}.");
                return StatusCode(500, ModelState);
            }
            
            return CreatedAtRoute("GetCategory", new { categoryId = creatingCategory.Id }, creatingCategory);
        }

        // /api/categories/categoryId
        [HttpPut("{categoryId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updatingCategory)
        {
            if (updatingCategory == null)
                return BadRequest(ModelState);

            if (categoryId != updatingCategory.Id)
                return BadRequest(ModelState);

            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();
            
            if (categoryRepository.IsDuplicateCategoryName(categoryId, updatingCategory.Name))
            {
                ModelState.AddModelError("", $"Category {updatingCategory.Name} already exists.");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!categoryRepository.UpdateCategory(updatingCategory))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't update {updatingCategory.Name}.");
                return StatusCode(500, ModelState);
            }
            
            return NoContent();
        }

        // /api/categories/categoryId
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(409)] // conflict 
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!categoryRepository.CategoryExist(categoryId))
                return NotFound();

            var deletingCategory = categoryRepository.GetCategory(categoryId);

            if (categoryRepository.GetAllBooksForCategory(categoryId).Count() > 0)
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingCategory.Name}."
                    + " It is used by at least one book.");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (!categoryRepository.DeleteCategory(deletingCategory))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingCategory.Name}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
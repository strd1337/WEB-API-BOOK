using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IReviewRepository reviewRepository;

        public BooksController(IBookRepository bookRepository,
            IAuthorRepository authorRepository, 
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.categoryRepository = categoryRepository;
            this.reviewRepository = reviewRepository;
        }
        
        // /api/books
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            var books = bookRepository.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach (var book in books)
            {
                booksDto.Add(new BookDto
                {
                    Id = book.Id,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished,
                    Title = book.Title
                });
            }

            return Ok(booksDto);
        }

        // /api/books/isbn/bookIsbn
        [HttpGet("ISBN/{bookIsbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(string bookIsbn)
        {
            if (!bookRepository.BookExist(bookIsbn))
                return NotFound();

            var book = bookRepository.GetBook(bookIsbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                DatePublished = book.DatePublished,
                Title = book.Title,
                Isbn = book.Isbn,
            };

            return Ok(bookDto);
        }

        // /api/books/bookId
        [HttpGet("{bookId}", Name = "GetBook")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var book = bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                DatePublished = book.DatePublished,
                Title = book.Title,
                Isbn = book.Isbn,
            };

            return Ok(bookDto);
        }

        // /api/books/bookId/rating
        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var rating = bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }


        // /api/books?authorsId=1&authorId=2&categoriesId=1&categoryId=2
        [HttpPost]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Book))] //created
        public IActionResult CreateBook([FromQuery] List<int> authorsId,
            [FromQuery] List<int> categoriesId, [FromBody] Book creatingBook)
        {
            var statusCode = ValidateBook(authorsId, categoriesId, creatingBook);

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!bookRepository.CreateBook(authorsId, categoriesId, creatingBook))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't create {creatingBook.Title}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetBook", new { bookId = creatingBook.Id }, creatingBook);
        }

        // /api/books/bookId?authorsId=1&authorId=2&categoriesId=1&categoryId=2
        [HttpPut("{bookId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // no content
        public IActionResult UpdateBook(int bookId, [FromQuery] List<int> authorsId,
            [FromQuery] List<int> categoriesId, [FromBody] Book updatingBook)
        {
            var statusCode = ValidateBook(authorsId, categoriesId, updatingBook);

            if (bookId != updatingBook.Id)
                return BadRequest();

            if (!bookRepository.BookExist(bookId))
                return NotFound();

            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);

            if (!bookRepository.UpdateBook(authorsId, categoriesId, updatingBook))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't update {updatingBook.Title}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // /api/book/bookId
        [HttpDelete("{bookId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteBook(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var deletingReviews = reviewRepository.GetReviewsOfBook(bookId).ToList();
            var deletingBook = bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (deletingReviews.Count() > 1 && !reviewRepository.DeleteReviews(deletingReviews))
            { 
                ModelState.AddModelError("", $"Something weng wrong. Can't delete reviews.");
                return StatusCode(500, ModelState);
            }
            
            if (!bookRepository.DeleteBook(deletingBook))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete book {deletingBook.Title}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private StatusCodeResult ValidateBook(List<int> authorsId,
            List<int> categoriesId, Book book)
        {
            if (book == null || authorsId.Count() <= 0 || categoriesId.Count() <= 0)
            {
                ModelState.AddModelError("", "Missing book, category and author.");
                return BadRequest();
            }

            if (bookRepository.IsDuplicateIsbn(book.Id, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN.");
                return StatusCode(422);
            }

            foreach (var authorId in authorsId)
            {
                if (!authorRepository.AuthorExist(authorId))
                {
                    ModelState.AddModelError("", "Author not found.");
                    return StatusCode(404);
                }
            }

            foreach (var categoryId in categoriesId)
            {
                if (!categoryRepository.CategoryExist(categoryId))
                {
                    ModelState.AddModelError("", "Category not found.");
                    return StatusCode(404);
                }
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical error.");
                return BadRequest();
            }

            return NoContent();
        }
    }
}

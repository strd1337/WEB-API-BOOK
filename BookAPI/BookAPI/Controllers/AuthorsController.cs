using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IBookRepository bookRepository;
        private readonly ICountryRepository countryRepository;

        public AuthorsController(IAuthorRepository authorRepository,
            IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            this.authorRepository = authorRepository;
            this.bookRepository = bookRepository;
            this.countryRepository = countryRepository;
        }

        // /api/authors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            var authors = authorRepository.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        // /api/authors/authorId
        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!authorRepository.AuthorExist(authorId))
                return NotFound();

            var author = authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            return Ok(authorDto);
        }

        // /api/authors/authorId/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!authorRepository.AuthorExist(authorId))
                return NotFound();

            var books = authorRepository.GetBooksByAuthor(authorId);

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

        // /api/authors/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsOfBook(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var authors = authorRepository.GetAuthorsOfBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorsDto.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorsDto);
        }

        // /api/authors
        [HttpPost]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Author))] //created
        public IActionResult CreateAuthor([FromBody] Author creatingAuthor)
        {
            if (creatingAuthor == null)
                return BadRequest(ModelState);

            if (!countryRepository.CountryExist(creatingAuthor.Country.Id))
            {
                ModelState.AddModelError("", "Country does not exist.");
                return StatusCode(404, ModelState);
            }

            creatingAuthor.Country = countryRepository.GetCountry(creatingAuthor.Country.Id);
         
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.CreateAuthor(creatingAuthor))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't create {creatingAuthor.FirstName}"
                    + $" {creatingAuthor.LastName}.");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = creatingAuthor.Id }, creatingAuthor);
        }


        // /api/authors/authorId
        [HttpPut("{authorId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult UpdateAuthor(int authorId, [FromBody] Author updatingAuthor)
        {
            if (updatingAuthor == null)
                return BadRequest(ModelState);

            if (authorId != updatingAuthor.Id)
                return BadRequest(ModelState);

            if (!authorRepository.AuthorExist(authorId))
                ModelState.AddModelError("", "Author does not exist.");

            if (!countryRepository.CountryExist(updatingAuthor.Country.Id))
                ModelState.AddModelError("", "Country does not exist.");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            updatingAuthor.Country = countryRepository.GetCountry(updatingAuthor.Country.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.UpdateAuthor(updatingAuthor))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't update {updatingAuthor.FirstName}"
                    + $" {updatingAuthor.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // /api/authors/authorId
        [HttpDelete("{authorId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(409)] // conflict 
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!authorRepository.AuthorExist(authorId))
                return NotFound();

            var deletingAuthor = authorRepository.GetAuthor(authorId);

            if (authorRepository.GetBooksByAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingAuthor.FirstName}"
                    + $" {deletingAuthor.LastName}. It is used by at least one book.");
                return StatusCode(409, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!authorRepository.DeleteAuthor(deletingAuthor))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingAuthor.FirstName}"
                    + $" {deletingAuthor.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

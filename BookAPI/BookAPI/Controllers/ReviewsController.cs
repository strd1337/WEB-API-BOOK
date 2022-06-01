using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : Controller
    {
        private readonly IReviewerRepository reviewerRepository;
        private readonly IReviewRepository reviewRepository;
        private readonly IBookRepository bookRepository;

        public ReviewsController(IReviewerRepository reviewerRepository,
            IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            this.reviewerRepository = reviewerRepository;
            this.reviewRepository = reviewRepository;
            this.bookRepository = bookRepository;
        }

        // /api/reviews
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = reviewRepository.GetReviews();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                });
            }

            return Ok(reviewsDto);
        }

        // /api/reviews/reviewId
        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview(int reviewId)
        {
            if (!reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var review = reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewDto = new ReviewDto()
            {
                Id = review.Id,
                Headline = review.Headline,
                Rating = review.Rating,
                ReviewText= review.ReviewText
            };

            return Ok(reviewDto);
        }

        // /api/reviews/books/bookId
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsOfBook(int bookId)
        {
            if (!bookRepository.BookExist(bookId))
                return NotFound();

            var reviews = reviewRepository.GetReviewsOfBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
            {
                reviewsDto.Add(new ReviewDto
                {
                    Id = review.Id,
                    Headline = review.Headline,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText
                });
            }

            return Ok(reviewsDto);
        }
        
        // /api/reviews/reviewId/book
        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookOfReview(int reviewId)
        {
            if (!reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var book = reviewRepository.GetBookOfReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                DatePublished = book.DatePublished
            };

            return Ok(bookDto);
        }

        // /api/reviews
        [HttpPost]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Review))] //created
        public IActionResult CreateReview([FromBody] Review creatingReview)
        {
            if (creatingReview == null)
                return BadRequest(ModelState);

            if (!reviewerRepository.ReviewerExist(creatingReview.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer does not exist.");

            if (!bookRepository.BookExist(creatingReview.Book.Id))
                ModelState.AddModelError("", "Book does not exist.");

            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            creatingReview.Book = bookRepository.GetBook(creatingReview.Book.Id);
            creatingReview.Reviewer = reviewerRepository.GetReviewer(creatingReview.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.CreateReview(creatingReview))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't create the review.");
                return StatusCode(500, ModelState);
            }
            
            return CreatedAtRoute("GetReview", new { reviewId = creatingReview.Id }, creatingReview);
        }

        // /api/reviews/reviewId
        [HttpPut("{reviewId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult UpdateReview(int reviewId, [FromBody] Review updatingReview)
        {
            if (updatingReview == null)
                return BadRequest(ModelState);
            
            if (reviewId != updatingReview.Id)
                return BadRequest(ModelState);

            if (!reviewRepository.ReviewExist(reviewId))
                ModelState.AddModelError("", "Review does not exist.");

            if (!reviewerRepository.ReviewerExist(updatingReview.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer does not exist.");

            if (!bookRepository.BookExist(updatingReview.Book.Id))
                ModelState.AddModelError("", "Book does not exist.");
            
            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            updatingReview.Book = bookRepository.GetBook(updatingReview.Book.Id);
            updatingReview.Reviewer = reviewerRepository.GetReviewer(updatingReview.Reviewer.Id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.UpdateReview(updatingReview))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't update the review.");
                return StatusCode(500, ModelState);
            }
           
            return NoContent();
        }

        // /api/reviews/reviewId
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(422)]
        [ProducesResponseType(409)] // conflict 
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteCountry(int reviewId)
        {
           if (!reviewRepository.ReviewExist(reviewId))
                return NotFound();
           
            var deletingReview = reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.DeleteReview(deletingReview))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete the review.");
                return StatusCode(500, ModelState);
            }
           
            return NoContent();
        }
    }
}

using BookAPI.Dtos;
using BookAPI.Models;
using BookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewersController : Controller
    {
        private readonly IReviewerRepository reviewerRepository;
        private readonly IReviewRepository reviewRepository;

        public ReviewersController(IReviewerRepository reviewerRepository,
            IReviewRepository reviewRepository)
        {
            this.reviewerRepository = reviewerRepository;
            this.reviewRepository = reviewRepository;
        }

        // /api/reviewers
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewersDto = new List<ReviewerDto>();
            foreach (var reviewer in reviewers)
            {
                reviewersDto.Add(new ReviewerDto
                {
                    Id = reviewer.Id,
                    FirstName = reviewer.FirstName,
                    LastName = reviewer.LastName
                });
            }

            return Ok(reviewersDto);
        }

        // /api/reviewers/reviewerId
        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExist(reviewerId))
                return NotFound();

            var reviewer = reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        // /api/reviewers/reviewerId/reviews
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExist(reviewerId))
                return NotFound();

            var reviews = reviewerRepository.GetReviewsByReviewer(reviewerId);

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

        // /api/reviewers/reviewId/reviewer
        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfReview(int reviewId)
        {
            if (!reviewRepository.ReviewExist(reviewId))
                return NotFound();

            var reviewer = reviewerRepository.GetReviewerOfReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto()
            {
                Id = reviewer.Id,
                FirstName = reviewer.FirstName,
                LastName = reviewer.LastName
            };

            return Ok(reviewerDto);
        }

        // /api/reviewers
        [HttpPost]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(Reviewer))] //created
        public IActionResult CreateReviewer([FromBody] Reviewer creatingReviewer)
        {
             if (creatingReviewer == null)
                 return BadRequest(ModelState);

             if (!ModelState.IsValid)
                 return BadRequest(ModelState);

             if (!reviewerRepository.CreateReviewer(creatingReviewer))
             {
                 ModelState.AddModelError("", $"Something weng wrong. Can't create {creatingReviewer.FirstName}"
                     + $" {creatingReviewer.LastName}.");
                 return StatusCode(500, ModelState);
             }

             return CreatedAtRoute("GetReviewer", new { reviewerId = creatingReviewer.Id }, creatingReviewer);
        }

        // /api/reviewers/reviewerId
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer updatingReviewer)
        {
             if (updatingReviewer == null)
                 return BadRequest(ModelState);
        
             if (reviewerId != updatingReviewer.Id)
                 return BadRequest(ModelState);

             if (!reviewerRepository.ReviewerExist(reviewerId))
                 return NotFound();

             if (!ModelState.IsValid)
                 return BadRequest(ModelState);

             if (!reviewerRepository.UpdateReviewer(updatingReviewer))
             {
                ModelState.AddModelError("", $"Something weng wrong. Can't update {updatingReviewer.FirstName}"
                   + $" {updatingReviewer.LastName}.");
                return StatusCode(500, ModelState);
             }
            
            return NoContent();
        }

        // /api/reviewers/reviewerId
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] //no content
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExist(reviewerId))
                return NotFound();

            var deletingReviewer = reviewerRepository.GetReviewer(reviewerId);
            var deletingReviews = reviewerRepository.GetReviewsByReviewer(reviewerId).ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewerRepository.DeleteReviewer(deletingReviewer))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete {deletingReviewer.FirstName}"
                   + $" {deletingReviewer.LastName}.");
                return StatusCode(500, ModelState);
            }

            if (!reviewRepository.DeleteReviews(deletingReviews))
            {
                ModelState.AddModelError("", $"Something weng wrong. Can't delete reviews by"
                   + $" {deletingReviewer.FirstName} {deletingReviewer.LastName}.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

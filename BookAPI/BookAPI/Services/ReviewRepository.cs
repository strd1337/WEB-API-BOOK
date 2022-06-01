using BookAPI.Models;

namespace BookAPI.Services
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BookDbContext reviewContext;

        public ReviewRepository(BookDbContext reviewContext)
        {
            this.reviewContext = reviewContext;
        }

        public bool CreateReview(Review review)
        {
            reviewContext.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            reviewContext.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            reviewContext.RemoveRange(reviews);
            return Save();
        }

        public Book GetBookOfReview(int reviewId)
        {
            var bookId = reviewContext.Reviews.Where(r => r.Id == reviewId)
                .Select(b => b.Book.Id).FirstOrDefault();
            return reviewContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Review GetReview(int reviewId)
        {
            return reviewContext.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return reviewContext.Reviews.OrderBy(r => r.Rating).ToList();
        }

        public ICollection<Review> GetReviewsOfBook(int bookId)
        {
            return reviewContext.Reviews.Where(b => b.Book.Id == bookId).ToList();
        }

        public bool ReviewExist(int reviewId)
        {
            return reviewContext.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            var isSaved = reviewContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            reviewContext.Update(review);
            return Save();
        }
    }
}

using BookAPI.Models;

namespace BookAPI.Services
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly BookDbContext reviewerContext;

        public ReviewerRepository(BookDbContext reviewerContext)
        {
            this.reviewerContext = reviewerContext;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            reviewerContext.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            reviewerContext.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return reviewerContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public Reviewer GetReviewerOfReview(int reviewId)
        {
            var reviewerId = reviewerContext.Reviews.Where(r => r.Id == reviewId)
                .Select(r => r.Reviewer.Id).FirstOrDefault();
            return reviewerContext.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return reviewerContext.Reviewers.OrderBy(r => r.LastName).ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return reviewerContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExist(int reviewerId)
        {
            return reviewerContext.Reviewers.Any(r => r.Id == reviewerId);
        }

        public bool Save()
        {
            var isSaved = reviewerContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            reviewerContext.Update(reviewer);
            return Save();
        }
    }
}

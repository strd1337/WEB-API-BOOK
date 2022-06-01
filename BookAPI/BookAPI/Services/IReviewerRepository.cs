using BookAPI.Models;

namespace BookAPI.Services
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();
        Reviewer GetReviewer(int reviewerId);
        ICollection<Review> GetReviewsByReviewer(int reviewerId);
        Reviewer GetReviewerOfReview(int reviewId);
        bool ReviewerExist(int reviewerId);
        bool CreateReviewer(Reviewer reviewer);
        bool UpdateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);
        bool Save();
    } 
}

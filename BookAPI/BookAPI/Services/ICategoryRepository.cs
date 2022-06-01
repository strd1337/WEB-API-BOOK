using BookAPI.Models;

namespace BookAPI.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Category> GetAllCategoriesForBook(int bookId);
        ICollection<Book> GetAllBooksForCategory(int categoryId);
        bool CategoryExist(int categoryId);
        bool IsDuplicateCategoryName(int categoryId, string categoryName);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}

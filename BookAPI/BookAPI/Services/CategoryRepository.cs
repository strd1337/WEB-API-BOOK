using BookAPI.Models;

namespace BookAPI.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BookDbContext categoryContext;

        public CategoryRepository(BookDbContext categoryContext)
        {
            this.categoryContext = categoryContext;
        }

        public bool CategoryExist(int categoryId)
        {
            return categoryContext.Categories.Any(c => c.Id == categoryId);
        }

        public ICollection<Book> GetAllBooksForCategory(int categoryId)
        {
            return categoryContext.BookCategories.Where(bc => bc.CategoryId == categoryId)
                .Select(b => b.Book).ToList();
        }

        public ICollection<Category> GetCategories()
        {
            return categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Category> GetAllCategoriesForBook(int bookId)
        {
            return categoryContext.BookCategories.Where(bc => bc.BookId == bookId)
                .Select(c => c.Category).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return categoryContext.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            var category = categoryContext.Categories.Where(c => c.Name.Trim().ToUpper() ==
                 categoryName.Trim().ToUpper() && c.Id != categoryId).FirstOrDefault();
            return category == null ? false : true;
        }

        public bool CreateCategory(Category category)
        {
            categoryContext.Add(category);
            return Save();
        }

        public bool UpdateCategory(Category category)
        {
            categoryContext.Update(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            categoryContext.Remove(category);
            return Save();
        }

        public bool Save()
        {
            var isSaved = categoryContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }
    }
}

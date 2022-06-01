using BookAPI.Models;

namespace BookAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext bookContext;

        public BookRepository(BookDbContext bookContext)
        {
            this.bookContext = bookContext;
        }

        public bool BookExist(int bookId)
        {
            return bookContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExist(string bookIsbn)
        {
            return bookContext.Books.Any(b => b.Isbn == bookIsbn);
        }

        public bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            foreach (var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                bookContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                bookContext.Add(bookCategory);
            }

            bookContext.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            bookContext.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId)
        {
            return bookContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string bookIsbn)
        {
            return bookContext.Books.Where(b => b.Isbn == bookIsbn).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var review = bookContext.Reviews.Where(r => r.Book.Id == bookId);
            
            if (review.Count() < 1)
                return 0;
            
            return ((decimal)review.Sum(r => r.Rating) / review.Count());           
        }

        public ICollection<Book> GetBooks()
        {
            return bookContext.Books.OrderBy(b => b.Id).ToList();
        }

        public bool IsDuplicateIsbn(int bookId, string bookIsbn)
        {
            var book = bookContext.Books.Where(b => b.Isbn.Trim().ToUpper() == 
                bookIsbn.Trim().ToUpper() && b.Id != bookId).FirstOrDefault();
            return book == null ? false : true;
        }

        public bool Save()
        {
            var isSaved = bookContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }

        public bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();
            
            var deleteBookAuthors = bookContext.BookAuthors.Where(ba => ba.BookId == book.Id).ToList();
            var deleteBookCategories = bookContext.BookCategories.Where(bc => bc.CategoryId == book.Id).ToList();

            bookContext.RemoveRange(deleteBookCategories);
            bookContext.RemoveRange(deleteBookAuthors);

            foreach (var author in authors)
            {
                var bookAuthor = new BookAuthor()
                {
                    Author = author,
                    Book = book
                };
                bookContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory()
                {
                    Category = category,
                    Book = book
                };
                bookContext.Add(bookCategory);
            }

            bookContext.Update(book);

            return Save();
        }
    }
}

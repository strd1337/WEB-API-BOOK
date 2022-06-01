using BookAPI.Models;

namespace BookAPI.Services
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        Book GetBook(string bookIsbn);
        decimal GetBookRating(int bookId);
        bool BookExist(int bookId);
        bool BookExist(string bookIsbn);
        bool IsDuplicateIsbn(int bookId, string bookIsbn);
        bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book);
        bool DeleteBook(Book book);
        bool Save();
    }
}

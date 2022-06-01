using BookAPI.Models;

namespace BookAPI.Services
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAuthors();
        Author GetAuthor(int authorId);
        ICollection<Author> GetAuthorsOfBook(int bookId);
        ICollection<Book> GetBooksByAuthor(int authorId);
        bool AuthorExist(int authorId);
        bool CreateAuthor(Author author);
        bool UpdateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();
    }
}

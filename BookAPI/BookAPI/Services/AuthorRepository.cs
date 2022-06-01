using BookAPI.Models;

namespace BookAPI.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookDbContext authorContext;

        public AuthorRepository(BookDbContext authorContext)
        {
            this.authorContext = authorContext;
        }

        public bool AuthorExist(int authorId)
        {
            return authorContext.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Author author)
        {
            authorContext.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            authorContext.Remove(author);
            return Save();
        }

        public Author GetAuthor(int authorId)
        {
            return authorContext.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthors()
        {
            return authorContext.Authors.OrderBy(a => a.LastName).ToList();
        }

        public ICollection<Author> GetAuthorsOfBook(int bookId)
        {
            return authorContext.BookAuthors.Where(ba => ba.Book.Id == bookId)
                .Select(a => a.Author).ToList();
        } 

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return authorContext.BookAuthors.Where(ba => ba.Author.Id == authorId)
                .Select(b => b.Book).ToList();
        }

        public bool Save()
        {
            var isSaved = authorContext.SaveChanges();
            return isSaved > 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            authorContext.Update(author);
            return Save();
        }
    }
}

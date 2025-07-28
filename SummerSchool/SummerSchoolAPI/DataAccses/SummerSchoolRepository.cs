using Microsoft.EntityFrameworkCore;
using SummerSchoolAPI.DataAccses.Interfaces;

namespace SummerSchoolAPI.DataAccses
{
    public class SummerSchoolRepository : ISummerSchoolRepository
    {
        private SummerSchoolDbContext _context;
        public SummerSchoolRepository(SummerSchoolDbContext context)
        {
            _context = context;
        }
        public List<Book> GetBooks()
        {
            return _context.Books.ToList();
        }

        public Book AddBook(Book request)
        {
            _context.Entry(request).State = EntityState.Added;
            _context.SaveChanges();
            return request;
        }
        public Book UpdateBook(int id, Book updatedBook)
        {
            var putBook = _context.Books.Find(id);
            if (putBook != null)
            {
                return null;
            }
            putBook.Title = updatedBook.Title;
            putBook.PublishDte = updatedBook.PublishDte;
            _context.Entry(putBook).State = EntityState.Modified;
            _context.SaveChanges();
            return updatedBook;

        }
        public Book DeleteBook(int id, Book request)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return null;
            }
            _context.Entry(book).State = EntityState.Deleted;
            _context.SaveChanges();
            return book;
        }
    }
}

using SummerSchoolAPI.DataAccses;

namespace SummerSchoolAPI.Business.Interfaces
{
    public interface ISummerSchoolService
    {
        List<Book> GetBooks();
        Book AddBook(Book request);
        Book UpdateBook(int id, Book updatedBook);
        Book DeleteBook(int id, Book deleteBook);

    }
}

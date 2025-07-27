using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SummerSchoolAPI.DataAccses.Interfaces
{
    public interface ISummerSchoolRepository
    {
        List<Book> GetBooks();
         Book AddBook(Book request);
         Book UpdateBook(int id, Book updatedBook);
        Book DeleteBook(int id, Book request);
    }
}

using SummerSchoolAPI.Business.Interfaces;
using SummerSchoolAPI.DataAccses;
using SummerSchoolAPI.DataAccses.Interfaces;

namespace SummerSchoolAPI.Business.Services
{
    public class SummerSchoolService : ISummerSchoolService
    {
        private ISummerSchoolRepository _repository;
        public SummerSchoolService(ISummerSchoolRepository repository)
        {
            _repository = repository;

        }
        public List<Book> GetBooks()
        {
            return _repository.GetBooks();
        }
        public Book AddBook(Book request)
        {
            return _repository.AddBook(request);
        }
        public Book UpdateBook(int id, Book updatedBook)
        {
            return _repository.UpdateBook(id, updatedBook);
        }
        public Book DeleteBook(int id, Book deletedBook)
        {
            return _repository.DeleteBook(id, deletedBook);
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using SummerSchoolAPI.Business.Interfaces;
using SummerSchoolAPI.DataAccses;
using SummerSchoolAPI.Presentation.DTO;
namespace SummerSchoolAPI.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private ISummerSchoolService _summerSchoolServices;
        public BookController(ISummerSchoolService summerSchoolServices)
        {
            _summerSchoolServices = summerSchoolServices;
        }
        [HttpGet()]
        [Route("list")]
        public IActionResult GetAll()
        {
            var bookList = _summerSchoolServices.GetBooks();
            // Book -> BookGetDto dönüşümü

            var bookDtos = bookList.Select(b => new BookGetDto
            {
                Id = b.Id,
                Title = b.Title,
                PublishDte = b.PublishDte
            }).ToList();
            return Ok(bookList);

        }
      

        [HttpPost]
        public IActionResult Add([FromBody] BookCreateDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newBook = new Book
            {
                Title = bookDto.Title,
                PublishDte = bookDto.PublishDate
            };
            var createdBook = _summerSchoolServices.AddBook(newBook);

            var responseDto = new BookGetDto
            {
                Id = createdBook.Id,
                Title = createdBook.Title,
                PublishDte = createdBook.PublishDte
            };
            
            return CreatedAtAction(nameof(GetAll), new { id = responseDto.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateBook bookDto)
        {
            var updatedBook = _summerSchoolServices.UpdateBook(id,new Book 
            {
            Id =bookDto.Id,
            Title = bookDto.Title,
                PublishDte = bookDto.PublishDte
            });
            if (updatedBook == null)
            { 
                return NotFound("Kitap Bulunamadı veya güncellenemedi");
            }
            var responseDto = new BookGetDto
            {
                Id = updatedBook.Id,
                Title = updatedBook.Title,
                PublishDte = updatedBook.PublishDte
            };
            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            var deleteBook =_summerSchoolServices.DeleteBook(id, null);
            if(deleteBook == null)
            {
                return NotFound("Kitap Bulunamadı veya silinemedi");
            }
            var responseDto = new BookGetDto
            {
                Id = deleteBook.Id,
                Title = deleteBook.Title,
                PublishDte = deleteBook.PublishDte
            };
            return Ok(deleteBook);
    }

}

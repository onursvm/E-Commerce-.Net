using Microsoft.AspNetCore.Mvc;

namespace SummerSchoolAPI.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {

        [HttpGet(Name ="çıkart")]
        public int çıkart(int a, int b)
        {
            return a - b;
        }

        [HttpPost(Name ="")]
        public String Post(String message)
        {
            return "Mesajınız: " + message;
        }
        [HttpPut(Name ="Put")]
        public string Put(String newMessage)
        {
            return "Güncellenen Mesajınız: " + newMessage;
        }
        
        [HttpDelete]
        public string Delete() => "Son mesaj silindi";

    }
}

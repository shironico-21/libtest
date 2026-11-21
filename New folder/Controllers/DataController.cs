using Microsoft.AspNetCore.Mvc;

namespace finalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        [HttpGet]
        public IActionResult GetData()
        {
            var data = new
            {
                Message = "Hello from the DataController!",
                Timestamp = DateTime.UtcNow
            };
            return Ok(data);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

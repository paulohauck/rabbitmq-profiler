using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ_Profiller.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class ConfigurationController : Controller
    {
        public IActionResult GetIndex()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ_Profiller.Controllers
{
    public class ConfigurationController : Controller
    {
        public IActionResult GetIndex()
        {
            return View();
        }
    }
}

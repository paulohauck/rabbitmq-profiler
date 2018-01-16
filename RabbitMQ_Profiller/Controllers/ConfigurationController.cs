using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace RabbitMQ_Profiller.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class ConfigurationController : Controller
    {
        public IActionResult GetIndex()
        {
            var o = new JObject();
            return Ok(o);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Central_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        [HttpGet("resources")]
        public IActionResult GetResources()
        {
            var resources = new List<object>
            {
                new { Type = "CSS", Url = "https://localhost:7283/css/style.css" },
                new { Type = "JS", Url = "https://localhost:7283/js/main.js" },
                new { Type = "Icon", Url = "https://localhost:7283/img/logo.png" }
            };
            return Ok(resources);
        }
    }
}

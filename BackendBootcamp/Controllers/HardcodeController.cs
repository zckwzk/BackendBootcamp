using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBootcamp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HardcodeController : ControllerBase
    {
        private static string frontEndurl = "";
        public HardcodeController(IConfiguration configuration)
        {
            frontEndurl = configuration["fronturl"];
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok(frontEndurl);
        }

        [HttpGet]
        [Route("GetHardCode")]
        public ActionResult GetNew()
        {
            return StatusCode(200, "test get hardcode");
        }
    }
}

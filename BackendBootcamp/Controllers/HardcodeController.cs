using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBootcamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardcodeController : ControllerBase
    {
        [HttpGet]
        public ActionResult Test()
        {
            return Ok("Test Get");
        }

        [HttpGet]
        [Route("GetHardCode")]
        public ActionResult GetNew()
        {
            return StatusCode(200, "test get hardcode");
        }
    }
}

using BackendBootcamp.Logics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBootcamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpGet]
        [Route("SendEmail")]
        public IActionResult sendEmail()
        {
            try
            {
                EmailLogic.SendEmail("zakaria.wicaksono@gmail.com", "Test Subject", "<h3>test h3</h3>");

                return Ok("Success kirim email");
            }
            catch(Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}

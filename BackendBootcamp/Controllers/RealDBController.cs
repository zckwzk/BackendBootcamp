using BackendBootcamp.Logics;
using BackendBootcamp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace BackendBootcamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealDBController : ControllerBase
    {
        

        [HttpGet]
        [Route("GetProductReader")]
        public ActionResult GetProductReader([FromQuery] string? name, [FromHeader] string? Authorization)
        {
            try
            {
                var isAuth = JwtTokenLogic.ValidateJwtToken(Authorization);

                if(isAuth == null)
                {
                    return StatusCode(401, "Not Authorized");
                }

                // validate role to access this api
                string role = isAuth.First(x => x.Type == "role").Value;
                if (role != "admin" && role != "user")
                {
                    return StatusCode(401, "Your roles is not high enough to access this!");
                }


                List<object> result = new List<object>();
                result = RealDBLogic.GetProductReader(name);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProducAdapter")]
        //[Authorize(Roles = "admin,user")]
        [Authorize]
        public ActionResult GetProductAdapter([FromQuery] string? name)
        {
            try
            {
                //claim data
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                string currentUser = Convert.ToString(identity.FindFirst("name").Value);
                
                // get product
                List<Product> result = new List<Product>();
                result = RealDBLogic.GetProductAdapter(name);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertProduct")]
        public ActionResult InserProduct([FromBody] Product body)
        {
            try
            {
                RealDBLogic.InsertProduct(body);

                return StatusCode(201,"created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetScalar")]
        public ActionResult GetScalar()
        {
            try
            {
               
                string result = RealDBLogic.ContohScalar();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("RequestTest")]
        public ActionResult RequestTest([FromBody] Product body)
        {
            try
            {
                return Ok(body);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertUserWithProduct")]
        public ActionResult InserUserProduct([FromBody] UserWithProducs body)
        {
            try
            {
                RealDBLogic.InsertUserWithProduct(body);

                return Ok("success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

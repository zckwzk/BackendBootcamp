using BackendBootcamp.Logics;
using BackendBootcamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BackendBootcamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private static string baseUrl = "";
        public LoginController(IConfiguration configuration) {
            baseUrl = configuration["fronturl"];
        }

        [HttpPost]
        public ActionResult Login([FromBody] UserDTO body)
        {
            try
            {
                if (String.IsNullOrEmpty(body.name))
                {
                    throw new Exception("username cant be empty");
                }

                if (String.IsNullOrEmpty(body.password))
                {
                    throw new Exception("password cant be empty");
                }

                string query = "Select top 1 * from users where [name] = @name";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = body.name ?? "" }
                };

                DataTable users = CRUD.ExecuteQuery(query, sqlParams);

                if(users.Rows.Count == 0)
                {
                    throw new Exception("User not found");
                }

                // initialize user variable from database
                byte[] passwordHash = Array.Empty<byte>();
                byte[] passwordSalt = Array.Empty<byte>();

                foreach (DataRow userRow in users.Rows)
                {
                    passwordHash = userRow["password_hash"] == DBNull.Value ? Array.Empty<byte>() : (byte[])userRow["password_hash"];
                    passwordSalt = userRow["password_salt"] == DBNull.Value ? Array.Empty<byte>() : (byte[])userRow["password_salt"];
                    body.role = userRow["role"] == DBNull.Value ? "" : (string)userRow["role"];
                }

                bool isValid = CryptoLogic.CompareHastVsString(body.password, passwordHash, passwordSalt);

                if (!isValid)
                {
                    throw new Exception("Wrong Credential");
                }

                //create jwt token

                string tokenJwt = JwtTokenLogic.GenerateJwtToken(new[]
                {
                    new Claim("name", body.name ?? ""),
                    new Claim(ClaimTypes.Role, body.role?? "")
                }
                );

                return Ok(tokenJwt);




            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Register([FromBody] UserDTO body)
        {
            try
            {
                if (String.IsNullOrEmpty(body.name))
                {
                    throw new Exception("username cant be empty");
                }

                if (String.IsNullOrEmpty(body.password))
                {
                    throw new Exception("password cant be empty");
                }

                if (String.IsNullOrEmpty(body.role))
                {
                    throw new Exception("role cant be empty");
                }

                string query = "Select top 1 * from users where [name] = @name";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = body.name ?? "" }
                };

                DataTable users = CRUD.ExecuteQuery(query, sqlParams);

                if (users.Rows.Count > 0)
                {
                    throw new Exception("User Already Registered");
                }


                byte[] passwordHash = Array.Empty<byte>();
                byte[] passwordSalt = Array.Empty<byte>();

                //(passwordHash,passwordSalt) = CryptoLogic.GenerateHash(body.password);
                CryptoLogic.GenerateHash(body.password,out passwordHash, out passwordSalt);

                query = "INSERT INTO Users([name], password_hash, password_salt, [role], active) VALUES (@name, @password_hash, @password_salt, @role,0)";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@name", SqlDbType.VarChar) { Value = body.name ?? "" },
                    new SqlParameter("@password_hash", SqlDbType.VarBinary) { Value = passwordHash },
                    new SqlParameter("@password_salt", SqlDbType.VarBinary) { Value = passwordSalt },
                    new SqlParameter("@role", SqlDbType.VarChar) { Value = body.role ?? "" },
                };

                CRUD.ExecuteNonQuery(query, sqlParams);


                //generate random string
                string token = Guid.NewGuid().ToString();


                query = "Insert into Token (token, expire_date, usage_type,email) values (@token,null,'register',@email)";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@email", SqlDbType.VarChar) { Value = body.name ?? "" },
                    new SqlParameter("@token", SqlDbType.VarChar) { Value = token },
                };

                CRUD.ExecuteNonQuery(query, sqlParams);

                string sendTo = body.name ?? "";
                string subject = "Email Activation";
                string verificationUrl = baseUrl + "/verify/" + token;

                string emailBody = $@"
                <h4>This is an activation link</h4>
                <a href='{verificationUrl}'>{verificationUrl}</a>
                ";

                EmailLogic.SendEmail(sendTo, subject, emailBody);

                return StatusCode(201,"Success");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("checktokenjwt")]
        [Authorize]
        public ActionResult TokenCheck(string token)
        {
            return Ok("Success");
        }

        [HttpGet]
        [Route("verifuser")]
        public ActionResult TokenVerification(string token)
        {
            try
            {
                string query = "Select top 1 email from Token where token = @token";
                SqlParameter[] sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@token", SqlDbType.VarChar) { Value = token ?? "" }
                };

                object emailObject = CRUD.ExecuteScalar(query, sqlParams);
                string email = emailObject == DBNull.Value ? "" : (string) emailObject;

                if (String.IsNullOrEmpty(email))
                {
                    throw new Exception("Activation link not valid");
                }

                //update user => active = 1
                query = "Update users set active = 1 where name = @email";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@email", SqlDbType.VarChar) { Value = email ?? "" }
                };

                CRUD.ExecuteNonQuery(query, sqlParams);

                query = "delete from Token where token = @token";
                sqlParams = new SqlParameter[]
                {
                    new SqlParameter("@token", SqlDbType.VarChar) { Value = token ?? "" }
                };

                CRUD.ExecuteNonQuery (query, sqlParams);

                return Ok("Success Activation");



            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

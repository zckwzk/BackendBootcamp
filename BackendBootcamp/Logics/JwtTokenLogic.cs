using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendBootcamp.Logics
{
    public class JwtTokenLogic
    {
        #region Get configuration from appsettings.json (butuh di register di Program.cs)
        private static string SecretKey = "";
        private static string Issuer = "";
        private static string Audience = "";

        public static void GetConfiguration(IConfiguration configuration)
        {
            SecretKey = configuration["Jwt:SecretKey"];
            Issuer = configuration["Jwt:Issuer"];
            Audience = configuration["Jwt:Audience"];
        }
        #endregion

        // source: https://jasonwatmore.com/post/2022/01/19/net-6-create-and-validate-jwt-tokens-use-custom-jwt-middleware

        public static string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            // prepare token settings
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims),
                Issuer = Issuer,
                Audience = Audience,
                Expires = DateTime.UtcNow.AddDays(1),
            };

            // create jwt token from token settings
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken tokenObj = tokenHandler.CreateToken(tokenDescriptor); // bentuk object
            string tokenString = tokenHandler.WriteToken(tokenObj); // bentuk string

            return tokenString;

        }


        // source: https://jasonwatmore.com/post/2022/01/19/net-6-create-and-validate-jwt-tokens-use-custom-jwt-middleware
        public static IEnumerable<Claim>? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    // validate credential
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    // validate issuer
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    // validate audience
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    // validate expire time, set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
                IEnumerable<Claim> claims = jwtToken.Claims;

                return claims;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }




    }
}

using System.Security.Cryptography;
using System.Text;

namespace BackendBootcamp.Logics
{
    public class CryptoLogic
    {
        public static (byte[] hash, byte[] salt) GenerateHash(string stringPassword)
        {
            using (var hmac = new HMACSHA512())
            {
                byte[] passwordSalt = hmac.Key;
                byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringPassword));

                return (passwordHash, passwordSalt);
            }
        }

        public static void GenerateHash(string stringPassword,out byte[] hash, out byte[] salt)
        {
            using(var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringPassword));
            }
        }

        public static bool CompareHastVsString(string stringPassword, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                byte[] passwordNewHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringPassword));
                
                return passwordNewHash.SequenceEqual(passwordHash);
            }
        }
    }
}

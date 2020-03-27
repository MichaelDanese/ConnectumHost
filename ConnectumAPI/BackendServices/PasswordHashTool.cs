using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConnectumAPI.BackendServices
{//This is used to put a hashed password in the database as putting a plaintext password in is not secure. Hashes and peppers the passwords.
    public static class PasswordHashTool
    {
         public static string HashPassword(string plainText)
        {
            using (SHA256 hashAlgorithm = SHA256.Create())
            {
                Random random = new Random();
                int randNum = random.Next(1,26);
                string pepper = randNum.ToString();
                string text = plainText;
                text = text + pepper;
                byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                string hash = builder.ToString();
                return hash;    
            }
            
        }
        public static bool VerifyHashedPassword(string password, string storedPass)
        {
            using (SHA256 hashAlgorithm = SHA256.Create())
            {
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                for (int i = 1; i < 27; i++) {
                    string hash = password;
                    hash = hash +  i.ToString();
                    byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(hash));
                    StringBuilder builder = new StringBuilder();
                    for (int j = 0; j < data.Length; j++)
                    {
                        builder.Append(data[j].ToString("x2"));
                    }
                    if (comparer.Compare(storedPass, builder.ToString()) == 0)
                    {
                        return true;
                    }
                    
                }
                return false;
            }
        }
    }
}

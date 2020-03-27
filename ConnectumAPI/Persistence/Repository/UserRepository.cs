using ConnectumAPI.BackendServices;
using ConnectumAPI.Models;
using ConnectumAPI.Models.DTOs;
using ConnectumAPI.Persistence;
using ConnectumAPI.Persistence.Models.DTOs;
using ConnectumAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ConnectumAPI.Repository
{
   
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;


        public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _appSettings = appSettings.Value;
        }

        public object PasswordHasherTool { get; private set; }

        public bool CreateUser(User user)
        {
            if (user != null)
            {

                user.Password = PasswordHashTool.HashPassword(user.Password);
            }
            _db.Users.Add(user);
            return Save();
        }

        public bool DeleteUser(User user)
        {
            _db.Users.Remove(user);
            return Save();
        }

        public User GetUser(int userId)
        {
            var obj = _db.Users.FirstOrDefault(a => a.UserId == userId);
            if (obj != null && obj.PublicName == false)
            {
                obj.Name = "PRIVATE";
            }
            return obj;
        }
        public User GetUserByUserName(string userName)
        {
            var obj = _db.Users.FirstOrDefault(a => a.UserName == userName);
            if (obj != null && obj.PublicName == false)
            {
                obj.Name = "PRIVATE";
            }
            return obj;
        }

        public ICollection<User> GetUsers()
        {
            var objList = _db.Users.OrderBy(a => a.UserId).ToList();
            if (objList != null)
            {
                foreach (var i in objList)
                {
                    if (i.PublicName == false)
                    {
                        i.Name = "PRIVATE";
                    }
                }
            }
            return objList;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateUser(User user)
        {
            _db.Users.Update(user);
            return Save();
        }

        public bool UserExists(string name)
        {
            bool value = _db.Users.Any(a => a.UserName.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }
        public User Authenticate(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return null;
            }
            bool passMatch = PasswordHashTool.VerifyHashedPassword(password, user.Password);
            if (passMatch == false)
            {
                Console.WriteLine("Wrong Password: " + password);
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            return user;

        }
        public bool PasswordValidation(LoginUserDTO credentials)
        {
            string name = credentials.UserName;
            string password = credentials.Password;
            User user = GetUserByUserName(name);
            if (user != null)
            {
                if (user.UserName == name && PasswordHashTool.VerifyHashedPassword(password,user.Password))
                {
                    return true;
                }
            }
            return false;
        }
        public bool UserExists(int id)
        {
            return _db.Users.Any(a => a.UserId == id);
        }
    }
}

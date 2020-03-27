using ConnectumAPI.Models;
using ConnectumAPI.Models.DTOs;
using ConnectumAPI.Persistence.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User GetUser(int userId);
        User GetUserByUserName(string userName);
        bool PasswordValidation(LoginUserDTO credentials);
        bool UserExists(string name);
        public User Authenticate(string username, string password);
        bool UserExists(int id);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        bool Save();
    }
}

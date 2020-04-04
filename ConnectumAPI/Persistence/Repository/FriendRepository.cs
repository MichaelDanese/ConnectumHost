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

    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext _db;


        public FriendRepository(ApplicationDbContext db, IOptions<AppSettings> appSettings)
        {
            _db = db;
        }

        public ICollection<Friend> GetFriends(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "friend").ToList();
            return objList;
        }

        public ICollection<Friend> GetPending(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "pending1").ToList();
            return objList;
        }

        public ICollection<Friend> GetBlockedUser1(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "block1").ToList();
            return objList;

        }

        public ICollection<Friend> GetBlockedUser2(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "block2").ToList();
            return objList;
        }

        public ICollection<Friend> GetBlockedUserBoth(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "block").ToList();
            return objList;
        }

        public ICollection<Friend> GetRequests(int User1ID)
        {
            var objList = _db.Friends.Where(a => a.User1ID == User1ID & a.Type == "pending2").ToList();
            return objList;

        }

        public Friend GetRelationship(int User1ID, int User2ID)
        {
            var obj = _db.Friends.FirstOrDefault(a => a.User1ID == User1ID & a.User2ID == User2ID);
            return obj;
        }

        public Friend GetFriend(int User1ID, int User2ID)
        {
            var obj = _db.Friends.FirstOrDefault(a => a.User1ID == User1ID & a.User2ID == User2ID & a.Type == "friend");
            return obj;
        }

        public bool CreateRelationship(Friend friend)
        {
            if(friend != null)
            {
                _db.Friends.Add(friend);
            }
            return Save();
        }

        public bool UpdateRelationship(Friend friend)
        {
            _db.Friends.Update(friend);
            return Save();
        }

        public bool DeleteRelationship(Friend friend)
        {
            _db.Friends.Remove(friend);
            return Save();
        }

        public bool RelationshipExists(int User1ID, int User2ID)
        {
            bool val = _db.Friends.Any(a => a.User1ID == User1ID & a.User2ID == User2ID);
            return val;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}

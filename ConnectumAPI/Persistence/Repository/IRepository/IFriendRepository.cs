using ConnectumAPI.Models;
using ConnectumAPI.Models.DTOs;
using ConnectumAPI.Persistence.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Repository.IRepository
{
    public interface IFriendRepository
    {
        ICollection<Friend> GetFriends(int User1ID);
        ICollection<Friend> GetPending(int User1ID);
        ICollection<Friend> GetBlockedUser1(int User1ID);
        ICollection<Friend> GetBlockedUser2(int User1ID);
        ICollection<Friend> GetBlockedUserBoth(int User1ID);
        ICollection<Friend> GetRequests(int User1ID);
        Friend GetRelationship(int User1ID, int User2ID);
        Friend GetFriend(int User1ID, int User2ID);
        bool CreateRelationship(Friend friend);
        bool UpdateRelationship(Friend friend);
        bool DeleteRelationship(Friend friend);
        bool RelationshipExists(int User1ID, int User2ID);
        bool Save();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Models
{
    public class Friend
    {
        [Key]
        public int FriendName { get; set; }
    }
}

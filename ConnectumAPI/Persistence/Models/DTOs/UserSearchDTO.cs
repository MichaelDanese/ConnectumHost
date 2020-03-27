using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Models.DTOs
{
    public class UserSearchDTO
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        public string ProfilePic { get; set; }
    }
}

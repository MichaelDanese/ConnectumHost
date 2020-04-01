using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Models.DTOs
{
    public class FriendDTO
    {
        [Required]
        public int User1ID { get; set; }
        [Required]
        public int User2ID { get; set; }
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }
    }
}

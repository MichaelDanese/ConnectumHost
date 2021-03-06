﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Models
{
    public class Friend
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

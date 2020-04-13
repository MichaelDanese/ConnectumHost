using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectumAPI.Persistence.Models
{
    public class Connection
    {
        [Key]
        public string ConnectionID { get; set; }
        public string Name { get; set; }
        public string Interest { get; set; }
        public string Partner { get; set; }
        public string PartnerName { get; set; }

    }
}

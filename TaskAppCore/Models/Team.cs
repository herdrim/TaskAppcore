using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ICollection<AppUser> Members { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}

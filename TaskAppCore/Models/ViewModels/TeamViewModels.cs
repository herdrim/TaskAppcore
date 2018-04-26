using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models.ViewModels
{
    public class TeamJoinViewModel
    {
        [Required]
        public int TeamId { get; set; }
        public List<Team> Teams { get; set; }
        public string Password { get; set; }
    }
}

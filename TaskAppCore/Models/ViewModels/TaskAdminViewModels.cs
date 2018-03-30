using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models.ViewModels
{
    public class TaskAdminCreateModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Deathline { get; set; }
        [Required]
        public int TeamId { get; set; }
        public List<Team> TeamsList { get; set; }
    }

    public class TaskAdminEditModel
    {
        public Models.Task Task { get; set; }
        public List<AppUser> UsersList { get; set; }
    }

    public class TaskAdminModificationModel
    {
        [Required]
        public string Name { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime Deathline { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models.ViewModels
{
    public class TaskUpdateJsonModel
    {
        public int TaskId { get; set; }
        public bool IsToDo { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsTested { get; set; }
        public bool IsFinished { get; set; }
    }

    public class TaskAssignToUserModel
    {
        public string UserId { get; set; }
        public List<Models.Task> TeamTasks { get; set; }
        public List<Models.Task> UserTasks { get; set; }
    }

    public class TaskCreateModel
    {        
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]        
        public DateTime Deathline { get; set; }
    }
}

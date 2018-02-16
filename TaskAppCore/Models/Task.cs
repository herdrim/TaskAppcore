﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deathline { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsToDo { get; set; }
        public bool IsInProgress { get; set; }
        public bool IsTested { get; set; }
        public bool IsFinished { get; set; }

        [ForeignKey("Team")]
        public int? TeamId { get; set; }        
        public string UserId { get; set; }

        public virtual Team Team { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }
}

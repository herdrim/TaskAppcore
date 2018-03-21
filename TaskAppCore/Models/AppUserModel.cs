using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    // model użytkownika użyty do stworzenia db(wykorzystany w AppIdentityDbContext)
    [Table("AspNetUsers")]
    public class AppUser : IdentityUser
    {        
        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}

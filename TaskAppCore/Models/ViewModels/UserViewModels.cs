using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TaskAppCore.Models.ViewModels
{
    public class UserAdminCreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int TeamId { get; set; }
    }

    public class UserAdminEditModel
    {
        [Required]
        public AppUser User { get; set; }
        [Required]
        public List<Team> Teams { get; set; }

    }

    public class LoginUserModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }

    public class RegisterUserModel
    {
        [Required]
        [UIHint("email")]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }
}

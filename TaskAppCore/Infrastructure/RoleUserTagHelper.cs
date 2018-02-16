using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TaskAppCore.Models;


namespace TaskAppCore.Infrastructure
{
    [HtmlTargetElement("td", Attributes = "identity-role")]
    public class RoleUserTagHelper : TagHelper
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public RoleUserTagHelper(UserManager<AppUser> usrMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = usrMgr;
            roleManager = roleMgr;
        }

        [HtmlAttributeName("identity-role")]
        public string Role { get; set; }

        public override async System.Threading.Tasks.Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> names = new List<string>();
            IdentityRole role = await roleManager.FindByIdAsync(Role);
            if(role != null)
            {
                foreach(var user in userManager.Users)
                {
                    if (user != null && await userManager.IsInRoleAsync(user, role.Name))
                        names.Add(user.UserName);
                }
            }

            output.Content.SetContent(names.Count == 0 ? "No Users" : string.Join(", ", names));
        }
    }
}

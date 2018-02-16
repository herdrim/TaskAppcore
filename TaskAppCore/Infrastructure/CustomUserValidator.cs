using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TaskAppCore.Models;

namespace TaskAppCore.Infrastructure
{
    // Class to custom user validation implements IUserValidator<user model>
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            if (user.Email.ToLower().EndsWith("@example.com"))
                return System.Threading.Tasks.Task.FromResult(IdentityResult.Success);
            else
                return System.Threading.Tasks.Task.FromResult(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "EmailDomainError",
                            Description = "Only example.com email addresses are allowed"
                        }));
        }
    }

    // Class to custom user validation derived from UserValidation<user model>
    public class CustomUserValidator2 : UserValidator<AppUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);

            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if (!user.Email.ToLower().EndsWith("@example.com"))
            {
                errors.Add(new IdentityError
                {
                    Code = "EmailDomainError",
                    Description = "Only example.com email addresses are allowed"
                });
            }

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}

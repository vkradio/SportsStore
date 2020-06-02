using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public static class IdentitySeedData
    {
        const string adminUser = "admin";
        const string adminPassword = "MySecret123$";
        const string adminRole = "Administrator";

        public static async Task SeedDatabase(IServiceProvider provider)
        {
            provider.GetRequiredService<IdentityDataContext>().Database.Migrate();

            UserManager<IdentityUser> userManager = provider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            var role = await roleManager.FindByNameAsync(adminRole).ConfigureAwait(true);
            var user = await userManager.FindByNameAsync(adminUser).ConfigureAwait(true);

            if (role == null)
            {
                role = new IdentityRole(adminRole);
                IdentityResult result = await roleManager.CreateAsync(role).ConfigureAwait(true);
                if (!result.Succeeded)
                    throw new Exception($"Cannot create role: {result.Errors.FirstOrDefault()}");
            }

            if (user == null)
            {
                user = new IdentityUser(adminUser);
                IdentityResult result = await userManager.CreateAsync(user, adminPassword).ConfigureAwait(true);
                if (!result.Succeeded)
                    throw new Exception($"Cannot create user: {result.Errors.FirstOrDefault()}");
            }

            if (!await userManager.IsInRoleAsync(user, adminRole).ConfigureAwait(true))
            {
                IdentityResult result = await userManager.AddToRoleAsync(user, adminRole).ConfigureAwait(true);
                if (!result.Succeeded)
                    throw new Exception($"Cannot add user to role: {result.Errors.FirstOrDefault()}");
            }
        }
    }
}

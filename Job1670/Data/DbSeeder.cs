using Job1670.Constants;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Job1670.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            //Seed Roles
            var userManager = service.GetService<UserManager<ApplicationUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.JobSeeker.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Master.ToString()));

        }

        internal static async Task SeedRolesAndJobbSeekerAsync(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}

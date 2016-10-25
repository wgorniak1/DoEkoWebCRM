using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DoEko.Models.Identity
{
    public static class IdentityExtensions
    {
        public static async void EnsureRolesCreated(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<ApplicationDbContext>();
            if (context.AllMigrationsApplied())
            {
                var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>();

                foreach (var roleName in Roles.All)
                {
                    var RoleExistsResult = await roleManager.RoleExistsAsync(roleName.ToUpper());
                    if (!RoleExistsResult)
                    {
                        IdentityRole IdentityRole = new IdentityRole(roleName);
                        var RoleCreateResut = await roleManager.CreateAsync(IdentityRole);
                    }
                }

                var userManager = app.ApplicationServices.GetService<UserManager<ApplicationUser>>();

                var AdminUser = await userManager.FindByNameAsync(Roles.Admin);

                if (AdminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = "Admin",
                        Email = "WebCRM.Admin@doeko.pl",
                        EmailConfirmed = true,
                        FirstName = "Uzupełnić",
                        LastName = "Uzupełnić"
                    };
                    var AccountCreateResult = await userManager.CreateAsync(admin, "sKs@q8u1");

                    if (AccountCreateResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, Roles.Admin);
                    }

                }
            }
        }
    }
}

using DoEko.Controllers.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DoEko.Controllers.Extensions;

namespace DoEko.Models.Identity
{
    public static class IdentityExtensions
    {
        public static async void EnsureRolesCreated(this IApplicationBuilder app, IOptions<AppSettings> settings)
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

                var account = settings.Value.AdminAccountOptions;

                var AdminUser = await userManager.FindByNameAsync(account.Login);
                
                if (AdminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = account.Login,
                        PasswordHash = account.PwdHash,
                        Email = account.Email,
                        EmailConfirmed = true,
                        FirstName = account.FirstName,
                        LastName = account.LastName
                    };
                    //var AccountCreateResult = await userManager.CreateAsync(admin, "sKs@q8u1");
                    var AccountCreateResult = await userManager.CreateAsync(admin);
                    if (AccountCreateResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, Roles.Admin);
                    }
                }
            }
        }
    }
}

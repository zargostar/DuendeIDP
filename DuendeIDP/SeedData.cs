
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Identity;

using OrderServise.Infrastructure.Persistance;

namespace DuendeIDP
{
    public static class SeedData
    {
        public static async Task SeedUserAppData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                if (context.Users.Count() == 0)
                {
                    var defaultUser = new AppUser()
                    {
                        FirstName = "admin",
                        LastName = "admin",
                        UserName = "admin",

                    };

                    var defaulRoles = new List<IdentityRole>() {
                    new IdentityRole()
                    {
                        Name = UserRole.ADMIN,

                    },
                    new IdentityRole()
                    {
                        Name = UserRole.CUSTOMER,

                    },
                    new IdentityRole()
                    {
                        Name = UserRole.OPERATOR,

                    },
                    };
                    var roleContext = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    foreach (var role in defaulRoles)
                    {
                        await roleContext.CreateAsync(role);
                    }
                    var userRolesList = defaulRoles.Select(role => role.Name).ToList();
                    await context.CreateAsync(defaultUser, "Mm@09133689050");
                    if (userRolesList is not null)
                    {
                        await context.AddToRolesAsync(defaultUser, userRolesList);
                    }


                }
            }
        }
       
    }

}

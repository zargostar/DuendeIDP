
using DuendeIDP.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

using OrderServise.Infrastructure.Persistance;
using System.Security.Claims;

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
                        FirstName = "majid",
                        LastName = "mazroie",
                        UserName = "admin",
                        EmailConfirmed = true,
                        Email="zargostar@yahoo.com"

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

                    //await context.AddClaimsAsync(defaultUser, new Claim[]
                    //{ new Claim(JwtClaimTypes.Name, "majid mazroie") });


                }
            }
        }
       
    }

}

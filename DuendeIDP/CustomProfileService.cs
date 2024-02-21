using Duende.IdentityServer.Models;
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Duende.IdentityServer.Services;
using IdentityModel;
using System.Collections.Generic;

namespace DuendeIDP
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole>  _roleManager;

        public CustomProfileService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
          var user=await _userManager.GetUserAsync(context.Subject);
            string fullname = null;
           
            
            if (user != null)
            {
                var userClaims=await _userManager.GetClaimsAsync(user);
               var userRoles=await _userManager.GetRolesAsync(user);
               var roles= userRoles.Select(x => new Claim(JwtClaimTypes.Role,x )).ToList();
                fullname =user.FirstName +" " + user.LastName;
                var claims = new List<Claim>()
                {
                    new Claim("fullname", fullname),
                    new Claim("username",user.UserName),
                    new Claim("userId",user.Id)
                   // new Claim("rolse",userRoles?.ToArray())
                };
                context.IssuedClaims.AddRange(claims);
                context.IssuedClaims.AddRange(roles);
               // context.IssuedClaims.Add(userClaims.FirstOrDefault(x=>x.Type==JwtClaimTypes.Name));

            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
           return Task.CompletedTask;
        }
    }
}

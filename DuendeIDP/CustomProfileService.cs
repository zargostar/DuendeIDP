using Duende.IdentityServer.Models;
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DuendeIDP
{
    public class CustomProfileService : Duende.IdentityServer.Services.IProfileService
    {
        private readonly UserManager<AppUser> _userManager;

        public CustomProfileService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
          var user=await _userManager.GetUserAsync(context.Subject);
            string fullname = null;
           
            
            if (user != null)
            {
                var userClaims=await _userManager.GetClaimsAsync(user);
                var userRole=await _userManager.GetRolesAsync(user);
                fullname =user.FirstName +" " + user.LastName;
                var claims = new List<Claim>()
                {
                    new Claim("fullname", fullname),
                    new Claim("username",user.UserName)
                };
                context.IssuedClaims.AddRange(claims);
                context.IssuedClaims.Add(userClaims.FirstOrDefault(x=>x.Type==ClaimTypes.Name));

            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
           return Task.CompletedTask;
        }
    }
}

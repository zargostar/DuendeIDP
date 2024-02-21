using Duende.IdentityServer;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using DuendeIDP;
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderServise.Infrastructure.Persistance;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.AddRazorPages();
builder.Services.AddOidcStateDataFormatterCache();

builder.Services.AddDbContext<DataBaseContext>(c => c.UseSqlServer(configuration["sqlConnection"]));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DataBaseContext>()
    .AddErrorDescriber<CustomIdentityError>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
    // options.EmitStaticAudienceClaim = true;
}).AddInMemoryApiScopes(new ApiScope[]
    {
        new ApiScope("adminpanel","admin panell full accecc")
    })
   // .AddInMemoryApiResources(new )
    .AddDeveloperSigningCredential()

    .AddInMemoryIdentityResources(new List<IdentityResource> {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    })
    .AddInMemoryClients(new List<Client> {
        new Client
        {
           ClientName="Next js admin",
           ClientId="adminfrontend",
           ClientSecrets= {new Secret ("secret".Sha256() ) },
           AllowedGrantTypes=GrantTypes.CodeAndClientCredentials ,
           // AllowedGrantTypes=GrantTypes.Code ,
           RequireConsent = false,
           ClientUri = configuration["PostLogoutRedirectUris"],
           RequirePkce=true,
           RedirectUris={configuration["RedirectUris"] },
           AllowOfflineAccess=true,
          PostLogoutRedirectUris={configuration["PostLogoutRedirectUris"] },

           AllowedScopes={
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "adminpanel",
            },
           AccessTokenLifetime=3600*24*30,
           AlwaysIncludeUserClaimsInIdToken=true,
           AlwaysSendClientClaims = true,
        }

    }).AddAspNetIdentity<AppUser>()
    .AddProfileService<CustomProfileService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddAuthentication();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataBaseContext>();
    //await context.Database.MigrateAsync();
    await context.Database.MigrateAsync();
    // await SeedData.SeedDataLast(context);
    await SeedData.SeedUserAppData(app)
    ;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.Run();

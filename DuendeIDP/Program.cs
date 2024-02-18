using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using DuendeIDP;
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderServise.Infrastructure.Persistance;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;
builder.Services.AddRazorPages();
builder.Services
           .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(options =>
           {
               // add an instance of the patched manager to the options:
               options.CookieManager = new ChunkingCookieManager();

               options.Cookie.HttpOnly = true;
               options.Cookie.SameSite = SameSiteMode.None;
               options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
           });
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
    options.EmitStaticAudienceClaim = true;
})


    //.AddInMemoryApiScopes(new List<ApiScope> { new ApiScope
    //{ Name="adminpanel",DisplayName="admin panell"}
    //})
    .AddInMemoryApiScopes(new ApiScope[]
    {
        new ApiScope("adminpanel","admin panell full accecc")
    })
    //.AddInMemoryApiResources()
    .AddDeveloperSigningCredential()
    //.AddInMemoryApiResources(new ApiResource[]
    //{
    //    // name and human-friendly name of our API
    //    new ApiResource("doughnutapi", "Doughnut API")
    //})
    //.AddTestUsers(new List<TestUser>() {   new TestUser{
    //    IsActive=true,
    //    Password="123456",
    //    Username="majid",
    //    SubjectId="1" } })
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
           //AllowedGrantTypes=GrantTypes.CodeAndClientCredentials ,
            AllowedGrantTypes=GrantTypes.Code ,
           RequireConsent = false,
          //  ClientUri = configuration["PostLogoutRedirectUris"],
          // AllowedGrantTypes =  new[] { GrantType.AuthorizationCode },
           RequirePkce=true,
           // AllowedCorsOrigins= { "http://localhost:3000" },
           RedirectUris={configuration["RedirectUris"] },
           AllowOfflineAccess=true,
          PostLogoutRedirectUris={configuration["PostLogoutRedirectUris"] },
          
           AllowedScopes={
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "adminpanel"
            },
           AccessTokenLifetime=3600*24*30,
           AlwaysIncludeUserClaimsInIdToken=true,
           AlwaysSendClientClaims = true,
        }

    }).AddAspNetIdentity<AppUser>().AddProfileService<CustomProfileService>();

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

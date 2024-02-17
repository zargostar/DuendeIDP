using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using DuendeIDP;
using DuendeIDP.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderServise.Infrastructure.Persistance;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration=builder.Configuration;
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DataBaseContext>(c => c.UseSqlServer(configuration["sqlConnection"]));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<DataBaseContext>()
    .AddErrorDescriber<CustomIdentityError>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer()
    //.AddInMemoryApiScopes(new List<ApiScope> { new ApiScope
    //{ Name="adminpanel",DisplayName="admin panell"}
    //})
    .AddInMemoryApiScopes(new ApiScope[]
    {
        new ApiScope("adminpanel.fullaccess","admin panell full accecc")
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
           AllowedGrantTypes=GrantTypes.CodeAndClientCredentials ,
          // AllowedGrantTypes =  new[] { GrantType.AuthorizationCode },
           RequirePkce=false,
           RedirectUris={configuration["RedirectUris"] },
           AllowOfflineAccess=true,
          // PostLogoutRedirectUris={configuration["PostLogoutRedirectUris"] },
           AllowedScopes={"openid","profile","adminpanel.fullaccess"},
           AccessTokenLifetime=3600*24*30,
           AlwaysIncludeUserClaimsInIdToken=true,
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

app.Run();

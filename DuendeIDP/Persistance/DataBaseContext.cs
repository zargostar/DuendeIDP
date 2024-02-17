using DuendeIDP.Entities;
using DuendeIDP.Persistance.EFConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OrderServise.Infrastructure.Persistance
{
    public class DataBaseContext : IdentityDbContext<AppUser>
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }
        //public DbSet<AppUser>  Users { get; set; }
        
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         //   modelBuilder.ApplyConfiguration(new OrderEfConfiguration());
         //   modelBuilder.HasDefaultSchema("ordering");
           
            base.OnModelCreating(modelBuilder);
        }
       
    }
}

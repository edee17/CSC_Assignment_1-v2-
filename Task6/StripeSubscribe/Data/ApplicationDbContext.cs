using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StripeSubscribe.Models;

namespace StripeSubscribe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   //Reference: http://benjii.me/2016/05/dotnet-ef-migrations-for-asp-net-core/  
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.DisplayName());
            }
            modelBuilder.Entity<Subscription>()
                .HasOne(input => input.Customer)
                .WithMany(input => input.Subscriptions)
                .HasForeignKey(input => input.CustomerID);

            base.OnModelCreating(modelBuilder);
        }
    }
}

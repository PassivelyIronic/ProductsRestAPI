using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProductAPI.Persistance
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }

        public DbSet<ForbiddenWord> ForbiddenWords { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<ProductHistory>()
                .HasOne(ph => ph.Product)
                .WithMany()
                .HasForeignKey(ph => ph.ProductId);
        }
        
    }
}

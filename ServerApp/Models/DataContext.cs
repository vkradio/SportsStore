using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts)
            : base(opts) { }

        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Supplier> Suppliers { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guard.Against.Null(modelBuilder, nameof(modelBuilder));

            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.Supplier!)
                .WithMany(s => s.Products)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

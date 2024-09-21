using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StealTheCatsAPI.Models;
using System;

namespace StealTheCatsAPI.Data
{
    // The DbContext class for managing database operations related to cats and tags
    public class CatsDataContext : DbContext
    {
        public CatsDataContext(DbContextOptions<CatsDataContext> options) : base(options)
        {
        }

        // DbSet representing the Cats table
        public DbSet<CatEntity> Cats { get; set; }

        // DbSet representing the Tags table
        public DbSet<TagEntity> Tags { get; set; }

        // DbSet representing the CatTags junction table for many-to-many relationship
        public DbSet<CatTagEntity> CatTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatTagEntity>()
                .HasKey(ct => new { ct.CatId, ct.TagId });

            // Define the relationship between CatTagEntity and CatEntity
            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Cat) 
                .WithMany(c => c.CatTags) 
                .HasForeignKey(ct => ct.CatId);

            // Define the relationship between CatTagEntity and TagEntity
            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Tag) 
                .WithMany(t => t.CatTags) 
                .HasForeignKey(ct => ct.TagId);
        }
    }
}

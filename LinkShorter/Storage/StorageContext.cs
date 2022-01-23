using LinkShorter.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorter.Storage
{
    public class StorageContext : DbContext
    {
        public StorageContext(DbContextOptions<StorageContext> options) : base(options)
        {
        }

        public DbSet<LinkModel> Links { get; init; }
        public DbSet<VisitModel> Visits { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkModel>().HasIndex(link => link.Name).IsUnique();
        }
    }
}
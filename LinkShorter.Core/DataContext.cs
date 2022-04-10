using LinkShorter.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShorter.Core
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Link> Links { get; init; }
        public DbSet<Visit> Visits { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>().HasIndex(link => link.Name).IsUnique();
        }
    }
}
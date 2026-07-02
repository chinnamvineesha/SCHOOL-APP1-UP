using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<TeachingStaff> TeachingStaff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<TeachingStaff>()
                .HasIndex(t => t.Email)
                .IsUnique();

            modelBuilder.Entity<TeachingStaff>()
                .HasIndex(t => t.EmployeeId)
                .IsUnique();
        }
    }
}

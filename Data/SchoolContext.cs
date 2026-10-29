using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Data;

public class SchoolContext(DbContextOptions<SchoolContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasIndex(s => s.StudentNumber).IsUnique();
        modelBuilder.Entity<Student>().Property(s => s.Marks).HasPrecision(5, 2);
    }
}

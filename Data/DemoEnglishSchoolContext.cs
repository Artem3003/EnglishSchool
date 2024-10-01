using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using demo_english_school.Models;
using Microsoft.EntityFrameworkCore;

namespace demo_english_school.Data;

public class DemoEnglishSchoolContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Admin> Admins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("DemoEnglishSchool");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.User)
            .WithOne()
            .HasForeignKey<Teacher>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
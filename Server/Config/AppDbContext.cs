using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Config;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<UserCompany> UserCompanies { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(e => e.Email)
            .IsUnique();

//        modelBuilder.Entity<User>()
//            .HasMany(e => e.AdditionalInfo)
//            .WithOne(e => e.User)
//            .HasForeignKey(e => e.UserId)
//            .IsRequired();
        
        modelBuilder.Entity<Company>()
            .HasIndex(e => e.Name)
            .IsUnique();

        modelBuilder.Entity<UserCompany>()
            .HasKey(e => new { e.UserId, e.CompanyId });

        modelBuilder.Entity<UserCompany>()
            .HasOne(e => e.User)
            .WithMany(e => e.UserCompanies)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<UserCompany>()
            .HasOne(e => e.Company)
            .WithMany(e => e.UserCompanies)
            .HasForeignKey(e => e.CompanyId);
    }
}
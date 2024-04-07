using Microsoft.EntityFrameworkCore;
using Server.Data.Models;

namespace Server.Config;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(e => e.Email)
            .IsUnique();
        
        modelBuilder.Entity<Company>()
            .HasIndex(e => e.Name)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(e => e.RefreshTokens)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

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
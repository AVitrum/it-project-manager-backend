using DatabaseService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ProfilePhoto> ProfilePhotos => Set<ProfilePhoto>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<PositionInCompany> PositionInCompanies => Set<PositionInCompany>();


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
        
        modelBuilder.Entity<User>()
            .HasOne(e => e.ProfilePhoto)
            .WithOne(e => e.User)
            .HasForeignKey<ProfilePhoto>(e => e.UserId)
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

        modelBuilder.Entity<PositionInCompany>()
            .HasMany(e => e.UserCompanies)
            .WithOne(e => e.PositionInCompany)
            .HasForeignKey(e => e.PositionInCompanyId)
            .IsRequired();
        
        modelBuilder.Entity<Company>()
            .HasMany(e => e.PositionInCompanies)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .IsRequired();
    }
}
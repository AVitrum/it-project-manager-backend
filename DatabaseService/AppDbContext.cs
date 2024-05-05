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
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ProjectPerformer> ProjectPerformers => Set<ProjectPerformer>();
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

        modelBuilder.Entity<Company>()
            .HasMany(e => e.Users)
            .WithMany(e => e.Companies)
            .UsingEntity<Employee>();

        modelBuilder.Entity<ProjectPerformer>()
            .HasOne(e => e.Project)
            .WithMany(e => e.ProjectPerformers)
            .HasForeignKey(e => e.ProjectId);

        modelBuilder.Entity<Employee>()
            .HasMany(e => e.ProjectPerformers)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId)
            .IsRequired();

        modelBuilder.Entity<Company>()
            .HasMany(e => e.PositionInCompanies)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .IsRequired();
    }
}
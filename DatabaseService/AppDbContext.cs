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
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ProjectPerformer> ProjectPerformers => Set<ProjectPerformer>();
    public DbSet<AssignmentPerformer> AssignmentPerformers => Set<AssignmentPerformer>();
    public DbSet<PositionInCompany> PositionInCompanies => Set<PositionInCompany>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<AssignmentHistory> AssignmentHistories => Set<AssignmentHistory>();
    public DbSet<AssignmentFile> AssignmentFiles => Set<AssignmentFile>();

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

        modelBuilder.Entity<Project>()
            .HasMany(e => e.Assignments)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId);
        
        modelBuilder.Entity<AssignmentPerformer>()
            .HasOne(e => e.Assignment)
            .WithMany(e => e.Performers)
            .HasForeignKey(e => e.AssignmentId);

        modelBuilder.Entity<Comment>()
            .HasOne(e => e.User)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.UserId);
        
        modelBuilder.Entity<Comment>()
            .HasOne(e => e.Assignment)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.AssignmentId);
        
        modelBuilder.Entity<AssignmentFile>()
            .HasOne(e => e.Assignment)
            .WithMany(e => e.Files)
            .HasForeignKey(e => e.AssignmentId);
            

        modelBuilder.Entity<AssignmentHistory>()
            .HasOne(e => e.Assignment)
            .WithMany(e => e.Changes)
            .HasForeignKey(e => e.AssignmentId);
        
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

        modelBuilder.Entity<Company>()
            .HasMany(e => e.Projects)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .IsRequired();
    }
}
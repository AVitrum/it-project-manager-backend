using Microsoft.EntityFrameworkCore;
using Server.Data.Models;
using Server.Data.SubModels;

namespace Server.Config;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<UserTeam> UserTeams { get; set; }

    
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
        
        modelBuilder.Entity<Team>()
            .HasIndex(e => e.Name)
            .IsUnique();

        modelBuilder.Entity<UserTeam>()
            .HasKey(e => new { e.UserId, e.TeamId });

        modelBuilder.Entity<UserTeam>()
            .HasOne(e => e.User)
            .WithMany(e => e.UserTeams)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<UserTeam>()
            .HasOne(e => e.Team)
            .WithMany(e => e.UserTeams)
            .HasForeignKey(e => e.TeamId);
    }
}
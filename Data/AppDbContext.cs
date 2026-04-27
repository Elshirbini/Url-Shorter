using Microsoft.EntityFrameworkCore;
using UrlShorter.Modules.Users.Models;
using UrlShorter.Modules.Auth.Models;
using UrlShorter.Modules.Categories.Models;

namespace UrlShorter.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
        .HasIndex(x => x.Jti)
        .IsUnique();

        modelBuilder.Entity<Category>()
        .HasOne(c => c.User)
        .WithMany(u => u.Categories)
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
        .HasIndex(c => new { c.UserId, c.Name })
        .IsUnique();
    }

}
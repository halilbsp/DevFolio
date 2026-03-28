using DevFolio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevFolio.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Veritabanında oluşacak tablolarımız 👇
    public DbSet<User> Users { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Experience> Experiences { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bire-Bir (One-to-One) İlişki: Her kullanıcının sadece 1 portfolyosu olabilir 🔗
        modelBuilder.Entity<User>()
            .HasOne(u => u.Portfolio)
            .WithOne(p => p.User)
            .HasForeignKey<Portfolio>(p => p.UserId);

        // Tablo isimlerini daha temiz tutmak istersen (İsteğe bağlı, veritabanında güzel dursun diye) 🎨
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Portfolio>().ToTable("Portfolios");
        modelBuilder.Entity<Project>().ToTable("Projects");
        modelBuilder.Entity<Skill>().ToTable("Skills");
        modelBuilder.Entity<Experience>().ToTable("Experiences");
        modelBuilder.Entity<SocialLink>().ToTable("SocialLinks");
    }
}
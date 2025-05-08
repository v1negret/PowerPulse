using Microsoft.EntityFrameworkCore;
using PowerPulse.Core.Entities;

namespace PowerPulse.Infrastructure.Data;

public sealed class EnergyDbContext : DbContext
{
    public EnergyDbContext(DbContextOptions<EnergyDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .ToTable("users")
            .HasMany(u => u.MeterReadings)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<MeterReading>()
            .ToTable("meter_readings")
            .HasKey(m => new {m.Id, m.UserId, m.Date});
    }
}
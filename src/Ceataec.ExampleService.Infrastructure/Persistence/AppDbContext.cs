using Ceataec.ExampleService.Domain.Certificates;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Voyages;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<Tank> Tanks => Set<Tank>();
    public DbSet<Voyage> Voyages => Set<Voyage>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

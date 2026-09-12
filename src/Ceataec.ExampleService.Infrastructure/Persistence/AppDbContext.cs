using Ceataec.ExampleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), ICommandDbContext
{
    DbSet<TAggregateRoot> ICommandDbContext.Set<TAggregateRoot>()
        => Set<TAggregateRoot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

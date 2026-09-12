using Ceataec.ExampleService.Api;
using Ceataec.ExampleService.Infrastructure;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddApi()
    .AddInfrastructure(builder.Configuration);

builder.EnrichNpgsqlDbContext<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapDefaultEndpoints();
app.UseApi();
app.Run();

public partial class Program;

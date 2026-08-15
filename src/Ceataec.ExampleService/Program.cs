using Ceataec.ExampleService.Api;
using Ceataec.ExampleService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApi()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseApi();
app.Run();

public partial class Program;

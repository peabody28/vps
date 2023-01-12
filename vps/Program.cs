using vps.Interfaces;
using vps.Operations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDockerOperation, DockerOperation>();
builder.Services.AddScoped<ITcpOperation, TcpOperation>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

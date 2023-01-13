using vps.Interfaces;
using vps.Operations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IProcessOperation, ProcessOperation>();
builder.Services.AddScoped<IDockerOperation, DockerOperation>();
builder.Services.AddScoped<INetworkOperation, NetworkOperation>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

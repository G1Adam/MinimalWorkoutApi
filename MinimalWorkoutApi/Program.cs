using Microsoft.EntityFrameworkCore;
using MinimalWorkoutApi.DatabaseContext;
using MinimalWorkoutApi.Endpoints;
using MinimalWorkoutApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WorkoutDbContext>(opt => opt.UseInMemoryDatabase("WorkoutEntries"));
builder.Services.RegisterEndpointServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("/workoutEntry").MapWorkoutEndpoints().WithOpenApi();

app.MapGroup("/workoutEntry/{id}/set").MapSetEndpoints().WithOpenApi();

app.Run();


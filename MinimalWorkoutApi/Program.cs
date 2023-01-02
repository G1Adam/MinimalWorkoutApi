using Microsoft.EntityFrameworkCore;
using MinimalWorkoutApi.DatabaseContext;
using MinimalWorkoutApi.Endpoints;
using MinimalWorkoutApi.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<WorkoutDbContext>(opt => opt.UseInMemoryDatabase("WorkoutEntries"));
    builder.Services.RegisterEndpointServices();

    builder.Host.UseSerilog((hostContext, services, configuration) =>
    {
        configuration
            .WriteTo.File("minimalWorkoutApi-logs.txt")
            .WriteTo.Console();
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapGroup("/workoutEntry").MapWorkoutEndpoints().WithOpenApi();

    app.MapGroup("/workoutEntry/{id}/set").MapSetEndpoints().WithOpenApi();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();   
}



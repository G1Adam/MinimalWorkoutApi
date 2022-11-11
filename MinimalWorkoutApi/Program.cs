using Microsoft.EntityFrameworkCore;
using MinimalWorkoutApi;
using MinimalWorkoutApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WorkoutDbContext>(opt => opt.UseInMemoryDatabase("WorkoutEntries"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/workoutEntries", async (WorkoutDbContext db) => await db.WorkoutEntries.Include(a => a.Sets).ToListAsync()).WithOpenApi();

app.MapGet("/workoutEntries/{id}", async (int id, WorkoutDbContext db) => 
{
    var workoutEntry = await db.WorkoutEntries.Include(a => a.Sets).FindAsync(id);

    if(workoutEntry is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(workoutEntry);
}).WithOpenApi();


app.MapPost("/workoutEntries", async (WorkoutEntry workout, WorkoutDbContext db) => 
{
    db.Add(workout);

    await db.SaveChangesAsync();

    return Results.Created($"/workoutEntries/{workout.Id}", workout);
}).WithOpenApi();

app.MapPut("/workoutEntries", async (int id, WorkoutEntry workout, WorkoutDbContext db) =>
{
    var workoutEntry = await db.WorkoutEntries.Include(a => a.Sets).FindAsync(id);

    if (workoutEntry is null)
    {
        return Results.NotFound(id);
    }

    workoutEntry.Name = workout.Name;
    workoutEntry.WorkoutDate = workout.WorkoutDate;
    workoutEntry.Sets = workout.Sets;

    await db.SaveChangesAsync();

    return Results.NoContent();

}).WithOpenApi();

app.MapDelete("/workoutEntries", async (int id, WorkoutDbContext db) =>
{
    var workoutEntry = db.WorkoutEntries.Find(id);

    if(workoutEntry is null)
    {
        return Results.NotFound();
    }

    db.Remove(workoutEntry);
    await db.SaveChangesAsync();

    return Results.NoContent();

}).WithOpenApi();

app.Run();


namespace MinimalWorkoutApi.Endpoints
{
    using Microsoft.EntityFrameworkCore;
    using MinimalWorkoutApi.Models;

    internal static class WorkoutEndpoints
    {
        public static void MapWorkoutEndpoints(this WebApplication app)
        {
            app.MapGet("/workoutEntry", GetAllWorkoutEntries).WithOpenApi();
            app.MapGet("/workoutEntry/{id}", GetWorkoutEntryById).WithOpenApi();
            app.MapPost("/workoutEntry", CreateWorkoutEntry).WithOpenApi();
            app.MapPut("workoutEntry/{id}", UpdateWorkoutEntry).WithOpenApi();
            app.MapDelete("workoutEntry/{id}", DeleteWorkoutEntry).WithOpenApi();
        }

        internal static async Task<List<WorkoutEntry>> GetAllWorkoutEntries(WorkoutDbContext db)
        {
            return await db.WorkoutEntries.Include(a => a.Sets).ToListAsync();
        }

        internal static async Task<IResult> GetWorkoutEntryById(int id, WorkoutDbContext db)
        {
            var workoutEntry = await db.WorkoutEntries.Include(a => a.Sets).FirstOrDefaultAsync(o => o.Id == id);

            if (workoutEntry is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(workoutEntry);
        }

        internal static async Task<IResult> CreateWorkoutEntry(WorkoutEntry workoutEntry, WorkoutDbContext db)
        {
            db.Add(workoutEntry);

            await db.SaveChangesAsync();

            return Results.Created($"/workoutEntries/{workoutEntry.Id}", workoutEntry);
        }

        internal static async Task<IResult> UpdateWorkoutEntry(int id, WorkoutEntry workoutEntry, WorkoutDbContext db)
        {
            var workoutEntryFromDb = await db.WorkoutEntries.Include(a => a.Sets).FirstOrDefaultAsync(o => o.Id == id);

            if (workoutEntryFromDb is null)
            {
                return Results.NotFound(id);
            }

            workoutEntryFromDb.Name = workoutEntry.Name;
            workoutEntryFromDb.WorkoutDate = workoutEntry.WorkoutDate;
            workoutEntryFromDb.Sets = workoutEntry.Sets;

            await db.SaveChangesAsync();

            return Results.NoContent();
        }

        internal static async Task<IResult> DeleteWorkoutEntry(int id, WorkoutDbContext db)
        {
            var workoutEntry = db.WorkoutEntries.Find(id);

            if (workoutEntry is null)
            {
                return Results.NotFound();
            }

            db.Remove(workoutEntry);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}

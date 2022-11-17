namespace MinimalWorkoutApi.Endpoints
{
    using Microsoft.AspNetCore.Mvc;
    using MinimalWorkoutApi.DatabaseContext;
    using MinimalWorkoutApi.Models;
    using MinimalWorkoutApi.Repository;

    public static class WorkoutEndpoints
    {
        public static void MapWorkoutEndpoints(this WebApplication app)
        {
            app.MapGet("/workoutEntry", GetAllWorkoutEntries).WithOpenApi();
            app.MapGet("/workoutEntry/{id}", GetWorkoutEntryById).WithOpenApi();
            app.MapPost("/workoutEntry", CreateWorkoutEntry).WithOpenApi();
            app.MapPut("workoutEntry/{id}", UpdateWorkoutEntry).WithOpenApi();
            app.MapDelete("workoutEntry/{id}", DeleteWorkoutEntry).WithOpenApi();
        }

        public static void RegisterWorkoutEntryServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkoutDbContext, WorkoutDbContext>();
            services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();       
        }

        internal static async Task<List<WorkoutEntry>> GetAllWorkoutEntries(IWorkoutEntryRepository workoutEntryRepository)
        {
            return await workoutEntryRepository.GetAllWorkoutEntriesAsync();
        }

        internal static async Task<IResult> GetWorkoutEntryById(int id, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntry is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(workoutEntry);
        }

        internal static async Task<IResult> CreateWorkoutEntry(WorkoutEntry workoutEntry, IWorkoutEntryRepository workoutEntryRepository)
        {
            workoutEntryRepository.CreateWorkoutEntry(workoutEntry);

            await workoutEntryRepository.SaveChangesAsync();

            return Results.Created($"/workoutEntries/{workoutEntry.Id}", workoutEntry);
        }

        internal static async Task<IResult> UpdateWorkoutEntry(int id, WorkoutEntry workoutEntry, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntryFromDb = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntryFromDb is null)
            {
                return Results.NotFound(id);
            }

            workoutEntryFromDb.Name = workoutEntry.Name;
            workoutEntryFromDb.WorkoutDate = workoutEntry.WorkoutDate;
            workoutEntryFromDb.Sets = workoutEntry.Sets;

            await workoutEntryRepository.SaveChangesAsync();

            return Results.NoContent();
        }

        internal static async Task<IResult> DeleteWorkoutEntry(int id, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntry is null)
            {
                return Results.NotFound(id);
            }

            workoutEntryRepository.DeleteWorkoutEntry(workoutEntry);
            await workoutEntryRepository.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}

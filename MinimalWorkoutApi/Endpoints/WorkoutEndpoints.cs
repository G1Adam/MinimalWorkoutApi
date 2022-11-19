﻿namespace MinimalWorkoutApi.Endpoints
{
    using Microsoft.AspNetCore.Http.HttpResults;
    using MinimalWorkoutApi.DatabaseContext;
    using MinimalWorkoutApi.Models;
    using MinimalWorkoutApi.Repository;

    public static class WorkoutEndpoints
    {
        public static RouteGroupBuilder MapWorkoutEndpoints(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllWorkoutEntries);
            group.MapGet("/{id}", GetWorkoutEntryById);
            group.MapPost("/", CreateWorkoutEntry);
            group.MapPut("/{id}", UpdateWorkoutEntry);
            group.MapDelete("/{id}", DeleteWorkoutEntry);

            return group;
        }

        public static void RegisterWorkoutEntryServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkoutDbContext, WorkoutDbContext>();
            services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();       
        }

        internal static async Task<Ok<List<WorkoutEntry>>> GetAllWorkoutEntries(IWorkoutEntryRepository workoutEntryRepository)
        {
            return TypedResults.Ok(await workoutEntryRepository.GetAllWorkoutEntriesAsync());
        }

        internal static async Task<Results<Ok<WorkoutEntry>, NotFound<int>>> GetWorkoutEntryById(int id, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntry is null)
            {
                return TypedResults.NotFound(id);
            }

            return TypedResults.Ok(workoutEntry);
        }

        internal static async Task<Created<WorkoutEntry>> CreateWorkoutEntry(WorkoutEntry workoutEntry, IWorkoutEntryRepository workoutEntryRepository)
        {
            workoutEntryRepository.CreateWorkoutEntry(workoutEntry);

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.Created($"/workoutEntries/{workoutEntry.Id}", workoutEntry);
        }

        internal static async Task<Results<NoContent, NotFound<int>>> UpdateWorkoutEntry(int id, WorkoutEntry workoutEntry, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntryFromDb = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntryFromDb is null)
            {
                return TypedResults.NotFound(id);
            }

            workoutEntryFromDb.Name = workoutEntry.Name;
            workoutEntryFromDb.WorkoutDate = workoutEntry.WorkoutDate;
            workoutEntryFromDb.Sets = workoutEntry.Sets;

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        internal static async Task<Results<NoContent, NotFound<int>>> DeleteWorkoutEntry(int id, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(id);

            if (workoutEntry is null)
            {
                return TypedResults.NotFound(id);
            }

            workoutEntryRepository.DeleteWorkoutEntry(workoutEntry);
            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.NoContent();
        }
    }
}

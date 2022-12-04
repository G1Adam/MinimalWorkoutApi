using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using MinimalWorkoutApi.Models;
using MinimalWorkoutApi.Repository;
using MinimalWorkoutApi.Validators;

namespace MinimalWorkoutApi.Endpoints
{
    public static class SetEndpoints
    {
        public static RouteGroupBuilder MapSetEndpoints(this RouteGroupBuilder group)
        {
            group.MapPost("/", CreateSet);
            group.MapPut("/", UpdateSet);
            group.MapDelete("/{setId}", DeleteSet);

            return group;
        }

        internal static async Task<Results<BadRequest<Set>, NotFound<int>, Created<Set>>> CreateSet(int workoutId, Set set, IWorkoutEntryRepository workoutEntryRepository, IValidator<Set> setValidator)
        {
            var validationResult = setValidator.Validate(set);

            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(set);
            }

            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(workoutId);

            if(workoutEntry is null)
            {
                return TypedResults.NotFound(workoutId);
            }

            workoutEntry.Sets.Add(set);

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.Created($"/workoutEntry/{workoutId}/set/{set.Id}", set);
        }

        internal static async Task<Results<BadRequest<Set>, NotFound<int>, NoContent>> UpdateSet(int workoutId, Set updatedSet, IWorkoutEntryRepository workoutEntryRepository, IValidator<Set> setValidator)
        {
            var validationResult = setValidator.Validate(updatedSet);

            if (!validationResult.IsValid)
            {
                return TypedResults.BadRequest(updatedSet);
            }

            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(workoutId);

            if (workoutEntry is null)
            {
                return TypedResults.NotFound(workoutId);
            }

            var set = workoutEntry.Sets.FirstOrDefault(o => o.Id == updatedSet.Id);

            if(set is null)
            {
                return TypedResults.NotFound(updatedSet.Id);
            }

            set.ExerciseName = updatedSet.ExerciseName;
            set.Repetitions = updatedSet.Repetitions;
            set.Weight = updatedSet.Weight;

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        internal static async Task<Results<NotFound<int>, NoContent>> DeleteSet(int workoutId, int setId, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(workoutId);

            if (workoutEntry is null)
            {
                return TypedResults.NotFound(workoutId);
            }

            var set = workoutEntry.Sets.FirstOrDefault(o => o.Id == setId);

            if (set is null)
            {
                return TypedResults.NotFound(setId);
            }

            workoutEntry.Sets.Remove(set);

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.NoContent();
        }

    }
}

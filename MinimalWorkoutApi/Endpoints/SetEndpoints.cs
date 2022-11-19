using Microsoft.AspNetCore.Http.HttpResults;
using MinimalWorkoutApi.Models;
using MinimalWorkoutApi.Repository;

namespace MinimalWorkoutApi.Endpoints
{
    public static class SetEndpoints
    {

        internal static async Task<Results<NotFound<int>, Created<Set>>> CreateSet(int workoutId, Set set, IWorkoutEntryRepository workoutEntryRepository)
        {
            var workoutEntry = await workoutEntryRepository.GetWorkEntryAsync(workoutId);

            if(workoutEntry is null)
            {
                return TypedResults.NotFound(workoutId);
            }

            workoutEntry.Sets.Add(set);

            await workoutEntryRepository.SaveChangesAsync();

            return TypedResults.Created($"workoutEntry/{workoutId}/set/{set.Id}", set);
        }

    }
}

namespace MinimalWorkoutApi.Repository
{
    using Microsoft.EntityFrameworkCore;
    using MinimalWorkoutApi.DatabaseContext;
    using MinimalWorkoutApi.Models;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class WorkoutEntryRepository : IWorkoutEntryRepository
    {
        private readonly IWorkoutDbContext workoutDbContext;

        public WorkoutEntryRepository(IWorkoutDbContext workoutDbContext)
        {
            this.workoutDbContext = workoutDbContext;
        }

        public void CreateWorkoutEntry(WorkoutEntry workoutEntry)
        {
            workoutDbContext.WorkoutEntries.Add(workoutEntry);
        }

        public void DeleteWorkoutEntry(WorkoutEntry workoutEntry)
        {
            workoutDbContext.WorkoutEntries.Remove(workoutEntry);
        }

        public async Task<List<WorkoutEntry>> GetAllWorkoutEntriesAsync()
        {
            return await workoutDbContext.WorkoutEntries.Include(o => o.Sets).ToListAsync();
        }

        public async Task<WorkoutEntry> GetWorkEntryAsync(int id)
        {
            var workoutEntry = await workoutDbContext.WorkoutEntries.Include(o => o.Sets).FirstOrDefaultAsync(o => o.Id == id);

            return workoutEntry;
        }

        public async Task SaveChangesAsync()
        {
            await workoutDbContext.SaveChangesAsync();
        }
    }
}

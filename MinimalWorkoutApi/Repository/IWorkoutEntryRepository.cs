using MinimalWorkoutApi.Models;

namespace MinimalWorkoutApi.Repository
{
    public interface IWorkoutEntryRepository
    {
        public Task<List<WorkoutEntry>> GetAllWorkoutEntriesAsync();
        public Task<WorkoutEntry> GetWorkEntryAsync(int id);
        public void CreateWorkoutEntry(WorkoutEntry workoutEntry);
        public void DeleteWorkoutEntry(WorkoutEntry workoutEntry);
        public Task SaveChangesAsync();
    }
}

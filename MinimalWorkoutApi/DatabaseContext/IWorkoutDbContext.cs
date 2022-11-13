using Microsoft.EntityFrameworkCore;
using MinimalWorkoutApi.Models;

namespace MinimalWorkoutApi.DatabaseContext
{
    public interface IWorkoutDbContext
    {
        public DbSet<WorkoutEntry> WorkoutEntries { get; }

        public Task SaveChangesAsync();
    }
}

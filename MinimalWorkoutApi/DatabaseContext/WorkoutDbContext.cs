namespace MinimalWorkoutApi.DatabaseContext
{
    using Microsoft.EntityFrameworkCore;
    using MinimalWorkoutApi.Models;

    public class WorkoutDbContext : DbContext, IWorkoutDbContext
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options) { }

        public DbSet<WorkoutEntry> WorkoutEntries => Set<WorkoutEntry>();

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

    }
}

namespace MinimalWorkoutApi
{
    using Microsoft.EntityFrameworkCore;
    using MinimalWorkoutApi.Models;

    public class WorkoutDbContext : DbContext
    {
        public WorkoutDbContext(DbContextOptions<WorkoutDbContext> options) : base(options) { }

        public DbSet<WorkoutEntry> WorkoutEntries => Set<WorkoutEntry>();

    }
}

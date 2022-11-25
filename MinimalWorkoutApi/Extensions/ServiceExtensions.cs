namespace MinimalWorkoutApi.Extensions
{
    using MinimalWorkoutApi.DatabaseContext;
    using MinimalWorkoutApi.Repository;

    public static class ServiceExtensions
    {
        public static void RegisterEndpointServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkoutDbContext, WorkoutDbContext>();
            services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();
        }
    }
}

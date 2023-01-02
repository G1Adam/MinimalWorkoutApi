namespace MinimalWorkoutApi.Extensions
{
    using FluentValidation;
    using MinimalWorkoutApi.DatabaseContext;
    using MinimalWorkoutApi.Models;
    using MinimalWorkoutApi.Repository;
    using MinimalWorkoutApi.Validators;

    public static class ServiceExtensions
    {
        public static void RegisterEndpointServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkoutDbContext, WorkoutDbContext>();
            services.AddScoped<IWorkoutEntryRepository, WorkoutEntryRepository>();
            services.AddScoped<IValidator<WorkoutEntry>, WorkoutEntryValidator>();
            services.AddScoped<IValidator<Set>, SetValidator>();
        }
    }
}

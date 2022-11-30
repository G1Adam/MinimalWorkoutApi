namespace MinimalWorkoutApi.Validators
{
    using FluentValidation;
    using MinimalWorkoutApi.Models;

    public class WorkoutEntryValidator : AbstractValidator<WorkoutEntry>
    {
        public WorkoutEntryValidator() 
        {
            RuleFor(wo => wo.Name).NotNull().MaximumLength(300);
            RuleFor(wo => wo.WorkoutDate).NotEmpty();
        }
    }
}

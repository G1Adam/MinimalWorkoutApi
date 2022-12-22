namespace MinimalWorkoutApi.Validators
{
    using FluentValidation;
    using MinimalWorkoutApi.Models;

    public class WorkoutEntryValidator : AbstractValidator<WorkoutEntry>
    {
        public WorkoutEntryValidator() 
        {
            RuleFor(wo => wo.Name).NotEmpty().MaximumLength(300);
            RuleFor(wo => wo.WorkoutDate).NotEmpty();
            RuleForEach(wo => wo.Sets).SetValidator(new SetValidator());
        }
    }
}

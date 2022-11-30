namespace MinimalWorkoutApi.Validators
{
    using FluentValidation;
    using MinimalWorkoutApi.Models;

    public class SetValidator : AbstractValidator<Set>
    {
        public SetValidator() 
        { 
            RuleFor(set => set.Repetitions).NotEmpty();
            RuleFor(set => set.Weight).NotEmpty();
            RuleFor(set => set.ExerciseName).NotNull().MaximumLength(150);
        }
    }
}

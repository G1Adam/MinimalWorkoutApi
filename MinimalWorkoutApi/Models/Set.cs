namespace MinimalWorkoutApi.Models
{
    public class Set
    {
        public int Id { get; set; }

        public string ExerciseName { get; set; } = string.Empty;

        public int Repetitions { get; set; }

        public double Weight { get; set; }
    }
}

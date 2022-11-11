namespace MinimalWorkoutApi.Models
{
    public class WorkoutEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime WorkoutDate { get; set; }
        public List<Set> Sets { get; set; } = new();
    }
}

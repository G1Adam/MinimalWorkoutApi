namespace MinimalWorkoutApi.UnitTests.Validators
{
    using Xunit;
    using AutoFixture;
    using MinimalWorkoutApi.Validators;
    using MinimalWorkoutApi.Models;

    public class WorkoutEntryValidatorTests
    {
        private readonly Fixture fixture;
        private readonly WorkoutEntryValidator sut;

        public WorkoutEntryValidatorTests()
        {
            fixture= new Fixture();
            sut = new WorkoutEntryValidator();
        }

        [Fact]
        public void Validate_When_WorkoutEntryIsPopulated_Should_ReturnIsValidTrue()
        {
            //Arrange
            var workoutEntry = fixture.Create<WorkoutEntry>();

            //Act
            var result = sut.Validate(workoutEntry);

            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_When_WorkoutEntryNameIsNullOrEmptyString_Should_ReturnIsValidAsFalse(string workoutEntryName)
        {
            //Arrange
            var workoutEntry = fixture.Build<WorkoutEntry>().With(o => o.Name, workoutEntryName).Create();

            //Act
            var result = sut.Validate(workoutEntry);

            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_When_WorkoutEntryWorkoutDateIsDefault_Should_ReturnIsValidAsFalse()
        {
            //Arrange
            var workoutEntry = fixture.Build<WorkoutEntry>().With(o => o.WorkoutDate, DateTime.MinValue).Create();

            //Act
            var result = sut.Validate(workoutEntry);

            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_When_WorkoutHasInvalidSets_Should_ReturnIsValidFalse()
        {
            //Arrange
            Set invalidSet = new();

            var workoutEntry = fixture.Build<WorkoutEntry>().With(o => o.Sets, new List<Set> { invalidSet }).Create();

            //Act
            var result = sut.Validate(workoutEntry);

            //Assert
            Assert.False(result.IsValid);
        }
    }
}

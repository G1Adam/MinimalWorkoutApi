namespace MinimalWorkoutApi.UnitTests.Validators
{
    using AutoFixture;
    using MinimalWorkoutApi.Models;
    using MinimalWorkoutApi.Validators;
    using Xunit;

    public class SetValidatorTests
    {
        private readonly Fixture fixture;
        private readonly SetValidator sut;

        public SetValidatorTests()
        {
            fixture = new Fixture();
            sut = new SetValidator();
        }

        [Fact]
        public void Validate_When_SetIsPopulated_Should_ReturnIsValidTrue()
        {
            //Arrange
            var set = fixture.Create<Set>();

            //Act
            var result = sut.Validate(set);

            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_When_ExerciseNameIsInvalid_Should_ReturnIsValidFalse(string exerciseName)
        {
            //Arrange
            var set = fixture.Build<Set>().With(o => o.ExerciseName, exerciseName).Create();

            //Act
            var result = sut.Validate(set);

            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_When_RepetionsIsDefault_Should_ReturnIsValidFalse()
        {
            //Arrange
            var set = fixture.Build<Set>().With(o => o.Repetitions, 0).Create();

            //Act
            var result = sut.Validate(set);

            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_When_WeightIsDefault_Should_ReturnIsValidFalse()
        {
            //Arrange
            var set = fixture.Build<Set>().With(o => o.Weight, 0.0).Create();

            //Act
            var result = sut.Validate(set);

            //Assert
            Assert.False(result.IsValid);
        }

    }
}

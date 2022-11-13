namespace MinimalWorkoutApi.UnitTests
{
    using Xunit;
    using Moq;
    using AutoFixture;
    using MinimalWorkoutApi.Endpoints;
    using MinimalWorkoutApi.Repository;
    using MinimalWorkoutApi.Models;
    using Microsoft.AspNetCore.Http.HttpResults;

    public class MinimalWorkoutApiTests
    {
        private readonly Mock<IWorkoutEntryRepository> mockWorkoutEntryRepository;
        private readonly Fixture fixture = new Fixture();

        public MinimalWorkoutApiTests()
        {
            mockWorkoutEntryRepository= new Mock<IWorkoutEntryRepository>();
        }

        [Fact]
        public async Task GetWorkoutById_When_WorkoutExists_ShouldReturnWorkoutAndStatusCode200()
        {
            //Arrange
            var workoutEntry = fixture.Create<WorkoutEntry>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(o => o == workoutEntry.Id))).ReturnsAsync(workoutEntry);

            //Act
            var result = await WorkoutEndpoints.GetWorkoutEntryById(workoutEntry.Id, mockWorkoutEntryRepository.Object);
            var okObjectResult = (Ok<WorkoutEntry>)result;

            //Assert
            Assert.Equal(200, okObjectResult.StatusCode);
            Assert.Equal(workoutEntry, okObjectResult.Value);
        }

        [Fact]
        public async Task GetWorkoutById_When_WorkoutDoesNotExists_ShouldReturnNotFound()
        {
            //Arrange
            int workoutEntryId = 1;
            WorkoutEntry workoutEntry = null;

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            //Act
            var result = await WorkoutEndpoints.GetWorkoutEntryById(workoutEntryId, mockWorkoutEntryRepository.Object);
            var notFoundResult = (NotFound)result;

            //Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetWorkouts_When_WorkoutsExist_Returns_WorkoutsAndOkResult()
        {
            //Arrange
            var workouts = fixture.CreateMany<WorkoutEntry>(3).ToList();

            mockWorkoutEntryRepository.Setup(o => o.GetAllWorkoutEntriesAsync()).ReturnsAsync(workouts);

            //Act
            var result = await WorkoutEndpoints.GetAllWorkoutEntries(mockWorkoutEntryRepository.Object);

            //Assert
            Assert.Equal(workouts, result);
        }
    }
}

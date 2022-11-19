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

            //Assert
            Assert.IsType<Ok<WorkoutEntry>>(result.Result);

            var okResult = (Ok<WorkoutEntry>)result.Result;

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(workoutEntry, okResult.Value);
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

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutEntryId, notFoundResult.Value);
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
            Assert.IsType<Ok<List<WorkoutEntry>>>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotEmpty(result.Value);
            Assert.Equal(workouts.Count, result.Value.Count);
        }

        [Fact]
        public async Task GetWorkouts_When_NoWorkoutsExist_Should_ReturnEmptyList()
        {
            //Arrange
            mockWorkoutEntryRepository.Setup(o => o.GetAllWorkoutEntriesAsync()).ReturnsAsync(new List<WorkoutEntry>());

            //Act
            var result = await WorkoutEndpoints.GetAllWorkoutEntries(mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<Ok<List<WorkoutEntry>>>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task CreateWorkoutEntry_When_ValidWorkoutIsSent_Should_ReturnWorkoutAndCreatedResult()
        {
            //Arrange
            var workout = fixture.Create<WorkoutEntry>();
            var expectedLocationUrl = $"/workoutEntries/{workout.Id}";

            //Act
            var result = await WorkoutEndpoints.CreateWorkoutEntry(workout, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal(expectedLocationUrl, result.Location);
            Assert.Equal(workout, result.Value);

            mockWorkoutEntryRepository.Verify(o => o.CreateWorkoutEntry(It.Is<WorkoutEntry>(o => o.Name == workout.Name && o.WorkoutDate == workout.WorkoutDate)), Times.Once());
            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateWorkoutEntry_When_WorkoutDoesNotExist_Should_ReturnNotFoundResult()
        {
            //Arrange
            int workoutEntryId = 1;
            WorkoutEntry workoutEntry = null;

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            var updatedWorkoutEntry = fixture.Create<WorkoutEntry>();

            //Act
            var result = await WorkoutEndpoints.UpdateWorkoutEntry(workoutEntryId, updatedWorkoutEntry, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutEntryId, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateWorkoutEntry_When_WorkoutExistsAndUpdateIsSent_Should_ReturnNoContentResult()
        {
            //Arrange
            var originalWorkout = fixture.Create<WorkoutEntry>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(i => i == originalWorkout.Id))).ReturnsAsync(originalWorkout);

            var updatedWorkout = fixture.Build<WorkoutEntry>()
                .With(o => o.Id, originalWorkout.Id)
                .With(o => o.Name, fixture.Create<string>())
                .With(o => o.Sets, fixture.CreateMany<Set>(3).ToList())
                .Create();

            //Act
            var result = await WorkoutEndpoints.UpdateWorkoutEntry(originalWorkout.Id, updatedWorkout, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NoContent>(result.Result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);

            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteWorkoutEntry_When_WorkoutDoesNotExist_Should_ReturnNotFoundResult()
        {
            //Arrange
            int workoutEntryId = 1;
            WorkoutEntry workoutEntry = null;

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            //Act
            var result = await WorkoutEndpoints.DeleteWorkoutEntry(workoutEntryId, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutEntryId, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteWorkoutEntry_When_WorkoutExistsAndDeleteRequestIsSent_Should_ReturnNoContentResult()
        {
            //Arrange
            var originalWorkout = fixture.Create<WorkoutEntry>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(i => i == originalWorkout.Id))).ReturnsAsync(originalWorkout);

            //Act
            var result = await WorkoutEndpoints.DeleteWorkoutEntry(originalWorkout.Id, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NoContent>(result.Result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);

            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }
    }
}

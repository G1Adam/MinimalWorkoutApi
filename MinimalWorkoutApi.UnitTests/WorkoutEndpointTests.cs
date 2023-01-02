namespace MinimalWorkoutApi.UnitTests
{
    using Xunit;
    using Moq;
    using AutoFixture;
    using MinimalWorkoutApi.Endpoints;
    using MinimalWorkoutApi.Repository;
    using MinimalWorkoutApi.Models;
    using Microsoft.AspNetCore.Http.HttpResults;
    using FluentValidation;
    using FluentValidation.Results;

    public class WorkoutEndpointTests
    {
        private readonly Fixture fixture = new Fixture();
        private readonly Mock<IWorkoutEntryRepository> mockWorkoutEntryRepository;
        private readonly Mock<IValidator<WorkoutEntry>> mockWorkoutEntryValidator;

        public WorkoutEndpointTests()
        {
            mockWorkoutEntryRepository= new Mock<IWorkoutEntryRepository>();
            mockWorkoutEntryValidator = new Mock<IValidator<WorkoutEntry>>();
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
        public async Task CreateWorkoutEntry_When_WorkoutEntryIsInvalid_Should_ReturnBadRequestWithWorkoutEntry()
        {
            //Arrange
            var invalidWorkout = new WorkoutEntry
            {
                WorkoutDate = DateTime.MinValue,
                Name = string.Empty,
                Sets = new List<Set>
                {
                    new Set()
                }
            };

            var validationFailures = fixture.CreateMany<ValidationFailure>(3).ToList();
            var validationResult = fixture.Build<ValidationResult>()
                .With(o => o.Errors, validationFailures)
                .Create();

            mockWorkoutEntryValidator.Setup(o => o.Validate(It.Is<WorkoutEntry>(o => o.Name == invalidWorkout.Name && o.WorkoutDate == invalidWorkout.WorkoutDate))).Returns(validationResult);

            //Act
            var result = await WorkoutEndpoints.CreateWorkoutEntry(invalidWorkout, mockWorkoutEntryRepository.Object, mockWorkoutEntryValidator.Object);

            //Assert
            Assert.IsType<BadRequest<WorkoutEntry>>(result.Result);

            var badRequestResult = (BadRequest<WorkoutEntry>)result.Result;

            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(invalidWorkout, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateWorkoutEntry_When_ValidWorkoutIsSent_Should_ReturnWorkoutAndCreatedResult()
        {
            //Arrange
            var workout = fixture.Create<WorkoutEntry>();
            var expectedLocationUrl = $"/workoutEntries/{workout.Id}";

            var validationResult = fixture.Build<ValidationResult>()
                .With(o => o.Errors, new List<ValidationFailure>())
                .Create();

            mockWorkoutEntryValidator.Setup(o => o.Validate(It.Is<WorkoutEntry>(o => o.Name == workout.Name && o.WorkoutDate == workout.WorkoutDate))).Returns(validationResult);

            //Act
            var result = await WorkoutEndpoints.CreateWorkoutEntry(workout, mockWorkoutEntryRepository.Object, mockWorkoutEntryValidator.Object);

            //Assert
            Assert.IsType<Created<WorkoutEntry>>(result.Result);

            var createdResult = (Created<WorkoutEntry>)result.Result;
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(expectedLocationUrl, createdResult.Location);
            Assert.Equal(workout, createdResult.Value);

            mockWorkoutEntryRepository.Verify(o => o.CreateWorkoutEntry(It.Is<WorkoutEntry>(o => o.Name == workout.Name && o.WorkoutDate == workout.WorkoutDate)), Times.Once());
            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateWorkoutEntry_When_WorkoutEntryIsInvalid_Should_ReturnBadRequestResult()
        {
            //Arrange
            int workoutEntryId = 1;

            var invalidWorkout = new WorkoutEntry
            {
                Id = workoutEntryId,
                WorkoutDate = DateTime.MinValue,
                Name = string.Empty,
                Sets = new List<Set>
                {
                    new Set()
                }
            };

            var validationFailures = fixture.CreateMany<ValidationFailure>(3).ToList();
            var validationResult = fixture.Build<ValidationResult>()
                .With(o => o.Errors, validationFailures)
                .Create();

            mockWorkoutEntryValidator.Setup(o => o.Validate(It.Is<WorkoutEntry>(o => o.Name == invalidWorkout.Name && o.WorkoutDate == invalidWorkout.WorkoutDate))).Returns(validationResult);

            //Act
            var result = await WorkoutEndpoints.UpdateWorkoutEntry(workoutEntryId, invalidWorkout, mockWorkoutEntryRepository.Object, mockWorkoutEntryValidator.Object);

            //Assert
            Assert.IsType<BadRequest<WorkoutEntry>>(result.Result);

            var badRequestResult = (BadRequest<WorkoutEntry>)result.Result;

            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal(invalidWorkout, badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateWorkoutEntry_When_WorkoutDoesNotExist_Should_ReturnNotFoundResult()
        {
            //Arrange
            int workoutEntryId = 1;
            WorkoutEntry workoutEntry = null;

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            var updatedWorkoutEntry = fixture.Create<WorkoutEntry>();

            var validationResult = fixture.Build<ValidationResult>()
                .With(o => o.Errors, new List<ValidationFailure>())
                .Create();

            mockWorkoutEntryValidator.Setup(o => o.Validate(It.Is<WorkoutEntry>(o => o.Name == updatedWorkoutEntry.Name && o.WorkoutDate == updatedWorkoutEntry.WorkoutDate))).Returns(validationResult);

            //Act
            var result = await WorkoutEndpoints.UpdateWorkoutEntry(workoutEntryId, updatedWorkoutEntry, mockWorkoutEntryRepository.Object, mockWorkoutEntryValidator.Object);

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

            var updatedWorkoutEntry = fixture.Build<WorkoutEntry>()
                .With(o => o.Id, originalWorkout.Id)
                .With(o => o.Name, fixture.Create<string>())
                .With(o => o.Sets, fixture.CreateMany<Set>(3).ToList())
                .Create();

            var validationResult = fixture.Build<ValidationResult>()
                .With(o => o.Errors, new List<ValidationFailure>())
                .Create();

            mockWorkoutEntryValidator.Setup(o => o.Validate(It.Is<WorkoutEntry>(o => o.Name == updatedWorkoutEntry.Name && o.WorkoutDate == updatedWorkoutEntry.WorkoutDate))).Returns(validationResult);

            //Act
            var result = await WorkoutEndpoints.UpdateWorkoutEntry(originalWorkout.Id, updatedWorkoutEntry, mockWorkoutEntryRepository.Object, mockWorkoutEntryValidator.Object);

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

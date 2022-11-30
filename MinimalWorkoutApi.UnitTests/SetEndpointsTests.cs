namespace MinimalWorkoutApi.UnitTests
{
    using AutoFixture;
    using Microsoft.AspNetCore.Http.HttpResults;
    using MinimalWorkoutApi.Endpoints;
    using MinimalWorkoutApi.Models;
    using MinimalWorkoutApi.Repository;
    using Moq;
    using Xunit;

    public class SetEndpointsTests
    {
        private readonly Fixture fixture;
        private readonly Mock<IWorkoutEntryRepository> mockWorkoutEntryRepository;

        public SetEndpointsTests()
        {
            fixture = new Fixture();
            mockWorkoutEntryRepository = new Mock<IWorkoutEntryRepository>();
        }

        [Fact]
        public async Task CreateSet_When_WorkoutIsNotFound_Should_ReturnNotFoundResult()
        {
            //Arrange
            int workoutEntryId = 1;
            WorkoutEntry? workoutEntry = null;
            var set = fixture.Create<Set>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.CreateSet(workoutEntryId, set, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutEntryId, notFoundResult.Value);

        }

        [Fact]
        public async Task CreateSet_When_SetExists_Should_ReturnCreatedResult()
        {
            //Arrange
            int workoutEntryId = 1;
            var set = fixture.Create<Set>();
            var expectLocationUrl = $"/workoutEntry/{workoutEntryId}/set/{set.Id}";

            var workoutEntry = new WorkoutEntry
            {
                Id = workoutEntryId,
                WorkoutDate = fixture.Create<DateTime>()
            };

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(workoutEntryId)).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.CreateSet(workoutEntryId, set, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<Created<Set>>(result.Result);

            var createdResult = (Created<Set>)result.Result;

            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(set, createdResult.Value);
            Assert.Equal(expectLocationUrl, createdResult.Location);

            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task UpdateSet_When_WorkoutEntryIsNotFound_Should_ReturnNotFoundResultWithSuppliedWorkoutId()
        {
            //Arrange
            int workoutId = 1;
            WorkoutEntry? workoutEntry = null;
            var set = fixture.Create<Set>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(i => i == workoutId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.UpdateSet(workoutId, set, mockWorkoutEntryRepository.Object);

            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutId, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateSet_When_SetIsNotFound_Should_ReturnNotFoundResultWithSuppliedSetId()
        {
            //Arrange
            int workoutEntryId = 1;
            int setId = 1;
            int updatedSetId = 2;

            var workoutEntry = new WorkoutEntry
            {
                Id = workoutEntryId,
                Name = fixture.Create<string>(),
                WorkoutDate = fixture.Create<DateTime>(),
                Sets = new List<Set>
                {
                    fixture.Build<Set>()
                    .With(o => o.Id, setId)
                    .Create()
                }
            };

            var updatedSet = fixture.Build<Set>()
                .With(o => o.Id, updatedSetId)
                .Create();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(o => o == workoutEntryId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.UpdateSet(workoutEntryId, updatedSet, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(updatedSetId, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateSet_When_SetIsSupplied_Should_ReturnNoContentResults()
        {
            //Arrange
            int workoutEntryId = 1;
            int setId = 1;

            var workoutEntry = new WorkoutEntry
            {
                Id = workoutEntryId,
                Name = fixture.Create<string>(),
                WorkoutDate = fixture.Create<DateTime>(),
                Sets = new List<Set>
                {
                    fixture.Build<Set>()
                    .With(o => o.Id, setId)
                    .Create()
                }
            };

            var updatedSet = fixture.Build<Set>()
                .With(o => o.Id, setId)
                .Create();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(o => o == workoutEntryId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.UpdateSet(workoutEntryId, updatedSet, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NoContent>(result.Result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);

            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public async Task DeleteSet_When_WorkoutIsNotFound_Should_ReturnNotFoundResultWithWorkoutId() 
        {
            //Arrange
            int workoutId = 1;
            WorkoutEntry? workoutEntry = null;
            var setId = fixture.Create<int>();

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(i => i == workoutId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.DeleteSet(workoutId, setId, mockWorkoutEntryRepository.Object);

            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(workoutId, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteSet_When_WorkoutIsNotFound_Should_ReturnNotFoundResultWithSetId()
        {
            //Arrange
            int workoutEntryId = 1;
            int setId = 1;
            int requestedSetId = 2;

            var workoutEntry = new WorkoutEntry
            {
                Id = workoutEntryId,
                Name = fixture.Create<string>(),
                WorkoutDate = fixture.Create<DateTime>(),
                Sets = new List<Set>
                {
                    fixture.Build<Set>()
                    .With(o => o.Id, setId)
                    .Create()
                }
            };

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(o => o == workoutEntryId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.DeleteSet(workoutEntryId, requestedSetId, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NotFound<int>>(result.Result);

            var notFoundResult = (NotFound<int>)result.Result;

            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(requestedSetId, notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteSet_When_SetIsSupplied_Should_ReturnNoContentResults()
        {
            //Arrange
            int workoutEntryId = 1;
            int setId = 1;

            var workoutEntry = new WorkoutEntry
            {
                Id = workoutEntryId,
                Name = fixture.Create<string>(),
                WorkoutDate = fixture.Create<DateTime>(),
                Sets = new List<Set>
                {
                    fixture.Build<Set>()
                    .With(o => o.Id, setId)
                    .Create()
                }
            };

            mockWorkoutEntryRepository.Setup(o => o.GetWorkEntryAsync(It.Is<int>(o => o == workoutEntryId))).ReturnsAsync(workoutEntry);

            //Act
            var result = await SetEndpoints.DeleteSet(workoutEntryId, setId, mockWorkoutEntryRepository.Object);

            //Assert
            Assert.IsType<NoContent>(result.Result);

            var noContentResult = (NoContent)result.Result;

            Assert.NotNull(noContentResult);
            Assert.Equal(204, noContentResult.StatusCode);

            mockWorkoutEntryRepository.Verify(o => o.SaveChangesAsync(), Times.Once());
        }
    }
}

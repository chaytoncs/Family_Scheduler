using FamilyScheduler.Controllers;
using FamilyScheduler.Data;
using FamilyScheduler.Models;
using FamilyScheduler.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace FamilySchedulerTests
{
    public class TaskControllerTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<FamilySchedulerContext> _contextOptions;

        public TaskControllerTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<FamilySchedulerContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new FamilySchedulerContext(_contextOptions);

            if (context.Database.EnsureCreated())
            {
                var workload = new Workload
                {
                    Description = "Easy",
                    Value = 3
                };
                context.Workloads.Add(workload);

                var tasktype = new TaskType
                {
                    Description = "Kitchen Task"
                };

                context.TaskTypes.Add(tasktype);

                var frequency = new Frequency
                {
                    Description = "Daily",
                    Value = 2
                };
                context.Frequencies.Add(frequency);

                context.Tasks.Add(new FamilyScheduler.Models.Task { Description = "Wash the Dishes", Workload = workload, TaskType = tasktype, Frequency = frequency });
                context.SaveChanges();
            }
        }

        // DRY-ify the creation of the DbContext.
        FamilySchedulerContext CreateContext() => new(_contextOptions);

        // Close database connection.
        public void Dispose() => _connection?.Dispose();

        [Fact]
        public void ListReturnsViewResultWithOneTask()
        {
            FamilySchedulerContext context = CreateContext();
            var controller = new TaskController(context);

            // Act
            var result = controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result); // Do we return a ViewResult?
            // Note that Index() returns a Task -- we can observe the result by using the Result value.
            var model = Assert.IsType<List<TaskDTO>>(viewResult.Model); // Does the model contain the correct type
            Assert.Single(model); // Does the list have the right amount of widgets?
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(2)]
        public void EditGET_ReturnsNotFoundResultWithNullOrZeroOrNegativeValuesOrNonexistentID(int? id)
        {
            // Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TaskController(context);

            // Act
            var result = controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result); // Do we return a NotFoundResult?
        }

        [Theory]
        [InlineData(1)]
        public void EditGET_ReturnsViewResultWithValuesLargerThanZero(int? id)
        {
            // Arrange
            FamilySchedulerContext context = CreateContext();
            var controller = new TaskController(context);

            // Act
            var result = controller.Edit(id);

            var viewResult = Assert.IsType<ViewResult>(result.Result); // Do we return a ViewResult?
            var model = Assert.IsType<TaskDTO>(viewResult.Model); // Does the model contain the correct type
            Assert.Equal(model.TaskID, id); // Does the object have the correct property value?
        }

        // Empty Tests
        [Fact]
        public void EditPOST_ReturnsViewResult()
        {
        }

        [Fact]
        public void DetailsReturnsViewResultWithOneTask()
        {
        }

        [Fact]
        public void CreateGET_ReturnsViewResult()
        {
        }

        [Fact]
        public void CreatePOST_ReturnsViewResult()
        {
        }

        [Fact]
        public void DeleteGET_ReturnsViewResult()
        {
        }

        [Fact]
        public void DeleteConfirmedPOST_ReturnsViewResult()
        {
        }
    }
}
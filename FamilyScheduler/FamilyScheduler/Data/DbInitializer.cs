using FamilyScheduler.Models;
using SQLitePCL;

namespace FamilyScheduler.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FamilySchedulerContext context)
        {
            context.Database.EnsureCreated();

            if (context.Tasks.Any())
            {
                return;
            }

            // Create Workload Entities
            var easy = new Workload { Description = "Easy", Value = 1 };
            var medium = new Workload() { Description = "Medium", Value = 2 };
            var hard = new Workload() { Description = "Hard", Value = 3 };

            // Add to DbSet
            context.Workloads.Add(easy);
            context.Workloads.Add(medium);
            context.Workloads.Add(hard);

            // Create TaskType Entities
            var kitchen = new TaskType { Description = "Kitchen Task" };
            var outdoor = new TaskType() { Description = "Outdoor Task" };
            var house = new TaskType() { Description = "House Task" };

            // Add to DbSet
            context.TaskTypes.Add(kitchen);
            context.TaskTypes.Add(outdoor);
            context.TaskTypes.Add(house);

            // Create Frequency Entities
            var daily = new Frequency { Description = "Daily", Value = 0 };
            var weekly = new Frequency { Description = "Weekly", Value = 1 };

            // Add to DbSet
            context.Frequencies.Add(daily);
            context.Frequencies.Add(weekly);

            // Create Task Entities
            var taskList = new List<Models.Task>()
            {
                new Models.Task() { Description = "Wash the dishes", Workload = easy, Frequency = daily, TaskType = kitchen },
                new Models.Task() { Description = "Mow the lawn", Workload = hard, Frequency = weekly, TaskType = outdoor },
                new Models.Task() { Description = "Sweep the floor", Workload = easy, Frequency = daily, TaskType = kitchen },
                new Models.Task() { Description = "Clean the living room", Workload = easy, Frequency = daily, TaskType = house },
                new Models.Task() { Description = "Clean the bathroom", Workload = medium, Frequency = daily, TaskType = house },
                new Models.Task() { Description = "Walk the dog", Workload = medium, Frequency = daily, TaskType = outdoor },
                new Models.Task() { Description = "Clean the fridge", Workload = easy, Frequency = weekly, TaskType = kitchen }
            };

            // Add to DbSet
            context.Tasks.AddRange(taskList);

            // Commit Changes
            context.SaveChanges();
        }
    }
}

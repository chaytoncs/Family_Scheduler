using FamilyScheduler.Models;

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

            // Create Entities
            var workload = new Workload
            {
                Description = "Hard",
                Value = 2
            };

            // Add to DbSet
            context.Workloads.Add(workload);

            var tasktype = new TaskType
            {
                Description = "Kitchen Task"
            };
            // Add to DbSet
            context.TaskTypes.Add(tasktype);

            var frequency = new Frequency
            {
                Description = "7 Days a week",
                Value = 7
            };
            // Add to DbSet
            context.Frequencies.Add(frequency);

            var task = new FamilyScheduler.Models.Task
            {
                Description = "Please put the dirty dishes in the dishwasher",
                Workload = workload,
                TaskType = tasktype,
                Frequency = frequency,
            };

            // Add to DbSet
            context.Tasks.Add(task);

            // Commit Changes
            context.SaveChanges();
        }
    }
}

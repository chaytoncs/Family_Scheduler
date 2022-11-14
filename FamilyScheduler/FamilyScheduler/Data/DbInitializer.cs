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
                Value = 3
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
                Description = "Twice Weekly",
                Value = 2
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

            var task2 = new FamilyScheduler.Models.Task
            {
                Description = "Mow the Lawn",
                Workload = workload,
                TaskType = tasktype,
                Frequency = frequency,
            };

            var workloadList = new List<Workload>
            {
                new Workload() {Description = "Easy", Value = 1},
                new Workload() {Description = "Medium", Value = 2}
            };

            var frequencyList = new List<Frequency>
            {
                new Frequency() {Description = "Daily", Value = 1},
                new Frequency() {Description = "Weekly", Value = 7}
            };

            var taskTypeList = new List<TaskType>
            {
                new TaskType() {Description = "Outdoor Task"},
                new TaskType() {Description = "Shopping Task"}
            };


            // Add to DbSet
            context.Tasks.Add(task);
            context.Tasks.Add(task2);
            context.Workloads.AddRange(workloadList);
            context.Frequencies.AddRange(frequencyList);
            context.TaskTypes.AddRange(taskTypeList);

            // Commit Changes
            context.SaveChanges();
        }
    }
}

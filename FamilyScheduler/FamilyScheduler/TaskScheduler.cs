using FamilyScheduler.Models;

namespace TaskSchedulerLibrary
{
    public static class TaskScheduler
    {
        private static readonly int _maxTries = 5;

        /// <summary>
        /// Generates a weekly assignment schedule for the household.
        /// </summary>
        /// <param name="start">DateTime for the start date of the schedule</param>
        /// <param name="userIDs">List of user IDs</param>
        /// <param name="tasks">List of triples (task ID, task frequency (0 = daily, 1 = weekly), and task workload (unused)</param>
        /// <param name="maxTasksPerUser">Specify how many tasks a user can be scheduled for per day.</param>
        /// <returns>List of Assignment objects that specify the user, task and date.</returns>
        public static List<Assignment> ScheduleWeek(DateTime start, List<int> userIDs, List<(int TaskID, int Frequency, int Workload)> tasks, int maxTasksPerUser = 1)
        {
            DateTime startDate = start;

            List<Assignment> result = new();
            Random rnd = new();

            // Pick a random user
            foreach(int userId in userIDs.OrderBy(x => rnd.Next()))
            {
                // Pick random tasks for user
                var taskData = tasks.OrderBy(x => rnd.Next()).Take(maxTasksPerUser);
                // Iterate through tasks
                foreach(var (TaskID, Frequency, Workload) in taskData)
                {
                    Assignment a = new() { TaskID = TaskID, UserID = userId};

                    // Weekly task
                    if (Frequency == 1)
                    {
                        a.DueDate = startDate;
                        result.Add(a);
                        // Since it's a weekly task, remove it from contention as it's now assigned
                        var item = tasks.Find(x => x.TaskID == TaskID);
                        tasks.Remove(item);
                    }
                    else
                    {
                        // Daily task -- randomly assign a date and try again until we find a free date for a daily task.
                        DateTime assignedDate = startDate.AddDays(rnd.Next(0, 6));
                        int tries = 0;
                        // As long as there is a daily task with the same ID and date, try again, up to max tries
                        while (result.Where(x => x.TaskID == TaskID).Where(x => x.DueDate == assignedDate).Count() > 0)
                        {
                            tries++;
                            if (tries == _maxTries)
                                break;
                            else
                                assignedDate = startDate.AddDays(rnd.Next(0, 6));
                        }
                        // If we didn't hit max tries, assing the date and add task to result
                        if(tries != _maxTries)
                        {
                            a.DueDate = assignedDate;
                            result.Add(a);
                        }
                    }
                }
            }

            return result;
        }
    }
}
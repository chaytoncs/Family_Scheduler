using FamilyScheduler.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyScheduler.Data
{
    public class FamilySchedulerContext : DbContext
    {
        public FamilySchedulerContext(DbContextOptions<FamilySchedulerContext> options) : base(options) { }

        // Table Definitions
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Frequency> Frequencies { get; set; }
        public DbSet<FamilyScheduler.Models.Task> Tasks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workload> Workloads { get; set; }
    }
}

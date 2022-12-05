using FamilyScheduler.Areas.Identity.Data;
using FamilyScheduler.Models.DTO;
using FamilyScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyScheduler.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FamilyScheduler.Controllers
{
    [Route("Schedule")]
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly FamilySchedulerContext _context;

        // Constructor
        public ScheduleController(FamilySchedulerContext context)
        {
            _context = context;
        }

        [Route("List")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> List()
        {
            // Currently I have no way to determine assignments specific to a schedule, so I will just display all assignments
            // Query Entities and related data
            var assignments = await _context.Assignments.Include(a => a.Task)
                .Include(a => a.User)
                .Include(a => a.Task.Workload)
                .Include(a => a.Task.Frequency)
                .Include(a => a.Task.TaskType).OrderBy(a => a.DueDate).ToListAsync();

            // Transform assignments into a List of Assignment DTOs to be returned to the view
            List<AssignmentDTO> assigmentDTOs = new();

            foreach (Assignment a in assignments)
            {
                AssignmentDTO assignmentDTO = new()
                {
                    AssignmentID = a.AssignmentID,
                    TaskID = a.TaskID,
                    UserID = a.UserID,
                    DueDate = a.DueDate,
                    Completed = a.Completed,
                    TaskDescription = a.Task.Description,
                    WorkloadDescription = a.Task.Workload.Description,
                    FrequencyDescription = a.Task.Frequency.Description,
                    TaskTypeDescription = a.Task.TaskType.Description,
                    FullName = $"{a.User.FirstName} {a.User.LastName}"
                };
                assigmentDTOs.Add(assignmentDTO);
            }
            return View(assigmentDTOs);
        }

        // GET CREATE
        [Route("Create")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Create()
        {
            // Validation to Ensure there are at least 1 user and 1 task before running a schedule
            int userCount = _context.Users.Count();
            if (userCount == 0)
            {
                TempData["ErrorMessage"] = "Error: You must have Household Members before running a schedule.";
                return RedirectToAction(nameof(List));
            }

            if(!_context.Tasks.Any())
            {
                TempData["ErrorMessage"] = "Error: You must have Tasks before runnning a schedule.";
                return RedirectToAction(nameof(List));
            }

            var frequencies = await _context.Frequencies.ToListAsync();
            int taskCount = 0;

            foreach(var a in frequencies)
            {
                switch (a.Value)
                {
                    case 0:
                        taskCount += 7;
                        break;
                    case 1:
                        taskCount += 1;
                        break;
                    default:
                        break;
                }
            }

            // Adds the the result of TaskCount (Numbers of tasks need to be assigned for the week) / Number of Users to ViewBag
            // to be used as the min of the number input
            ViewBag.MinTasksPerUser = taskCount / userCount;
            return View("Create");
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(DateTime StartDate, int maxAssigns)
        {
            // I am unsure of how to ensure the model state is valid at this point
            // I am only testing the scheduling function this is not final
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userIDs = await _context.Users.Select(a => a.UserID).ToListAsync();

            // Verify there are Users
            if (userIDs == null)
            {
                TempData["ErrorMessage"] = "Error: You must have Household Members before running a schedule.";
                return RedirectToAction(nameof(List));
            }

            List<FamilyScheduler.Models.Task> tasks = await _context.Tasks.Include(t => t.Workload).Include(t => t.Frequency).ToListAsync();
            // Verify there are tasks
            if (tasks == null)
            {
                TempData["ErrorMessage"] = "Error: You must have Tasks before runnning a schedule.";
                return RedirectToAction(nameof(List));
            }

            List<(int, int, int)> triples = new List<(int, int, int)>();

            foreach(var t in tasks)
            {
                triples.Add((t.TaskID, t.Frequency.Value, t.Workload.Value));
            }

            List<Assignment> schedule = TaskSchedulerLibrary.TaskScheduler.ScheduleWeek(StartDate, userIDs, triples, maxAssigns);

            _context.Assignments.AddRange(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
    }
}

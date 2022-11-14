using FamilyScheduler.Data;
using FamilyScheduler.Models;
using FamilyScheduler.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FamilyScheduler.Controllers
{
    [Route("Task")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        // DB Context
        private readonly FamilySchedulerContext _context;

        public TaskController(ILogger<TaskController> logger, FamilySchedulerContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("List")]
        public async Task<IActionResult> List()
        {
            List<FamilyScheduler.Models.Task> tasks = await _context.Tasks.Include(x => x.Workload).Include(x => x.Frequency).Include(x => x.TaskType).ToListAsync();
            List<TaskDTO> taskDTOs = new();
            foreach (FamilyScheduler.Models.Task t in tasks)
            {
                TaskDTO taskDTO = new()
                {
                    TaskID = t.TaskID,
                    Description = t.Description,
                    FrequencyDescription = t.Frequency.Description,
                    WorkloadDescription = t.Workload.Description,
                    TaskTypeDescription = t.TaskType.Description,
                    WorkloadValue = t.Workload.Value,
                    FrequencyValue = t.Frequency.Value
                };
                taskDTOs.Add(taskDTO);
            }

            return View(taskDTOs);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View("Create");
        }

        [Route("Details/{id}")]
        public IActionResult Details()
        {
            return View("Details");
        }

        // GET REQUEST
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            // Query entity and include related data.
            var task = await _context.Tasks
                .Include(t => t.Workload)
                .Include(t => t.Frequency)
                .Include(t => t.TaskType)
                .FirstOrDefaultAsync(t => t.TaskID == id);

            // Logic check -- did we find something?
            if (task == null)
            {
                return NotFound();
            }

            // Use DTO to pass data to the View.
            TaskDTO taskDTO = new()
            {
                TaskID = task.TaskID,
                WorkloadID = task.WorkloadID,
                FrequencyID = task.FrequencyID,
                TaskTypeID = task.TaskTypeID,
                Description = task.Description,
            };

            // Add Workloads, Frequencies, and Task Types to Viewbag to display in Select List
            var workloads = await _context.Workloads.ToListAsync();
            var workloadList = new List<SelectListItem>();

            foreach (Workload w in workloads)
            {
                workloadList.Add(new SelectListItem { Value = w.WorkloadID.ToString(), Text = w.Description });
            }
            // Add list to ViewBag as a dynamic property.
            ViewBag.WorkloadList = workloadList;

            var frequencies = await _context.Frequencies.ToListAsync();
            var frequencyList = new List<SelectListItem>();

            foreach (Frequency f in frequencies)
            {
                frequencyList.Add(new SelectListItem { Value = f.FrequencyID.ToString(), Text = f.Description });
            }
            // Add list to ViewBag as a dynamic property.
            ViewBag.FrequencyList = frequencyList;

            var taskTypes = await _context.TaskTypes.ToListAsync();
            var taskTypeList = new List<SelectListItem>();

            foreach (TaskType t in taskTypes)
            {
                taskTypeList.Add(new SelectListItem { Value = t.TaskTypeID.ToString(), Text = t.Description });
            }
            // Add list to ViewBag as a dynamic property.
            ViewBag.TaskTypeList = taskTypeList;

            return View(taskDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [Bind("TaskID,WorkloadID,FrequencyID,TaskTypeID,Description")] TaskDTO task)
        {
            // Logic check -- do the IDs match
            if (id != task.TaskID)
            {
                return NotFound();
            }
            // Check validation
            if (ModelState.IsValid)
            {
                try
                {
                    // Pull entity from DB with the provided ID
                    var entity = await _context.Tasks
                        .FirstOrDefaultAsync(t => t.TaskID == id);

                    if (entity != null)
                    {
                        /* Update changes to the entity and since it's being tracked
                         * as we are in a connected state, only SaveChangesAsync is needed
                         * to persist. */

                        entity.WorkloadID = task.WorkloadID;
                        entity.FrequencyID = task.FrequencyID;
                        entity.TaskTypeID = task.TaskTypeID;
                        entity.Description = task.Description;
                        // Save changes
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tasks.Any(t => t.TaskID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            return View(task);
        }

        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Trying to figure out if I need confirm delete action or if I can do the deletion on this Action
            if (!ModelState.IsValid)
            {
                // Not sure what I want / need to return here
                return View("Error");
            }
            return View(await _context.Tasks.FindAsync(id));
        }
    }
}

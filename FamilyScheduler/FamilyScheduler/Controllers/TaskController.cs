using FamilyScheduler.Data;
using FamilyScheduler.Models;
using FamilyScheduler.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FamilyScheduler.Controllers
{
    [Route("Task")]
    [Authorize]
    public class TaskController : Controller
    {
        // DB Context
        private readonly FamilySchedulerContext _context;

        public TaskController(FamilySchedulerContext context)
        {
            _context = context;
        }

        [Route("List")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            // Query entities and related data
            List<FamilyScheduler.Models.Task> tasks = await _context.Tasks.Include(x => x.Workload).Include(x => x.Frequency).Include(x => x.TaskType).ToListAsync();

            // Transform Tasks into Task DTOs to be displayed in the Vie
            List<TaskDTO> taskDTOs = new();
            foreach (FamilyScheduler.Models.Task t in tasks)
            {
                TaskDTO taskDTO = new() {
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

        // GET CREATE
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Add Workloads, Frequencies, and Task Types to Viewbag to display in Select List
            // Add Lists to ViewBag as a dynamic properties.
            ViewBag.WorkloadList = WorkloadSelectList();
            ViewBag.FrequencyList = FrequencySelectList();
            ViewBag.TaskTypeList = TaskTypeSelectList();

            return View("Create");
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Description,FrequencyID,TaskTypeID,WorkloadID")] TaskDTO task)
        {
            // Checks if Model State is Valid
            // If Model State is Valid it will add the new Task to the DB, else it will return the invalid TaskDTO to the view
            if (ModelState.IsValid)
            {
                // Transform Task DTO to DTO
                FamilyScheduler.Models.Task t = new FamilyScheduler.Models.Task
                {
                    Description = task.Description,
                    FrequencyID = task.FrequencyID,
                    WorkloadID = task.WorkloadID,
                    TaskTypeID = task.TaskTypeID
                };
                // Add Task to Database
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(task);
        }

        [Route("Details/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            // Logic Check to see if ID was passed in
            if (id == null)
            {
                return NotFound();
            }

            // Query entity and its related data
            var task = await _context.Tasks
                .Include(t => t.Frequency)
                .Include(t => t.TaskType)
                .Include(t => t.Workload)
                .FirstOrDefaultAsync(m => m.TaskID == id);

            // Logic check to see if Task was found
            if (task == null)
            {
                return NotFound();
            }

            // Transform Task to DTO
            // Use DTO to pass data to the View.
            TaskDTO taskDTO = new()
            {
                TaskID = task.TaskID,
                WorkloadID = task.WorkloadID,
                FrequencyID = task.FrequencyID,
                TaskTypeID = task.TaskTypeID,
                Description = task.Description,
                WorkloadDescription = task.Workload.Description,
                FrequencyDescription = task.Frequency.Description,
                TaskTypeDescription = task.TaskType.Description,
                WorkloadValue = task.Workload.Value,
                FrequencyValue = task.Frequency.Value
            };

            return View(taskDTO);
        }

        // GET EDIT
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Logic check to see if ID was passed in or Tasks has data
            if (id == null)
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

            // Transform Task to DTO
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
            // Add Lists to ViewBag as a dynamic properties.
            ViewBag.WorkloadList = WorkloadSelectList();
            ViewBag.FrequencyList = FrequencySelectList();
            ViewBag.TaskTypeList = TaskTypeSelectList();

            return View(taskDTO);
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, [Bind("TaskID,WorkloadID,FrequencyID,TaskTypeID,Description")] TaskDTO task)
        {
            // Logic check -- do the IDs match
            if (id != task.TaskID)
            {
                return NotFound();
            }
            // Checks if Model State is Valid
            // If Model State is Valid it will attempt to Update Task, else return invalid Task DTO to View
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
                    // Checks if Task with ID passed in does not exist
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

        // GET DELETE
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Logic check to see if ID was passed in or Tasks has data
            if (id == null)
            {
                return NotFound();
            }

            // Query entity
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == id);

            // Logic check to see if Task with passed in ID was found
            if (task == null)
            {
                return NotFound();
            }

            // Transform Task to DTO
            // Use DTO to pass data to the View.
            TaskDTO taskDTO = new()
            {
                TaskID = task.TaskID,
                WorkloadID = task.WorkloadID,
                FrequencyID = task.FrequencyID,
                TaskTypeID = task.TaskTypeID,
                Description = task.Description,
            };
            return View(taskDTO);
        }

        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            // Query entity based on id passed in
            var task = await _context.Tasks.FindAsync(id);

            // Logic check to see if Task was found
            if (task != null)
            {
                // If found, remove task
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        // Loads all Workloads into Select List
        // Used to Display Select Lists on Create and Edit
        private List<SelectListItem> WorkloadSelectList()
        {
            var workloads = _context.Workloads.ToList();
            var workloadList = new List<SelectListItem>();

            foreach (Workload w in workloads)
            {
                workloadList.Add(new SelectListItem { Value = w.WorkloadID.ToString(), Text = w.Description });
            }
            return workloadList;
        }

        // Loads all Frequencies into Select List
        // Used to Display Select Lists on Create and Edit
        private List<SelectListItem> FrequencySelectList()
        {
            var frequencies = _context.Frequencies.ToList();
            var frequencyList = new List<SelectListItem>();

            foreach (Frequency f in frequencies)
            {
                frequencyList.Add(new SelectListItem { Value = f.FrequencyID.ToString(), Text = f.Description });
            }
            return frequencyList;
        }

        // Loads all TaskTypes into Select List
        // Used to Display Select Lists on Create and Edit
        private List<SelectListItem> TaskTypeSelectList()
        {
            var taskTypes = _context.TaskTypes.ToList();
            var taskTypeList = new List<SelectListItem>();

            foreach (TaskType t in taskTypes)
            {
                taskTypeList.Add(new SelectListItem { Value = t.TaskTypeID.ToString(), Text = t.Description });
            }
            return taskTypeList;
        }
    }
}

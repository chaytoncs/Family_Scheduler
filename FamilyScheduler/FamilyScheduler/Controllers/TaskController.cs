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
        [Authorize(Roles = "Admin,SuperUser")]
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

        // GET CREATE
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
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
            return View("Create");
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("TaskID,Description,FrequencyID,TaskTypeID,WorkloadID")] TaskDTO task)
        {
            if (ModelState.IsValid)
            {
                FamilyScheduler.Models.Task t = new FamilyScheduler.Models.Task
                {
                    TaskID = task.TaskID,
                    Description = task.Description,
                    FrequencyID = task.FrequencyID,
                    WorkloadID = task.WorkloadID,
                    TaskTypeID = task.TaskTypeID
                };
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(task);
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.Frequency)
                .Include(t => t.TaskType)
                .Include(t => t.Workload)
                .FirstOrDefaultAsync(m => m.TaskID == id);

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

        // POST EDIT
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

        // GET DELETE
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == id);
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
            return View(taskDTO);
        }

        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'FamilySchedulerContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
    }
}

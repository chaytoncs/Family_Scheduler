using FamilyScheduler.Data;
using FamilyScheduler.Models;
using FamilyScheduler.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FamilyScheduler.Controllers
{
    /// <summary>
    /// The Task Controller is used by Admins to display Task related forms/views (CRUD), as well as, performing CRUD functions for Tasks.
    /// </summary>
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

        /// <summary>
        /// The List action for Task is used to display a List view of all of the Tasks in the database.
        /// </summary>
        /// <returns>The List view for Task along with a List of TaskDTOs.</returns>
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

        /// <summary>
        /// CREATE (GET) action for Task is used to display a form for Admins to create new Tasks. Adds Select List items for Workload, Frequency, and TakType to the Viewbag.
        /// </summary>
        /// <returns>The Create for view for Task.</returns>
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

        /// <summary>
        /// CREATE (POST) action for Task is used to perform the Create and save function for a new Task. 
        /// </summary>
        /// <param name="task">The task param is the TaskDTO returned from the view, which contains the user bound TaskDTO values.</param>
        /// <returns>A redirect to the List view for Task if the creation of a new Task is successful. If the Model State is Invalid it will return the invalid task param (TaskDTO Model)
        /// to the Create(GET) view.</returns>
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

        /// <summary>
        /// Details action for Task is used to display all the details of the Task and its related data (Workload, Frequency, TaskType).
        /// </summary>
        /// <param name="id">The id param is the TaskID passed from the view or retrieved from route data, which is used to query the Tasks table to retrieve the Task details.</param>
        /// <returns>If the id passed in exists and a Task is found, it will return the Details view for Task with a TaskDTO. If the id was not passed in or the 
        /// task was not found, it will return a NotFound view.</returns>
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

        /// <summary>
        /// EDIT (GET) action is used to display an edit form for a specified task.
        /// </summary>
        /// <param name="id">The id param is the TaskID passed from the view or retrieved from route data, which is used to query the Tasks table to retrieve the Task details.</param>
        /// <returns>If the id passed in exists and a Task is found, it will return the Edit view for Task with a TaskDTO. If the id was not passed in or the 
        /// task was not found, it will return a NotFound view.</returns>
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

        /// <summary>
        /// EDIT (POST) action handles the update and save functions for a Task.
        /// </summary>
        /// <param name="id">The id param is the TaskID passed from the view or retrieved from route data, which is used to query the Tasks table to retrieve the Task entity.</param>
        /// <param name="task">The task param is the TaskDTO passed from the view, which contains the user bound data.</param>
        /// <returns>If the id param does not match the TaskID on the "task" param, it returns a NotFound View. If the Model State is Invalid, it returns 
        /// the Edit view alongside the "task" TaskDTO. On a successfull update it will return a redirect to the List Task action.</returns>
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

        /// <summary>
        /// DELETE (GET) action is used to display a form that is used to delete a task.
        /// </summary>
        /// <param name="id">The id param corresponds to a TaskID for a Task that a user wants to delete.</param>
        /// <returns>If the user does not pass in an id, it returns a NotFound view. If there is not Task that matches the passed in id, it returns a 
        /// NotFound view. If the id passed in corresponds to a TaskID for a Task, it will return the Delete view for Task along with a TaskDTO.</returns>
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

        /// <summary>
        /// DELETE (POST) action is used to perform the delete function for a task.
        /// </summary>
        /// <param name="id">The id param corresponds to a TaskID for a Task that a user wants to delete.</param>
        /// <returns>A redirect to the List Task action.</returns>
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

        /// <summary>
        /// Loads all Workloads into Select List. Used to Display Select Lists on Create and Edit.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Loads all Frequencies into Select List. Used to Display Select Lists on Create and Edit.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Loads all TaskTypes into Select List. Used to Display Select Lists on Create and Edit.
        /// </summary>
        /// <returns></returns>
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

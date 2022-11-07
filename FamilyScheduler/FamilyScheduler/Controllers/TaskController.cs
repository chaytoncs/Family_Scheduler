using FamilyScheduler.Data;
using Microsoft.AspNetCore.Mvc;
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
            // Get all rows in Tasks table as a list of Task entities.
            // Create list of DTOS to return to the view
            // Query the tasks like we have currently
           // foreach(var task in _context.Tasks)
            //{
            //    TaskDTO tdto = new TaskDTO();
                // set the dto property values
                // 
            //}
            // Change the view to use the DTO instead
            return View(await _context.Tasks.Include(x => x.Workload).Include(x => x.Frequency).Include(x => x.TaskType).ToListAsync());
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

        [Route("Edit/{id}")]
        public IActionResult Edit()
        {
            return View("Edit");
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var task = await _context.Tasks.FindAsync(id);
            return View(task);
        }
    }
}

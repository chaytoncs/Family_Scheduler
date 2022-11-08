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
            // Placeholder code for Lab 6 until I fully implement the DTO for my Models
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

using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("Task")]
    public class TaskController : Controller
    {
        [Route("List")]
        public IActionResult List()
        {
            return View("List");
        }

        [Route("Entry")]
        public IActionResult Entry()
        {
            return View("Entry");
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

        [Route("Delete/{id}")]
        public IActionResult Delete()
        {
            return View("Delete");
        }
    }
}

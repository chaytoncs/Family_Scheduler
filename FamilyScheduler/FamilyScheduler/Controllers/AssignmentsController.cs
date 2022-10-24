using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("Assignments")]
    public class AssignmentsController : Controller
    {
        [Route("List")]
        public IActionResult List()
        {
            return View("List");
        }

        [Route("Dashboard/{id}")]
        public IActionResult DisplayDashboard()
        {
            return View("Dashboard");
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

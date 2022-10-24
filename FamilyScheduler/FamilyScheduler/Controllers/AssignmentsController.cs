using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("Assignments")]
    public class AssignmentsController : Controller
    {
        [Route("List")]
        public IActionResult List()
        {
            // Add logic here that will display correct view for Parent or Household member
            // Parent Renders List, Household member renders Dashboard

            return View("List");
            //return View("Dashboard")'
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

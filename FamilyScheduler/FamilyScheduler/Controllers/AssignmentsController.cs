using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("")]
    [Route("Assignments")]
    [Authorize]
    public class AssignmentsController : Controller
    {
        [Route("")]
        [Route("List")]
        public IActionResult List()
        {
            // Add logic here that will display correct view for Parent or Household member
            // Parent Renders List, Household member renders Dashboard

            return View("List");
            //return View("Dashboard")'
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

        [Route("Delete/{id}")]
        public IActionResult Delete()
        {
            return View("Delete");
        }
    }
}

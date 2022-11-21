using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("Schedule")]
    [Authorize]
    public class ScheduleController : Controller
    {
        [Route("List")]
        public IActionResult List()
        {
            return View("List");
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View("Create");
        }

        [Route("Delete/{id}")]
        public IActionResult Delete()
        {
            return View("Delete");
        }
    }
}

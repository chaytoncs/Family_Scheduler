using Microsoft.AspNetCore.Mvc;

namespace FamilyScheduler.Controllers
{
    [Route("Schedule")]
    public class ScheduleController : Controller
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

        [Route("Delete/{id}")]
        public IActionResult Delete()
        {
            return View("Delete");
        }
    }
}

using FamilyScheduler.Areas.Identity.Data;
using FamilyScheduler.Data;
using FamilyScheduler.Models;
using FamilyScheduler.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;

namespace FamilyScheduler.Controllers
{
    [Route("")]
    [Route("Assignment")]
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly FamilySchedulerContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor
        public AssignmentsController(FamilySchedulerContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("")]
        public async Task<IActionResult> List()
        {
            // Admin/ SuperUser Returns all Assigments
            // Member returns Assigments linked to a UserAccountID
            List<Assignment> assignments;
            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                // Query Entities and related data
                assignments = await _context.Assignments.Include(a => a.Task)
                    .Include(a => a.User)
                    .Include(a => a.Task.Workload)
                    .Include(a => a.Task.Frequency)
                    .Include(a => a.Task.TaskType).ToListAsync();
            } 
            else
            {
                // Checks if user is signed in
                ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
                if (applicationUser == null)
                {
                    return Problem("No user is signed in.");
                }
                // Query Entities and related data specific to a user
                assignments = await _context.Assignments.Where(a => a.UserID == applicationUser.UserAccountID)
                    .Include(a => a.Task)
                    .Include(a => a.User)
                    .Include(a => a.Task.Workload)
                    .Include(a => a.Task.Frequency)
                    .Include(a => a.Task.TaskType).OrderBy(a => a.DueDate).ToListAsync();
            }

            // Transform assignments into a List of Assignment DTOs to be returned to the view
            List<AssignmentDTO> assigmentDTOs = new();
            
            foreach (Assignment a in assignments)
            {
                AssignmentDTO assignmentDTO = new()
                {
                    AssignmentID = a.AssignmentID,
                    TaskID = a.TaskID,
                    UserID = a.UserID,
                    DueDate = a.DueDate,
                    Completed = a.Completed,
                    TaskDescription = a.Task.Description,
                    WorkloadDescription = a.Task.Workload.Description,
                    FrequencyDescription = a.Task.Frequency.Description,
                    TaskTypeDescription = a.Task.TaskType.Description,
                    FullName = $"{a.User.FirstName} {a.User.LastName}"
                };
                assigmentDTOs.Add(assignmentDTO);
            }

            // If Admin/SuperUser display List else Member Dashboard
            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                return View("List", assigmentDTOs);
            }
            return View("Dashboard", assigmentDTOs);
        }

        // GET CREATE
        [Route("Create")]
        [Authorize(Roles = "Admin,SuperUser")]
        public IActionResult Create()
        {
            // Add Tasks and Users to Viewbag to display in Select List
            // Add Lists to ViewBag as a dynamic properties
            ViewBag.TaskList = TaskSelectList();
            ViewBag.MemberList = MemberSelectList();

            return View("Create");
        }

        // POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Create([Bind("TaskID,UserID,DueDate,Completed")] AssignmentDTO assignment)
        {
            // Checks if Model State is Valid
            // If Valid Save to Database else return View with invalid Model
            if (ModelState.IsValid)
            {
                // Transform from DTO to Assignment
                Assignment a = new Assignment
                {
                    TaskID = assignment.TaskID,
                    UserID = assignment.UserID,
                    DueDate = assignment.DueDate,
                    Completed = assignment.Completed,
                };
                // Save Assignment to Database
                _context.Assignments.Add(a);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(assignment);
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            // Logic check to see if ID was passed in and Assignments has data
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            // Query entity and related data based on ID passed in
            var assignment = await _context.Assignments
                .Include(a => a.Task)
                .Include(a => a.User)
                .Include(a => a.Task.Workload)
                .Include(a => a.Task.Frequency)
                .Include(a => a.Task.TaskType)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);

            // Logic check to see if entity was found
            if (assignment == null)
            {
                return NotFound();
            }

            // Checks to see if User is not an admin
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperUser"))
            {
                ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
                if (applicationUser == null)
                {
                    return Problem("No user is signed in.");
                }

                // Checks if user is attempting to access details of an assigment that is not theirs
                if (assignment.UserID != applicationUser.UserAccountID)
                {
                    TempData["ErrorMessage"] = "Page Forbidden: This assignment does not belong to you.";
                    return RedirectToAction(nameof(List));
                }
            }

            // Transform Assignment to DTO
            // Pass Assignment DTO to the View
            AssignmentDTO assignmentDTO = new()
            {
                AssignmentID = assignment.AssignmentID,
                DueDate = assignment.DueDate,
                Completed = assignment.Completed,
                TaskDescription = assignment.Task.Description,
                WorkloadDescription = assignment.Task.Workload.Description,
                FrequencyDescription = assignment.Task.Frequency.Description,
                TaskTypeDescription = assignment.Task.TaskType.Description,
                FullName = $"{assignment.User.FirstName} {assignment.User.LastName}"
            };

            return View(assignmentDTO);
        }

        // GET EDIT
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Logic check to see if ID was passed and Assignments has data
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            // Query entity and include related data.
            var assignment = await _context.Assignments
                .Include(a => a.User)
                .Include(a => a.Task)
                .FirstOrDefaultAsync(t => t.AssignmentID == id);

            // Logic check -- did we find something?
            if (assignment == null)
            {
                return NotFound();
            }

            // Use DTO to pass data to the View.
            AssignmentDTO assignmentDTO = new()
            {
                AssignmentID = assignment.AssignmentID,
                TaskID = assignment.TaskID,
                UserID = assignment.UserID,
                DueDate = assignment.DueDate,
                Completed = assignment.Completed
            };

            // Add Tasks and Users to Viewbag to display in Select List
            // Add Lists to ViewBag as a dynamic properties
            ViewBag.TaskList = TaskSelectList();
            ViewBag.MemberList = MemberSelectList();

            return View(assignmentDTO);
        }

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentID,TaskID,UserID,DueDate,Completed")] AssignmentDTO assignment)
        {
            // Logic check -- do the IDs match
            if (id != assignment.AssignmentID)
            {
                return NotFound();
            }

            // Checks if Model State is Valid
            // If Model State is Valid it will attempt to Update Assignment, else return invalid Assignment DTO to View
            if (ModelState.IsValid)
            {
                try
                {
                    // Pull entity from DB with the provided ID
                    var entity = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentID == id);

                    if(entity != null)
                    {
                        /* Update changes to the entity and since it's being tracked
                         * as we are in a connected state, only SaveChangesAsync is needed
                         * to persist. */

                        entity.TaskID = assignment.TaskID;
                        entity.UserID = assignment.UserID;
                        entity.DueDate = assignment.DueDate;
                        entity.Completed = assignment.Completed;

                        // Save changes
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Checks if Assignment with ID passed in does not exist
                    if (!_context.Assignments.Any(a => a.AssignmentID == id))
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
            return View(assignment);
        }

        // GET DELETE
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Logic check to see if ID is passed in and there are Assignments in DB
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            // Query entity and include related data
            var assignment = await _context.Assignments
                .Include(a => a.Task)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);

            // Logic check to see if something was found
            if (assignment == null)
            {
                return NotFound();
            }

            // Transform to DTO to send to View
            AssignmentDTO assignmentDTO = new AssignmentDTO()
            {
                AssignmentID = assignment.AssignmentID,
                FullName = $"{assignment.User.FirstName} {assignment.User.LastName}",
                TaskDescription= assignment.Task.Description,
                DueDate = assignment.DueDate,
            };
            return View(assignmentDTO);
        }

        // POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin,SuperUser")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            // Checks if Assignments has data
            if (_context.Assignments == null)
            {
                return Problem("Entity set 'FamilySchedulerContext.Assignments'  is null.");
            }
            
            // Query entity
            var assignment = await _context.Assignments.FindAsync(id);

            // Ensure entity is found before attempting to remove
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        // GET Complete
        // Used for Household members to mark assignments complete
        [Route("Complete/{id}")]
        public async Task<IActionResult> Complete(int? id)
        {
            // Temporary Role Check until I implement Member Role for Authorization Attribute
            // Stop Admins from accessing this function
            if(User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                TempData["ErrorMessage"] = "Page Forbidden: This is a household member function.";
                return RedirectToAction(nameof(List));
            }

            // Logic check to see if ID was passed and Assignments has data
            if (id == null || _context.Assignments == null)
            {
                return NotFound();
            }

            // Query entity and include related data.
            var assignment = await _context.Assignments
                .Include(a => a.Task)
                .Include(a => a.User)
                .Include(a => a.Task.Workload)
                .Include(a => a.Task.Frequency)
                .Include(a => a.Task.TaskType)
                .FirstOrDefaultAsync(m => m.AssignmentID == id);

            // Logic check -- did we find something?
            if (assignment == null)
            {
                return NotFound();
            }

            // Transform Assignment to DTO
            // Pass Assignment DTO to the View
            AssignmentDTO assignmentDTO = new()
            {
                AssignmentID = assignment.AssignmentID,
                UserID = assignment.UserID,
                TaskID = assignment.TaskID,
                DueDate = assignment.DueDate,
                Completed = assignment.Completed,
                TaskDescription = assignment.Task.Description,
                WorkloadDescription = assignment.Task.Workload.Description,
                FrequencyDescription = assignment.Task.Frequency.Description,
                TaskTypeDescription = assignment.Task.TaskType.Description,
                FullName = $"{assignment.User.FirstName} {assignment.User.LastName}"
            };
            return View(assignmentDTO);
        }

        // POST Complete
        // Used for Household members to mark assignments complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Complete/{id}")]
        public async Task<IActionResult> PerformComplete(int id, [Bind("AssignmentID,TaskID,UserID,DueDate,Completed")] AssignmentDTO assignment)
        {
            // Temporary Role Check until I implement Member Role for Authorization Attribute
            // Stop Admins from accessing this function
            if (User.IsInRole("Admin") || User.IsInRole("SuperUser"))
            {
                TempData["ErrorMessage"] = "Page Forbidden: This is a household member function.";
                return RedirectToAction(nameof(List));
            }

            // Logic check -- do the IDs match
            if (id != assignment.AssignmentID)
            {
                return NotFound();
            }

            // Checks if Model State is Valid
            // If Model State is Valid it will attempt to Update Assignment, else return invalid Assignment DTO to View
            if (ModelState.IsValid)
            {
                try
                {
                    // Pull entity from DB with the provided ID
                    var entity = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentID == id);

                    if (entity != null)
                    {
                        /* Update changes to the entity and since it's being tracked
                         * as we are in a connected state, only SaveChangesAsync is needed
                         * to persist. */

                        entity.TaskID = assignment.TaskID;
                        entity.UserID = assignment.UserID;
                        entity.DueDate = assignment.DueDate;
                        entity.Completed = true;

                        // Save changes
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Checks if Assignment with ID passed in does not exist
                    if (!_context.Assignments.Any(a => a.AssignmentID == id))
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
            return View(assignment);
        }


        // Loads all Members into Select List
        // Used to Display Select Lists on Create and Edit
        private List<SelectListItem> MemberSelectList()
        {
            var householdMembers = _context.Users.ToList();
            var memberList = new List<SelectListItem>();

            foreach (User u in householdMembers)
            {
                memberList.Add(new SelectListItem { Value = u.UserID.ToString(), Text = $"{u.FirstName} {u.LastName}" });
            }
            return memberList;
        }

        // Loads all Tasks into Select List
        // Used to Display Select Lists on Create and Edit
        private List<SelectListItem> TaskSelectList()
        {
            var tasks = _context.Tasks.ToList();
            var taskList = new List<SelectListItem>();

            foreach (FamilyScheduler.Models.Task t in tasks)
            {
                taskList.Add(new SelectListItem { Value = t.TaskID.ToString(), Text = t.Description });
            }
            return taskList;
        }
    }
}

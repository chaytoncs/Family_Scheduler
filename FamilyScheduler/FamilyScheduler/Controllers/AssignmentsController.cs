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
    /// <summary>
    /// The Assignment Controller is used by Admins and Members to display Assignment related forms/views (CRUD), as well as, performing CRUD functions for Assignments.
    /// </summary>
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

        /// <summary>
        /// The List action for Assignments is used to display the List view (for Admin) or Dashboard (Member). The list view contains 
        /// all of the assignments (for Admin) or Assignments linked to a specific Member (for Member).
        /// </summary>
        /// <returns>The List view for Assignments along with all assignments transformed to AssignmentDTOs (for Admin). 
        /// The Dashboard view for Assignments along with all the assigments linked to the signed in user transformed to AssignmentDTOs (for Member).
        /// </returns>
        [Route("")]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> List()
        {
            // Admin/ SuperUser Returns all Assigments
            // Member returns Assigments linked to a UserAccountID
            List<Assignment> assignments;
            if (User.IsInRole("Admin"))
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
            if (User.IsInRole("Admin"))
            {
                return View("List", assigmentDTOs);
            }
            return View("Dashboard", assigmentDTOs);
        }

        /// <summary>
        /// CREATE (GET) action for Assignments is used to display a form for Admins to Create new Assignments. 
        /// Select List items for Tasks and Members are added to the ViewBag.
        /// </summary>
        /// <returns>If there are members and tasks it will return the Create view for Assignments. If there are no members or tasks it will return
        /// a redirect to the List Action for Assigments with an error message.</returns>
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Verify there are Members and Tasks before attempting to add an Assignment
            var memberCount = _userManager.GetUsersInRoleAsync("Member").Result.Count();
            if(memberCount < 1)
            {
                TempData["ErrorMessage"] = "Error: You must have Household Members before adding an Assignment.";
                return RedirectToAction(nameof(List));
            }

            if (!_context.Tasks.Any())
            {
                TempData["ErrorMessage"] = "Error: You must have Tasks before adding an Assignment.";
                return RedirectToAction(nameof(List));
            }
            // Add Tasks and Users to Viewbag to display in Select List
            // Add Lists to ViewBag as a dynamic properties
            ViewBag.TaskList = TaskSelectList();
            ViewBag.MemberList = MemberSelectList();

            return View("Create");
        }

        /// <summary>
        /// CREATE (POST) action is used to perform the function of creating and saving a new assignment to the database.
        /// </summary>
        /// <param name="assignment">The assignment param is the AssignmentDTO from the create view, which contains the user bound Assignment information.</param>
        /// <returns>If the Model State is invalid it will return the Create view with the "assigment" AssignmentDTO. If the creation is successful, it will 
        /// return a redirect to the List view for Assignments.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// The Details action for Assigments is used to display the details for a specified Assignment.
        /// </summary>
        /// <param name="id">The id param corresponds to an AssigmentID of an Assignment that the user wants to retrieve details for.</param>
        /// <returns>If the id is not passed in or there is no Assignment that corresponds to the id, it will return a NotFound View. If the Assignment is found based
        /// on the passed in id, it will return the Details view for Assignments along with a Assigment DTO.</returns>
        [Route("Details/{id}")]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> Details(int? id)
        {
            // Logic check to see if ID was passed in
            if (id == null)
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
            if (!User.IsInRole("Admin"))
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

        /// <summary>
        /// EDIT (GET) action is used to display a form for an Admin to edit the details of an Assignment.
        /// </summary>
        /// <param name="id">The id param corresponds to an AssigmentID of an Assignment that the user wants to retrieve details for.</param>
        /// <returns>If the id is not passed in or there is no Assignment that corresponds to the id, it will return a NotFound View. If the Assignment is found based
        /// on the passed in id, it will return the Edit view for Assignments along with a Assigment DTO.</returns>
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Logic check to see if ID was passed in
            if (id == null)
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

        /// <summary>
        /// EDIT (POST) action is used to perform the updates to a specified Assignment.
        /// </summary>
        /// <param name="id">The id param is the AssignmentID passed from the view or retrieved from route data, which is used to query the Assigments table to retrieve the Assignment entity.</param>
        /// <param name="assignment">The assignment param is the AssignmentDTO passed from the view, which contains the user bound data.</param>
        /// <returns>If the id param does not match the AssigmentID on the "assignment" param, it returns a NotFound View. If the Model State is Invalid, it returns 
        /// the Edit view alongside the "assigment" AssignmentDTO. On a successfull update it will return a redirect to the List Assigment action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// DELETE (GET) action is used to display a form that is used to delete an Assigment.
        /// </summary>
        /// <param name="id">The id param corresponds to a AssignmentID for a Assignment that a user wants to delete.</param>
        /// <returns>If the user does not pass in an id, it returns a NotFound view. If there is not Assignment that matches the passed in id, it returns a 
        /// NotFound view. If the id passed in corresponds to a AssignmentID for a Assignment, it will return the Delete view for Assignment along with a AssignmentDTO.</returns>
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Logic check to see if ID is passed in
            if (id == null)
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

        /// <summary>
        /// DELETE (POST) action is used to perform the delete function for a Assignment.
        /// </summary>
        /// <param name="id">The id param corresponds to a AssignmentID for a Assignment that a user wants to delete.</param>
        /// <returns>A redirect to the List Assignment action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
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

        /// <summary>
        /// COMPLETE (GET) action displays a form that allows users to the completion for an Assignment.
        /// </summary>
        /// <param name="id">The id param corresponds to a AssignmentID for a Assignment that a user wants to confirm completion.</param>
        /// <returns>If the id is not passed in or if the id does not correspond to an Assignment, it returns a NotFound view.
        /// If a Member attempts to access Complete for an Assignment that is not associated with their UserID, it will return a Redirect to the List Assignment action with an error
        /// message stored in temp data. If the Assignment that corresponds to the id passed in and it belongs to the user (or if user is Admin), it returns the Complete view for
        /// Assignments along with an AssignmentDTO.</returns>
        [Route("Complete/{id}")]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> Complete(int? id)
        {
            // Logic check to see if ID was passed and Assignments has data
            if (id == null)
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

            // Checks to see if User is not an admin
            if (!User.IsInRole("Admin"))
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

                // Checks if user is attempting to confirm completion on a task that is due on a future date
                if (assignment.DueDate > DateTime.Today)
                {
                    TempData["ErrorMessage"] = "Error: You cannot complete an Assigned Task due on a future date.";
                    return RedirectToAction(nameof(List));
                }
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

        /// <summary>
        /// COMPLETE (POST) action is used to update the Completed field on an Assignment.
        /// </summary>
        /// <param name="id">The id param corresponds to a AssignmentID for a Assignment that a user wants to confirm completion.</param>
        /// <param name="assignment">The assignment param is the AssignmentDTO passed from the view, which contains the user bound data.</param>
        /// <returns>If the id param does not match the AssigmentID on the assignment param, it will return a NotFound view. If the Model State is invalid, it will return 
        /// the Complete view for Assignments along with the "assignment" Assigment DTO. If the update to the Completed field is successful, it will return a Redirect
        /// to the List Assignments action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Complete/{id}")]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> PerformComplete(int id, [Bind("AssignmentID")] AssignmentDTO assignment)
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

                    if (entity != null)
                    {
                        /* Update changes to the entity and since it's being tracked
                         * as we are in a connected state, only SaveChangesAsync is needed
                         * to persist. */
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


        /// <summary>
        /// Loads all Members into Select List. Used to Display Select Lists on Create and Edit
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> MemberSelectList()
        {
            var memberList = new List<SelectListItem>();

            // Filters out any user that is not a Member
            var members = _userManager.GetUsersInRoleAsync("Member").Result;
            foreach (var user in members)
            {
                User member = _context.Users.Where(u => u.UserID == user.UserAccountID).First();
                if (member != null)
                {
                    memberList.Add(new SelectListItem { Value = member.UserID.ToString(), Text = $"{member.FirstName} {member.LastName}" });
                }
            }
            return memberList;
        }

        /// <summary>
        /// Loads all Tasks into Select List. Used to Display Select Lists on Create and Edit.
        /// </summary>
        /// <returns></returns>
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

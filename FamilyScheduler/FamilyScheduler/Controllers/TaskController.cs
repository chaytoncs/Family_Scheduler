﻿using FamilyScheduler.Data;
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
            return View(await _context.Tasks.ToListAsync());
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

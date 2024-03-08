using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data;
using Task_Mangment_Api.DTO;
using Task_Mangment_Api.Models;

namespace Task_Mangment_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TasksController(AppDbContext context,IMapper mapper,UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _context.Tasks.OrderByDescending(t => t.DueDate).ToListAsync();

            return Ok(tasks);
        }
        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromForm]TaskDTO DTO)
        {
            DTO.DueDate = DateTime.Now;
            //var task =_mapper.Map<Models.Task>(DTO);
            var task = new Models.Task
            {
                Title = DTO.Title,
                Description = DTO.Description,
                DueDate = DateTime.Now,
                IsFinished = DTO.IsFinished,
            };
            var user = await _userManager.FindByIdAsync(DTO.UserID);
            var UserName = user.UserName;

            var userTask = new UserTask
            {
                User = user,
                Task = task
            };
            task.AssignedUsers.Add(userTask);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
        [HttpGet("/AssignedTasks")]
        [Authorize]
        public async Task<ActionResult> GetTaskAssigned(int id)
        {
            var FilteredTasks = await _context.Tasks
                .Where(task => task.IsFinished == true)
                .ToListAsync();

            if (FilteredTasks == null)
            {
                return NotFound();
            }

            return Ok(FilteredTasks);
        }
        [HttpGet("/CreatedTasks")]
        [Authorize]
        public async Task<ActionResult> GetTaskCreated(int id)
        {
            var FilteredTasks = await _context.Tasks.ToListAsync();

            if (FilteredTasks == null)
            {
                return NotFound();
            }

            return Ok(FilteredTasks);
        }
        // PUT: api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, bool IsFinished)
        {

            var task = await _context.Tasks.FindAsync(id);
            try
            {
                task.IsFinished = IsFinished;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Done");
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // Require "Admin" role for this endpoint
        public async Task<IActionResult> GetAllTasks()
        {
            // Implement logic to retrieve all tasks (admin-only)
            var allTasks = await _context.Tasks.ToListAsync();

            return Ok(allTasks);
        }
        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}


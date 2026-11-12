using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using практика_8.Models;
using Task = практика_8.Models.Task;

namespace практика_8.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    [Authorize]
    public class TaskController:ControllerBase
    {
        private readonly JwtBdContext _tasks;

        public TaskController(JwtBdContext tasks)
        {
            _tasks = tasks;
        }

        public record TaskRequest(string Title, string Description);

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = _tasks.Tasks.AsQueryable();

            if (!User.IsInRole("Admin"))
                query = query.Where(t => t.OwnerLogin == User.Identity!.Name);

            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _tasks.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (!User.IsInRole("Admin") && task.OwnerLogin != User.Identity!.Name)
                return Forbid();

            return Ok(task);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Create(TaskRequest request)
        {
            var task = new Task
            {
                Title = request.Title,
                Description = request.Description,
                OwnerLogin = User.Identity!.Name!,
                IsCompleted = false
            };

            _tasks.Tasks.Add(task);
            await _tasks.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(int id, TaskRequest request)
        {
            var task = await _tasks.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (!User.IsInRole("Admin") && task.OwnerLogin != User.Identity!.Name)
                return Forbid();

            task.Title = request.Title;
            task.Description = request.Description;
            await _tasks.SaveChangesAsync();
            return Ok(task);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _tasks.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (!User.IsInRole("Admin") && task.OwnerLogin != User.Identity!.Name)
                return Forbid();

            _tasks.Tasks.Remove(task);
            await _tasks.SaveChangesAsync();
            return NoContent();
        }
    }
}

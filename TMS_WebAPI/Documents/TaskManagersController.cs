using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS_WebAPI.Models;

namespace TMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskManagersController : ControllerBase
    {
        private readonly TMS_WebAPIContext _context;
        private readonly ILog _log;


        public TaskManagersController(TMS_WebAPIContext context, ILog log)
        {
           

            _context = context;
            _log = log;
        }

        // GET: api/TaskManagers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskManager>>> GetTaskManager()
        {
           
            try
            {

                return await _context.TaskManager.ToListAsync();

            }
            catch (Exception ex)
            {
                _log.info($"error in // GET: api/TaskManagers: {ex.Message}", _context);
                return null;

            }
        }

        // GET: api/TaskManagers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskManager>> GetTaskManager(int id)
        {
            

            try
            {

                var taskManager = await _context.TaskManager.FindAsync(id);

                if (taskManager == null)
                {
                    return NotFound();
                }

                return taskManager;

            }
            catch (Exception ex)
            {
                _log.info($"error in // GET: api/TaskManagers/5 : {ex.Message}", _context);
                return null;

            }

        }

        // PUT: api/TaskManagers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskManager(int id, TaskManager taskManager)
        {
           
            try
            {

                if (id != taskManager.Id)
                {
                    return BadRequest();
                }


                _context.Entry(taskManager).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!TaskManagerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _log.info($"error in // PUT: api/TaskManagers/5 : {ex.Message}", _context);

                        throw;

                    }
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                _log.info($"error in // PUT: api/TaskManagers/5 : {ex.Message}", _context);
                return null;

            }

        }

        // POST: api/TaskManagers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
      
        public async Task<ActionResult<TaskManager>> PostTaskManager(TaskManager taskManager)
        {
            _context.TaskManager.Add(taskManager);
            await _context.SaveChangesAsync();

            var subtask = await _context.TaskManager.FindAsync(taskManager.Id);

            taskManager.ParentId = taskManager.Id;
            _context.Entry(taskManager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _log.info($"error in // POST: api/TaskManagers : {ex.Message}", _context);
                return null;
            }

            return CreatedAtAction("GetTaskManager", new { id = taskManager.Id }, taskManager);

        }

        // DELETE: api/TaskManagers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskManager>> DeleteTaskManager(int id)
        {
           
            try
            {

                var taskManager = await _context.TaskManager.FindAsync(id);
                if (taskManager == null)
                {
                    return NotFound();
                }

                _context.TaskManager.Remove(taskManager);
                await _context.SaveChangesAsync();

                return taskManager;


            }
            catch (Exception ex)
            {
                _log.info($"error in // DELETE: api/TaskManagers/5 : {ex.Message}", _context);
                return null;

            }

        }

        private bool TaskManagerExists(int id)
        {
            return _context.TaskManager.Any(e => e.Id == id);
        }
    }
}

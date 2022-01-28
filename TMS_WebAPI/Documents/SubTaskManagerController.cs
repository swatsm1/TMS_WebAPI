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
    public class SubTaskManagerController : ControllerBase
    {
        private readonly TMS_WebAPIContext _context;
        private readonly ILog _log;

       
        public SubTaskManagerController(TMS_WebAPIContext context, ILog log)
        {
            _context = context;
            _log = log;
        }

        // GET: api/SubTaskManager
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskManager>>> GetTaskManager()
        {
            
            try
            {

                return await _context.TaskManager.ToListAsync();

            }
            catch (Exception ex)
            {
                _log.info($"error in GetTaskManager : {ex.Message}", _context);
                return null;

            }


        }



        // GET: api/SubTaskManager/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TaskManager>>> GetTaskManagerAsync(int id)
        {
            try
            {
                var result = _context.TaskManager.Where(s => s.ParentId == id && s.TaskType == TaskStatusType.SubTask).ToList();

                await updateStateAsync(result, id);

                _log.info($" GET: api/SubTaskManager/5 ok {id}", _context);


                return result;
            }
            catch (Exception ex)
            {
                _log.info($"error in  // GET: api/SubTaskManager/5 : {ex.Message}", _context);
                return null;

            }

            
        }

        private async Task updateStateAsync(IEnumerable<TaskManager> result, int id)
        {

            try
            {

                TaskManager parent = _context.TaskManager.Where(s => s.Id == id).FirstOrDefault();


                int countSubTasks = result.Where(s => s.TaskType == TaskStatusType.SubTask).Count();

                if (countSubTasks == 0)
                {
                    return;
                }

                int countcompleted = result.Where(s => s.TaskState == TaskState.Completed && s.TaskType == TaskStatusType.SubTask).Count();
                int countinprogress = result.Where(s => s.TaskState == TaskState.inProgress && s.TaskType == TaskStatusType.SubTask).Count();

                if (countcompleted == countSubTasks)
                {
                    parent.TaskState = TaskState.Completed;

                }
                else if (countinprogress > 0)
                {
                    parent.TaskState = TaskState.inProgress;
                }
                else parent.TaskState = TaskState.Planned;


                _context.Entry(parent).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    _log.info($"error in updateStat : {ex.Message}", _context);
                }

            }
            catch (Exception ex)
            {
                _log.info($"error in updateState : {ex.Message}", _context);
                

            }

        }
            

        // PUT: api/SubTaskManager/5
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
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskManagerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();


            }
            catch (Exception ex)
            {
                _log.info($"error in GetTaskManager : {ex.Message}", _context);
                return null;

            }

           
        }

        // POST: api/SubTaskManager
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TaskManager>> PostTaskManager(TaskManager taskManager)
        {
           

            try
            {

                _context.TaskManager.Add(taskManager);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTaskManager", new { id = taskManager.Id }, taskManager);


            }
            catch (Exception ex)
            {
                _log.info($"error in GetTaskManager : {ex.Message}", _context);
                return null;

            }

        }




        // DELETE: api/SubTaskManager/5
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
                _log.info($"error in GetTaskManager : {ex.Message}", _context);
                return null;

            }

        }

        private bool TaskManagerExists(int id)
        {
            return _context.TaskManager.Any(e => e.Id == id);
        }
    }
}

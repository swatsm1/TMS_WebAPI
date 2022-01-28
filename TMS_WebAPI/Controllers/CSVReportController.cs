using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS_WebAPI.Models;
using TMS_WebAPI.Services;

namespace TMS_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVReportController : ControllerBase
    {
        
        private readonly IDal _Dal;
        
        /// <summary>
        /// Inject data access layer
        /// </summary>
        /// <param name="Dal"></param>
        
        
        public CSVReportController(IDal Dal)
        {
            
            _Dal = Dal;
        }

        /// <summary>
        /// Get Taskmanager rows to a List
        /// </summary>
        /// <returns>IEnumerable<TaskManager>></returns>
        // GET: api/CSVReport
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskManager>>> GetTaskManager()
        {

            try
            {
               
                return await _Dal.GetTaskManagers();

            }
            catch (Exception ex)
            {

                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.GetBaseException() }");

            }

        }

        // GET: api/CSVReport/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskManager>> GetTaskManager(int id)
        {
            try
            {

                var taskManager = await _Dal.GetTaskManager(id);

                if (taskManager == null)
                {
                    return NotFound();
                }

                return taskManager;

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.GetBaseException() }");

            }
        }

        // PUT: api/CSVReport/5
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


                await _Dal.PutTaskManager(id, taskManager);

                return NoContent();

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.GetBaseException() }");

            }
        }

        // POST: api/CSVReport
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TaskManager>> PostTaskManager(TaskManager taskManager)
        {
            try
            {

                await _Dal.PostTaskManager(taskManager);

                return CreatedAtAction("GetTaskManager", new { id = taskManager.Id }, taskManager);

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.GetBaseException() }");

            }
        }

        // DELETE: api/CSVReport/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskManager>> DeleteTaskManager(int id)
        {
            try
            {

                await _Dal.DeleteTaskManager(id);

                var taskManager = await _Dal.GetTaskManager(id);
                if (taskManager == null)
                {
                    // return NotFound();
                }

                return taskManager;

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.GetBaseException() }");

            }
        }
    }
}

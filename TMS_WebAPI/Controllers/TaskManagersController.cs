using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class TaskManagersController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IDal _Dal;
        /// <summary>
        /// Inject Data Access Layer
        /// </summary>
        /// <param name="Dal"></param>
        public TaskManagersController(IDal Dal)
        {

            _Dal = Dal;
        }
        /// <summary>
        /// Get all Tasks
        /// </summary>
        /// <returns><IEnumerable<TaskManager>></returns>
        // GET: api/TaskManagers
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        /// / GET: api/TaskManagers/search/{0}
        [HttpGet, Route("search/{search=search}")]
        public async Task<ActionResult<IEnumerable<TaskManager>>> FilterGetTaskManager(string search)
        {


            try
            {

                return await _Dal.FilterGetTaskManagers(search);

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
        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TaskManager</returns>
        // GET: api/TaskManagers/5
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

        /// <summary>
        /// Update Task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskManager"></param>
        /// <returns></returns>
        // PUT: api/TaskManagers/5

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

        /// <summary>
        /// Add New Task
        /// </summary>
        /// <param name="taskManager"></param>
        /// <returns></returns>
        // POST: api/TaskManagers

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

                if (ex.GetType()==typeof(TaskExistsException))
                {

                    return UnprocessableEntity($"Task { taskManager.TaskName } Already Exists");

                    //var message = string.Format("Product with id = {0} not found", id);
                    //HttpError err = new HttpError(message);

                    //return Request.CreateErrorResponse(HttpStatusCode.NotFound, message);

                    //var resp = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
                    //{
                    //    Content = new StringContent($"Task { taskManager.TaskName } Already Exists"),
                    //    ReasonPhrase = "Task Exists"
                    // };

                    //throw new HttpResponseException(resp);
                    // throw new TaskExistsException($" Task {taskManager.TaskName} Already Exists ");
                }

                if (!string.IsNullOrEmpty(ExceptionHandler.GetExceptionMessage()))
                {
                    throw new Exception($"Error { ExceptionHandler.GetException() }");
                }
                else

                    throw new Exception($"Error { ex.Message }");

            }

        }

        /// <summary>
        /// Delete Task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/TaskManagers/5
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

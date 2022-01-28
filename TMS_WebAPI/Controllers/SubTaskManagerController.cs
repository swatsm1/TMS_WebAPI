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
    public class SubTaskManagerController : ControllerBase
    {
        
        /// <summary>
        /// Implement
        /// </summary>
        private readonly IDal _Dal;
        /// <summary>
        /// Inject Data Access Layer
        /// </summary>
        /// <param name="Dal"></param>
        public SubTaskManagerController(IDal Dal)
        {
            _Dal = Dal;

        }

        /// <summary>
        /// Get Alltasks
        /// </summary>
        /// <returns <IEnumerable<TaskManager>> ></returns>

        // GET: api/SubTaskManager
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
        /// Get all Subtasks
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/SubTaskManager/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<TaskManager>>> GetTaskManagerAsync(int id)
        {
            try
            {

                var result = await _Dal.GetSubTaskManager(id);

                return result;
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
        /// Add new SubTask
        /// </summary>
        /// <param name="taskManager"></param>
        /// <returns>TaskManager</returns>
        public async Task<ActionResult<TaskManager>> PostTaskManager(TaskManager taskManager)
        {

            try
            {

                await _Dal.PostSubTaskManager(taskManager);

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
    }
}

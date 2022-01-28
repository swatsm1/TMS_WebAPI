using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMS_WebAPI.Models;

namespace TMS_WebAPI.Services
{
    /// <summary>
    /// Data Access Layer
    /// </summary>
    public class Dal : IDal 
    {
        
        private readonly TMS_WebAPIContext _context;
        
        /// <summary>
        /// Injected WebAPI DataContext
        /// </summary>
        /// <param name="ctx"></param>
        public Dal(TMS_WebAPIContext ctx)
        {
            _context = ctx;
        }
        /// <summary>
        /// Get Taskmanager rows to a List
        /// </summary>
        /// <returns><List<TaskManager></returns>
        public async Task<List<TaskManager>> GetTaskManagers()
        {

            try
            {
          
                return await _context.TaskManager.ToListAsync();
            }
            catch (Exception ex)
            {

               throwException(ex);
               throw new Exception($"Error { ex.GetBaseException() }");
            }

        }
        /// <summary>
        /// Get Taskmanager rows to a List via search argument
        /// </summary>
        /// <param name="search"></param>
        /// <returns><List<TaskManager></returns>
        public async Task<List<TaskManager>> FilterGetTaskManagers(string search)
        {

            try
            {
                Regex regex = new Regex("[0-9]");

                List<TaskManager> result = null; 

                if (regex.IsMatch(search))
                {
                    result = _context.TaskManager.Where(s => s.ParentId.Equals(Convert.ToInt32(search))).ToList();
                    //true
                }
                else
                {
                    result = _context.TaskManager.Where(s => s.TaskName.Contains(search)
                      || s.Description.Contains(search)).ToList();

                }

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {

                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");
            }

        }

        private void throwException(Exception ex)
        {

            ExceptionHandler.ClearException();
            ExceptionHandler.appException = ex;
            ExceptionHandler.exceptionMessage = ex.Message;


        }

        /// <summary>
        /// Get single TaskManager Row
        /// </summary>
        /// <param name="id"></param>
        /// <returns><TaskManager></returns>
        public async Task<TaskManager> GetTaskManager(int id)
        {

            try
            {

                var taskManager = await _context.TaskManager.FindAsync(id);

                if (taskManager == null)
                {
                }

                return taskManager;

            }
            catch (Exception ex)
            {
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");

            }

        }
        /// <summary>
        /// Get List of SubTasks and pass results updateStateAsync
        /// </summary>
        /// <param name="id"></param>
        /// <returns><List<TaskManager></returns>
        public async Task<ActionResult<List<TaskManager>>> GetSubTaskManager(int id)
        {

            try
            {
                var result = _context.TaskManager.Where(s => s.ParentId == id && s.TaskType == TaskStatusType.SubTask).ToList();

                await updateStateAsync(result, id);

                await info($" GET: api/SubTaskManager/5 ok {id}");


                return result;
            }
            catch (Exception ex)
            {
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");

            }
        }
        /// <summary>
        /// Update Task States
        /// </summary>
        /// <param name="result"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task updateStateAsync(List<TaskManager> result, int id)
        {

            try
            {
                //Get Parent Task
                TaskManager parent = _context.TaskManager.Where(s => s.Id == id).FirstOrDefault();

                //Get Subtask Count
                int countSubTasks = result.Where(s => s.TaskType == TaskStatusType.SubTask).Count();

                if (countSubTasks == 0)
                {
                    return;
                }

                //Count completed
                int countcompleted = 
                  result.Where(s => s.TaskState == TaskState.Completed && s.TaskType == TaskStatusType.SubTask).Count();

                //Count inprogress
                int countinprogress = 
                  result.Where(s => s.TaskState == TaskState.inProgress && s.TaskType == TaskStatusType.SubTask).Count();

                //if countcompleted == countSubTasks then Parent. Taskstate= completed
                if (countcompleted == countSubTasks)
                {
                    parent.TaskState = TaskState.Completed;

                }
                //elseif countinprogress > 0 Parent Taskstate = inProgress
                else if (countinprogress > 0)
                {
                    parent.TaskState = TaskState.inProgress;
                }
                //else  parent.TaskState = TaskState.Planned
                else parent.TaskState = TaskState.Planned;


                _context.Entry(parent).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();

                    await info($" updateStateAsync ok {id}");

                }
                catch (DbUpdateConcurrencyException ex)
                {

                    throwException(ex);
                    throw new Exception($"Error { ex.GetBaseException() }");
                }

            }
            catch (Exception ex)
            {
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");
            }
        }

        /// <summary>
        /// Update TASK
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskManager"></param>
        /// <returns></returns>
        public async Task PutTaskManager(int id, TaskManager taskManager)
        {

            try
            {

                if (id != taskManager.Id)
                {
                   
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
                        //return NotFound();
                    }
                    else
                    {
                        await info($"error in // PUT: api/TaskManagers/5 : {ex.Message}");

                        throwException(ex);
                        throw new Exception($"Error { ex.GetBaseException() }");

                    }
                }

                return ;

            }
            catch (Exception ex)
            {
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");

            }

        }
        /// <summary>
        /// if TaskManagerExists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TaskManagerExists(int id)
        {
            return _context.TaskManager.Any(e => e.Id == id);
        }

        /// <summary>
        /// Add New Task
        /// </summary>
        /// <param name="taskManager"></param>
        /// <returns>TaskManager</returns>
        public async Task<TaskManager> PostTaskManager(TaskManager taskManager)
        {

            var result = _context.TaskManager.Where(s => s.TaskName == taskManager.TaskName).ToList();
            //Get task Count
            int countTasks = result.Where(s => s.TaskType == TaskStatusType.Task).Count();

            if (countTasks > 0)
            {
                await info($"error in // POST: api/TaskManagers : Task Already Exists");
                throw new TaskExistsException($" Task {taskManager.TaskName} Already Exists ");
            }

            _context.TaskManager.Add(taskManager);
            await _context.SaveChangesAsync();

            var subtask = await _context.TaskManager.FindAsync(taskManager.Id);

            taskManager.ParentId = taskManager.Id;
            _context.Entry(taskManager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await info($"error in // POST: api/TaskManagers : {ex.Message}");
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");
                
            }

            return taskManager;

        }
        // POST: api/SubTaskManager
        /// <summary>
        /// Add new SubTask
        /// </summary>
        /// <param name="taskManager"></param>
        /// <returns></returns>
        public async Task<TaskManager> PostSubTaskManager(TaskManager taskManager)
        {
           
            try
            {
                _context.TaskManager.Add(taskManager);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await info($"error in // POST: api/SubTaskManager : {ex.Message}");
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");
               
            }

            return taskManager;

        }

        /// <summary>
        /// Delete TaskManager
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TaskManager> DeleteTaskManager(int id)
        {

            try
            {

                var taskManager = await _context.TaskManager.FindAsync(id);

                if (string.IsNullOrEmpty(taskManager.ParentId.ToString()))
                {


                   

                }
                else
                {
                    if (taskManager.ParentId > 0)
                    {

                        var result = _context.TaskManager.Where(s => s.TaskName == taskManager.TaskName).ToList();
                        //Get task Count
                        int countTasks = result.Where(s => s.TaskType == TaskStatusType.SubTask).Count();

                        if (countTasks > 0)
                        {
                            await info($"error in // POST: api/TaskManagers : Attempting to delete parent task before removing subtask");
                            throw new TaskExistsException($" Task {taskManager.TaskName} Attempting to delete parent task before removing subtask ");
                        }

                    }
                }


                if (taskManager == null)
                {
                    return taskManager;
                }

                _context.TaskManager.Remove(taskManager);
                await _context.SaveChangesAsync();

                return taskManager;


            }
            catch (Exception ex)
            {
                await info($"error in // DELETE: api/TaskManagers/5 : {ex.GetBaseException()}");
                throwException(ex);
                throw new Exception($"Error { ex.GetBaseException() }");
               

            }
        }
        /// <summary>
        /// Update Logger
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task info(string str)

        {
            try
            {
                Logger mylog = new Logger
                {
                    logDatTime = DateTime.Now,
                    logText = str
                };

                _context.Logger.Add(mylog);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ExceptionHandler.appException = ex;
                ExceptionHandler.exceptionMessage = ex.Message;

                throw new Exception($"Error { ex.GetBaseException() }");
            }
        }
    }

    /// <summary>
    /// Data Access Layer Interface
    /// </summary>
    public interface IDal
    {
        
        Task<List<TaskManager>> GetTaskManagers();
        Task<List<TaskManager>> FilterGetTaskManagers(string search);

        Task<TaskManager> GetTaskManager(int id);
        Task PutTaskManager(int id, TaskManager taskManager);
        Task<TaskManager> PostTaskManager(TaskManager taskManager);
        Task<TaskManager> DeleteTaskManager(int id);
        Task<ActionResult<List<TaskManager>>> GetSubTaskManager(int id);
        Task<TaskManager> PostSubTaskManager(TaskManager taskManager);
        Task info(string str);

    }
}

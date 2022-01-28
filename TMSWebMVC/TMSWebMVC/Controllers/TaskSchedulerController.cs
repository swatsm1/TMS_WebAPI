using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMSWebMVC.Class;
using TMSWebMVC.Models;
using TMSWebMVC.Service;

namespace TMSWebMVC.Controllers
{
    public class TaskSchedulerController : Controller
    {
        private readonly ClientService _client;
        /// <summary>
        /// Inject Client Service
        /// </summary>
        /// <param name="client"></param>
        public TaskSchedulerController(ClientService client)
        {
            _client = client;
            _client.BaseAddress = client._clientUrl;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<TaskManager> Tasks = null;

                //HTTP GET
                var responseTask = _client.GetAsync("api/TaskManagers");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = await result.Content.ReadAsStringAsync();

                    Tasks = JsonConvert.DeserializeObject<IList<TaskManager>>(readTask);
                    return View(Tasks);
                }
                else //web api sent error response 
                {

                    //log response status here..

                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }

                    return View(Tasks);
                }
            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.Message }");
                    return RedirectToAction("500", "Error");
                }
                else

                    return RedirectToAction("500", "Error", ex.Message);

            }
        }

        /// <summary>
        /// Return View with list of Tasks
        /// </summary>
        /// <returns></returns>
        // GET: TaskScheduler
        public async Task<IActionResult> FilterIndex(string SearchString)
        {
            try
            {
                
                    IEnumerable<TaskManager> Tasks = null;

                    if (String.IsNullOrEmpty(SearchString))
                    {
                        ModelState.AddModelError("SearchString", "SearchString must contain a value.");
                        return View(Tasks);
                    }
                    

                    //HTTP GET
                    var responseTask = _client.GetAsync("api/TaskManagers/search/" + SearchString.ToString());
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {

                        var readTask = await result.Content.ReadAsStringAsync();

                        Tasks = JsonConvert.DeserializeObject<IList<TaskManager>>(readTask);
                        return View(Tasks);
                    }
                    else //web api sent error response 
                    {

                        //log response status here..

                        Errors error = await globalErrorsAsync(result);

                        if (error.statuscode > 0)
                        {
                            return RedirectToAction("500", "Error", error);

                        }

                        return View(Tasks);
                    }
                
            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.Message }");
                    return RedirectToAction("500", "Error");
                }
                else

                    return RedirectToAction("500", "Error", ex.Message);

            }
        }
        /// <summary>
        /// Global Errors
        /// </summary>
        /// <param name="result"></param>
        /// <returns><Errors></returns>
        private async Task<Errors> globalErrorsAsync(HttpResponseMessage result)
        {
            Errors errors = new Errors();

            errors.statuscode = (int)result.StatusCode;
            errors.message= result.StatusCode.ToString();
            
            string content = await result.Content.ReadAsStringAsync();
            
            if (!string.IsNullOrEmpty(content))
            {
                var readTask = await result.Content.ReadAsStringAsync();
                string[] sError = readTask.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (!(sError == null))
                    {
                    if (sError.Length >= 2)
                    {
                        errors.message = sError[1].ToString();

                        if (TempData == null)
                        {

                            errors.message = sError[0].ToString();
                            return errors;
                        }
                        else

                        if (sError[0].ToString().Contains("Exists"))
                        {

                        }
                        else
                        {
                            TempData["Message"] = ($"Server error.{sError[0].ToString()}  Please contact administrator.");
                        }

                        string[] lineError = sError[0].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                        if (!(lineError == null))
                        {
                            if (lineError.Length >= 2)
                            {
                                errors.message = lineError[1].ToString();
                            }
                        }

                    }
                }

            }
            else
            {
                if (TempData == null)
                {
                    return errors;
                }
                else

                    TempData["Message"] = ($"The Request Returned and Error Status Code : {errors.statuscode} : Reason : {errors.message}.");
            }

            return errors;


        }

        /// <summary>
        /// Get tasks and subtasks by Parent ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: TaskScheduler/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
        {

            try
            {
                TaskManager Tasks = new TaskManager();
                TaskManager SubTasks = new TaskManager();
                TaskManager qryTasks = new TaskManager();
                bool Parent = false;

                var postTask = _client.GetAsync("api/TaskManagers/" + id.ToString());
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<TaskManager>();
                    readTask.Wait();
                    qryTasks = readTask.Result;
                    int myID = 0;

                    if (qryTasks.TaskType == TaskStatusType.Task)
                    {
                        myID = qryTasks.Id;
                        Tasks = readTask.Result;
                        Parent = true;
                    }
                    else
                    {
                        myID = qryTasks.ParentId;
                        SubTasks = readTask.Result;
                        Parent = false;
                    }



                    postTask = _client.GetAsync("api/TaskManagers/" + myID.ToString());
                    postTask.Wait();

                    result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        readTask = result.Content.ReadAsAsync<TaskManager>();
                        readTask.Wait();

                        qryTasks = readTask.Result;
                    }
                    else
                    {

                        Errors error = await globalErrorsAsync(result);

                        if (error.statuscode > 0)
                        {
                            return RedirectToAction("500", "Error", error);

                        }

                      
                        return View(Tasks);
                    }

                    if (qryTasks.TaskType == TaskStatusType.Task)
                    {
                        myID = qryTasks.Id;
                        Tasks = readTask.Result;

                    }
                    else
                    {
                        myID = qryTasks.ParentId;
                        SubTasks = readTask.Result;

                    }

                    var postSubTask = _client.GetAsync("api/SubTaskManager/" + myID.ToString());
                    postSubTask.Wait();

                    var Subresult = postSubTask.Result;
                    if (Subresult.IsSuccessStatusCode)
                    {
                        var readSubTask = Subresult.Content.ReadAsAsync<IEnumerable<TaskManager>>();
                        readSubTask.Wait();

                        var taskmanagerVM = new TaskManagerViewModel();

                        if (Parent)
                        {
                            taskmanagerVM.ParentTask = Tasks;
                            taskmanagerVM.SubTask = SubTasks;
                        }
                        else
                            taskmanagerVM.ParentTask = SubTasks;
                        taskmanagerVM.SubTask = Tasks;


                        taskmanagerVM.SubTasks = readSubTask.Result.ToList(); 
                        taskmanagerVM.Parent = Parent;


                        //return RedirectToAction("Index"); //for now
                        return View(taskmanagerVM);
                    }
                    else
                    {
                        //log response status here..

                        Errors error = await globalErrorsAsync(result);

                        if (error.statuscode > 0)
                        {
                            return RedirectToAction("500", "Error", error);

                        }

                        return View(Tasks);
                    }

                }
                else
                {
                    //log response status here..

                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode == 500)
                    {
                        return RedirectToAction("500", "Error", error);

                    }
                    if (error.statuscode == 404)
                    {
                        return RedirectToAction("404", "Error", error);

                    }

                    return View(Tasks);
                }

            }

            catch (Exception ex)
            {
                Errors error = new Errors() {

                    message = ex.Message
                     

                };

                

                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    
                }

                return RedirectToAction("500", "Error", error);
            }
        }



        // GET: TaskScheduler/CreateSubTask
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateSubTask(int id, string taskname)
        {


            return View();
        }

        /// <summary>
        /// Create SubTask
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: TaskScheduler/CreateSubTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubTaskAsync(IFormCollection collection)
        {
            try
            {

                TaskManager Task = new TaskManager();

                Task.ParentId = Convert.ToInt32(collection["Id"]);
                Task.Description = collection["Description"];

                Enum.TryParse(collection["TaskState"], out TaskState myState);

                TaskStatusType myStatus = TaskStatusType.SubTask;

                Task.TaskState = myState;
                Task.TaskType = myStatus;

                DateTime tDate;
                DateTime.TryParse(collection["StartDate"], out tDate);

                Task.StartDate = tDate;

                DateTime.TryParse(collection["FinishDate"], out tDate);

                Task.FinishDate = tDate;
                Task.TaskName = collection["TaskName"];

                if (String.IsNullOrEmpty(Task.TaskName))
                {
                    ModelState.AddModelError(nameof(Task.TaskName), "The Name is Required");
                    return View(Task);
                }
                if (String.IsNullOrEmpty(Task.Description))
                {
                    ModelState.AddModelError(nameof(Task.Description), "The Description is Required");
                    return View(Task);
                }


                //HTTP POST
                var postTask = _client.PostAsJsonAsync<TaskManager>("api/SubTaskManager/", Task);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    //log response status here..

                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }


                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else

                return RedirectToAction("500", "Error",ex);

            }
        }

        // GET: TaskScheduler/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create Parent Task
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: TaskScheduler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(IFormCollection collection)
        {
            try
            {

                TaskManager Task = new TaskManager();

                Task.Description = collection["Description"];

                Enum.TryParse(collection["TaskState"], out TaskState myState);
                Enum.TryParse(collection["TaskType"], out TaskStatusType myStatus);

                Task.TaskState = myState;
                Task.TaskType = myStatus;

                DateTime tDate;
                DateTime.TryParse(collection["StartDate"], out tDate);

                Task.StartDate = tDate;

                DateTime.TryParse(collection["FinishDate"], out tDate);

                Task.FinishDate = tDate;
                Task.TaskName = collection["TaskName"];

                if (String.IsNullOrEmpty(Task.TaskName))
                {
                    ModelState.AddModelError(nameof(Task.TaskName), "The Name is Required");
                    return View(Task);
                }
                if (String.IsNullOrEmpty(Task.Description))
                {
                    ModelState.AddModelError(nameof(Task.Description), "The Description is Required");
                    return View(Task);
                }


                //HTTP POST
                var postTask = _client.PostAsJsonAsync<TaskManager>("api/TaskManagers/", Task);
                postTask.Wait();

                var result = postTask.Result;
               if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<TaskManager>();
                    readTask.Wait();

                    var Tasks = readTask.Result;

                    if (!(TempData == null))
                    {
                        TempData["Message"] = ($"You have Created the Task : {Tasks.Description}  Successfully");

                    }
                    else
                    {
                        return View(Tasks);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    //log response status here..

                    if(result.StatusCode==HttpStatusCode.UnprocessableEntity)
                    {
                        string content = await result.Content.ReadAsStringAsync();
                        ModelState.AddModelError(nameof(Task.TaskName), content);
                        return View(Task);
                    }

                    var error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                       if (error.message.Contains("Exists"))
                        {
                            ModelState.AddModelError(nameof(Task.TaskName), error.message);
                            return View(Task);
                        }
                        
                        return RedirectToAction("500", "Error", error);

                    }

           
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.Message }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else
                
                return RedirectToAction("500", "Error", ex);

            }
        }

        /// <summary>
        /// Get Task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: TaskScheduler/Edit/5
        public async Task<ActionResult> EditAsync(int id)
        {

            try
            {

                TaskManager Tasks = new TaskManager();

                var postTask = _client.GetAsync("api/TaskManagers/" + id.ToString());
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<TaskManager>();
                    readTask.Wait();

                    Tasks = readTask.Result;

                    var postSubTask = _client.GetAsync("api/SubTaskManager/" + Tasks.ParentId.ToString());
                    postSubTask.Wait();

                    var Subresult = postSubTask.Result;
                    if (Subresult.IsSuccessStatusCode)
                    {
                        var readSubTask = Subresult.Content.ReadAsAsync<IEnumerable<TaskManager>>();
                        readSubTask.Wait();

                        var taskmanagerVM = new TaskManagerViewModel()
                        {
                            SubTasks = readSubTask.Result.ToList(),
                            ParentTask = Tasks
                        };

                        if (taskmanagerVM.ParentTask.TaskType == TaskStatusType.Task)
                        {
                            if (taskmanagerVM.SubTasks.Count > 0)
                            {
                                TempData["SubTaskCount"] = taskmanagerVM.SubTasks.Count();

                            }
                        }

                        return View(Tasks);
                    }
                    else
                    {
                        //log response status here..

                        Errors error = await globalErrorsAsync(result);

                        if (error.statuscode > 0)
                        {
                            return RedirectToAction("500", "Error", error);

                        }


                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    //log response status here..
                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }

                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else

                    return RedirectToAction("500", "Error", ex);

            }
        }
        /// <summary>
        /// Edit Task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: TaskScheduler/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, IFormCollection collection)
        {
            try
            {

                TaskManager Task = new TaskManager();

                Task.Id = Convert.ToInt32(collection["Id"]);
                Task.ParentId = Convert.ToInt32(collection["ParentId"]);

                Task.Description = collection["Description"];

                Enum.TryParse(collection["TaskState"], out TaskState myState);
                Enum.TryParse(collection["TaskType"], out TaskStatusType myStatus);

                Task.TaskState = myState;
                Task.TaskType = myStatus;

                DateTime tDate;
                DateTime.TryParse(collection["StartDate"], out tDate);

                Task.StartDate = tDate;

                DateTime.TryParse(collection["FinishDate"], out tDate);

                Task.FinishDate = tDate;
                Task.TaskName = collection["TaskName"];

                if (String.IsNullOrEmpty(Task.TaskName))
                {
                    ModelState.AddModelError(nameof(Task.TaskName), "The Name is Required");
                    return View(Task);
                }

                TaskManager Tasks = new TaskManager();

                //HTTP POST
                var postTask = _client.PutAsJsonAsync<TaskManager>("api/TaskManagers/" + id.ToString(), Task);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var postSubTask = _client.GetAsync("api/SubTaskManager/" + Task.ParentId.ToString());
                    postSubTask.Wait();

                    var Subresult = postSubTask.Result;
                    if (Subresult.IsSuccessStatusCode)
                    {
                        var readSubTask = Subresult.Content.ReadAsAsync<IEnumerable<TaskManager>>();
                        readSubTask.Wait();

                        var taskmanagerVM = new TaskManagerViewModel()
                        {
                            SubTasks = readSubTask.Result.ToList(),
                            ParentTask = Task
                        };

                        TempData["Message"] = "You have saved the Task Successfully";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //log response status here..

                        Errors error = await globalErrorsAsync(result);

                        if (error.statuscode > 0)
                        {
                            return RedirectToAction("500", "Error", error);

                        }

                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    //log response status here..
                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }

                    return RedirectToAction("Index");
                }

            }

            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else

                    return RedirectToAction("500", "Error", ex);

            }
        }
        /// <summary>
        /// Get the TASK to Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: TaskScheduler/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                TaskManager Tasks = new TaskManager();


                var postTask = _client.GetAsync("api/TaskManagers/" + id.ToString());
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<TaskManager>();
                    readTask.Wait();

                    Tasks = readTask.Result;

                    return View(Tasks);
                }
                else
                {
                    //log response status here..

                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }

                    return View(Tasks);
                }
            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else

                    return RedirectToAction("500", "Error", ex);

            }
        }

        /// <summary>
        /// Delete the Task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: TaskScheduler/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int id, IFormCollection collection)
        {
            try
            {

                TaskManager Task = new TaskManager();

                Task.Id = Convert.ToInt32(collection["Id"]);

                Task.Description = collection["Description"];

                Enum.TryParse(collection["TaskState"], out TaskState myState);
                Enum.TryParse(collection["TaskType"], out TaskStatusType myStatus);

                Task.TaskState = myState;
                Task.TaskType = myStatus;

                DateTime tDate;
                DateTime.TryParse(collection["StartDate"], out tDate);

                Task.StartDate = tDate;

                DateTime.TryParse(collection["FinishDate"], out tDate);

                Task.FinishDate = tDate;
                Task.TaskName = collection["TaskName"];

                //HTTP POST
                var postTask = _client.DeleteAsync("api/TaskManagers/" + id.ToString());
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    //return RedirectToAction(nameof(Index));

                    TempData["Message"] = "You have Deleted the Task Successfully";

                    return RedirectToAction("Index");


                }
                else
                {
                    //log response status here..
                    var readTask = await result.Content.ReadAsStringAsync();
                    string[] sError = readTask.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    TempData["Message"] = ($"Server error.{sError[0].ToString()}  Please contact administrator.");


                    return RedirectToAction("Index");

                }

            }
            catch (Exception ex)
            {
                if (!(TempData == null))
                {
                    TempData["Message"] = ($"Error { ex.GetBaseException() }");
                    return RedirectToAction("500", "Error", ex.Message);
                }
                else

                    return RedirectToAction("500", "Error", ex);

            }
        }
    }
}
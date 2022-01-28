using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TMSWebMVC.Models;
using TMSWebMVC.Service;
using Assert = NUnit.Framework.Assert;
using TMSWebMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TMSWebMVC.Class;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NUnitTestProject1
{
    [TestClass]
    public class TestClient
    {
        private readonly ClientService _client;
        public HttpResponseMessage response;

        public TestClient()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<ClientService>(c =>
            {

            });

            var serviceProvider = services.BuildServiceProvider();

            _client = serviceProvider.GetService<ClientService>();
            _client.BaseAddress = new Uri("https://localhost:44389/");
        }


        [SetUp]
        public void Setup()
        {




        }

       

        [Test]
        public void GetResponseIsSuccess()
        {
            IEnumerable<TaskManager> Tasks = null;

            //HTTP GET
            var responseTask = _client.GetAsync("api/CSVReport");
            responseTask.Wait();

            response = responseTask.Result;

            if (response.IsSuccessStatusCode)
            {

                var readTask = response.Content.ReadAsStringAsync().Result;

                Tasks = JsonConvert.DeserializeObject<IList<TaskManager>>(readTask);

            }


            //  Setup();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


        }

        [TestClass]
        public class HomeControllerTest
        {
            private readonly ClientService _client;

            public HttpResponseMessage response;

            public HomeControllerTest()
            {
                var services = new ServiceCollection();
                services.AddHttpClient<ClientService>(c =>
                {
                    c.BaseAddress = new Uri("https://localhost:44389/");
                });

                var serviceProvider = services.BuildServiceProvider();

                _client = serviceProvider.GetService<ClientService>();
                
            }


            [Test]
            public void Index()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                Task<IActionResult> result = controller.Index() as Task<IActionResult>;
                // Assert
                Assert.IsNotNull(result);
            }


            [Test]
            public void GetTaskDetailsbySearchArgs()
            {
                string search = "Bank";

                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                Task<IActionResult> result = controller.FilterIndex(search) as Task<IActionResult>;
                // Assert

                IActionResult myActResult = result.Result;

                var model = ModelFromIActionResult<List<TaskManager>>(myActResult);

                //Assert.IsTrue((model.errors.message ?? "").Contains("Exists"));

                Assert.IsTrue((model[0].TaskName ?? "").Contains("Bank"));
             }

            [Test]
            public void GetTaskDetails()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                int taskID = 2;
                Task<ActionResult> result = controller.DetailsAsync(taskID) as Task<ActionResult>;
                // Assert

                ActionResult myActResult = result.Result;

                var model = ModelFromActionResult<TaskManagerViewModel>(myActResult);


                Assert.AreEqual(taskID, model.ParentTask.Id  );
            }

            [Test]
            public void DeleteParentTask()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                int taskID = 1;
                Task<ActionResult> result = controller.DeleteAsync(taskID) as Task<ActionResult>;
                // Assert

                ActionResult myActResult = result.Result;

                var model = ModelFromActionResult<TaskManager>(myActResult);

                var formCol = new FormCollection(new Dictionary<string, StringValues>
                {
                    { "Description", model.Description },
                    { "StartDate", model.StartDate.ToString()},
                    { "FinishDate", model.FinishDate.ToString() },
                    { "TaskState", model.TaskState.ToString() },
                    { "TaskType", model.TaskType.ToString() },
                    { "TaskName", model.TaskName },
                    { "Id", model.Id.ToString() },
                    { "ParentId", model.ParentId.ToString() }

                });

                IFormCollection iform;

                iform = (IFormCollection)formCol;

                result = controller.DeleteAsync(model.Id,iform);
                // Assert
                myActResult = result.Result;

                Assert.AreEqual(taskID, model.ParentId);
            }

            [Test]
            public void GetTaskDetailsNotFound()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                int taskID = 2222;
                Task<ActionResult> result = controller.DetailsAsync(taskID) as Task<ActionResult>;
                // Assert

                ActionResult myActResult = result.Result;
                
                var model = ModelFromActionResult<TaskManagerViewModel>(myActResult);
                Assert.AreEqual(model.errors.message, "NotFound");



            }
            public T ModelFromIActionResult<T>(IActionResult actionResult)
            {
                object model = null;
                if (actionResult.GetType() == typeof(ViewResult))
                {
                    ViewResult viewResult = (ViewResult)actionResult;
                    TaskManagerViewModel TMmodel = new TaskManagerViewModel();
                    Errors errors = new Errors();



                    if (viewResult.ViewData.ModelState.IsValid == false)
                    {

                        foreach (ModelStateEntry modelState in viewResult.ViewData.ModelState.Values)
                        {
                            foreach (ModelError error in modelState.Errors)
                            {
                                errors.message = error.ErrorMessage;
                                TMmodel.errors = errors;
                                model = TMmodel;
                            }
                        }

                    }
                    else
                    {

                        model = viewResult.Model;

                    }


                }
                else if (actionResult.GetType() == typeof(PartialViewResult))
                {
                    PartialViewResult partialViewResult = (PartialViewResult)actionResult;
                    model = partialViewResult.Model;
                }
                else if (actionResult.GetType() == typeof(RedirectToActionResult))
                {
                    RedirectToActionResult partialViewResult = (RedirectToActionResult)actionResult;
                    TaskManagerViewModel TMmodel = new TaskManagerViewModel();
                    Errors errors = new Errors();


                    if (partialViewResult.RouteValues == null)
                    {
                        if (partialViewResult.Fragment == null)
                        {

                        }
                        else errors.message = (string)partialViewResult.Fragment;

                    }
                    else
                    {
                        if (partialViewResult.RouteValues != null)
                        {
                            if (partialViewResult.RouteValues.ContainsKey("message"))
                            {
                                errors.message = (string)partialViewResult.RouteValues["message"];
                            }

                            if (partialViewResult.RouteValues.ContainsKey("statuscode"))
                            {
                                errors.statuscode = (int)partialViewResult.RouteValues["statuscode"];
                            }
                        }
                    }

                    TMmodel.errors = errors;
                    model = TMmodel;
                    //errors.message = partialViewResult.RouteValues[1];
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Actionresult of type {0} is not supported by ModelFromResult extractor.", actionResult.GetType()));
                }
                T typedModel = (T)model;

                return typedModel;
            }
            public T ModelFromActionResult<T>(ActionResult actionResult)
            {
                object model=null;
                if (actionResult.GetType() == typeof(ViewResult))
                {
                    ViewResult viewResult = (ViewResult)actionResult;
                    TaskManagerViewModel TMmodel = new TaskManagerViewModel();
                    Errors errors = new Errors();

                    

                    if (viewResult.ViewData.ModelState.IsValid==false)
                    {
                        
                        foreach (ModelStateEntry modelState in viewResult.ViewData.ModelState.Values)
                        {
                            foreach (ModelError error in modelState.Errors)
                            {
                                errors.message = error.ErrorMessage;
                                TMmodel.errors = errors;
                                model = TMmodel;
                            }
                        }

                    }
                    else
                    {

                        model = viewResult.Model;

                    }

                   
                }
                else if (actionResult.GetType() == typeof(PartialViewResult))
                {
                    PartialViewResult partialViewResult = (PartialViewResult)actionResult;
                    model = partialViewResult.Model;
                }
                else if (actionResult.GetType() == typeof(RedirectToActionResult))
                {
                    RedirectToActionResult partialViewResult = (RedirectToActionResult)actionResult;
                    TaskManagerViewModel TMmodel = new TaskManagerViewModel();
                    Errors errors = new Errors();

                    if (partialViewResult.RouteValues.ContainsKey("message"))
                    {
                        errors.message = (string)partialViewResult.RouteValues["message"];
                    }

                    if (partialViewResult.RouteValues.ContainsKey("statuscode"))
                    {
                        errors.statuscode = (int)partialViewResult.RouteValues["statuscode"];
                    }

                    
                    TMmodel.errors = errors;
                    model = TMmodel;
                    //errors.message = partialViewResult.RouteValues[1];
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Actionresult of type {0} is not supported by ModelFromResult extractor.", actionResult.GetType()));
                }
                T typedModel = (T)model;
                
                return typedModel;
            }
            [Test]
            public void ADDTask()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act
                
                var formCol = new FormCollection(new Dictionary<string, StringValues>
                {
                    { "Description", "Get Requirements" },
                    { "StartDate", "01/01/2020" },
                    { "FinishDate", "02/05/2024" },
                    { "TaskState", "Planned" },
                    { "TaskType", "Task" },
                    { "TaskName", "JAMMER" }

                });


                IFormCollection iform;

                iform = (IFormCollection)formCol;

                Task<ActionResult> result = controller.CreateAsync(iform);
                // Assert
                ActionResult myActResult = result.Result;

                if (myActResult.GetType() == typeof(ViewResult))
                {
                    ViewResult viewResult = (ViewResult)myActResult;

                    if (viewResult.ViewData.ModelState.IsValid == false)
                    {
                        var model = ModelFromActionResult<TaskManagerViewModel>(myActResult);
                       
                        Assert.IsTrue((model.errors.message ?? "").Contains("Exists"));

                    }
                    else
                    {
                        var model = ModelFromActionResult<TaskManager>(myActResult);

                        Assert.IsTrue((model.TaskName ?? "").Contains("JAMMER"));

                    }

                }
                

            }
            [Test]
            public void ADDExistingTask()
            {
                // Arrange
                TaskSchedulerController controller = new TaskSchedulerController(_client);
                // Act

                var formCol = new FormCollection(new Dictionary<string, StringValues>
                {
                    { "Description", "Get Requirements" },
                    { "StartDate", "01/01/2019" },
                    { "FinishDate", "02/05/2020" },
                    { "TaskState", "Planned" },
                    { "TaskType", "Task" },
                    { "TaskName", "GM" }

                });


                IFormCollection iform;

                iform = (IFormCollection)formCol;

                Task<ActionResult> result = controller.CreateAsync(iform);
                // Assert
                ActionResult myActResult = result.Result;

                if (myActResult.GetType() == typeof(ViewResult))
                {
                    ViewResult viewResult = (ViewResult)myActResult;

                    if (viewResult.ViewData.ModelState.IsValid == false)
                    {
                        var model = ModelFromActionResult<TaskManagerViewModel>(myActResult);

                        Assert.IsTrue((model.errors.message ?? "").Contains("Exists"));

                    }
                    else
                    {
                        var model = ModelFromActionResult<TaskManager>(myActResult);

                        Assert.IsTrue((model.TaskName ?? "").Contains("JAMMER"));

                    }

                }


                //var model = ModelFromActionResult<TaskManagerViewModel>(myActResult);

                //Assert.IsTrue((model.errors.message ?? "").Contains("Exists"));

                //Assert.IsTrue((model.errors.message ?? "").Contains("Timeout"));


            }

        }

    }
}
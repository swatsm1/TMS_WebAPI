using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMSWebMVC.Class;
using TMSWebMVC.Models;
using TMSWebMVC.Service;

namespace TMSWebMVC.Controllers
{
    public class CSVReportController : Controller
    {
        private readonly ClientService _client;
        /// <summary>
        /// DI Inject Client service
        /// </summary>
        /// <param name="client"></param>
        public CSVReportController(ClientService client)
        {
            _client = client;
            _client.BaseAddress = client._clientUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>View()</returns>
        // GET: CSVReport
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: CSVReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: CSVReport/Create
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Filter result by Date
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        // POST: CSVReport/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(IFormCollection collection)
        {
            try
            {
                ReportDates reportDates = new ReportDates();
                IEnumerable<TaskManager> Tasks = null;

                DateTime tDate;
                DateTime.TryParse(collection["StartDate"], out tDate);

                reportDates.FDate = tDate;

                DateTime.TryParse(collection["FinishDate"], out tDate);

                reportDates.TDate = tDate;
                //HTTP GET
                var responseTask = _client.GetAsync("api/CSVReport");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = await result.Content.ReadAsStringAsync();

                    Tasks = JsonConvert.DeserializeObject<IList<TaskManager>>(readTask);
                    StringBuilder sb = new StringBuilder();

                    //Filter out by Dates
                    List<TaskManager> queryresult = Tasks.Where(s => s.StartDate >= reportDates.FDate
                       && s.FinishDate <= reportDates.TDate).ToList();

                    foreach (TaskManager Tms in queryresult)
                    {
                        sb.AppendFormat(
                            "{0};{1};{2};{3};{4};{5};{6};{7}",
                            Tms.Id,
                            Tms.ParentId,
                            Tms.Description,
                            Tms.TaskName,
                            Tms.StartDate,
                            Tms.FinishDate,
                            Tms.TaskState,
                            Tms.TaskType,
                            Environment.NewLine);
                    }


                    CSVReportViewModel report = new CSVReportViewModel();

                    report.CSVResult = sb.ToString();

                    TempData["Report"] = report.CSVResult;

                    return View();

                }
                else //web api sent error response 
                {
                    Errors error = await globalErrorsAsync(result);

                    if (error.statuscode > 0)
                    {
                        return RedirectToAction("500", "Error", error);

                    }

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ($"Error { ex.GetBaseException() }");
                return RedirectToAction("500", "Error");

            }
        }

        private async Task<Errors> globalErrorsAsync(HttpResponseMessage result)
        {
            Errors errors = new Errors();

            errors.statuscode = (int)result.StatusCode;
            errors.message = result.StatusCode.ToString();

            string content = await result.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                var readTask = await result.Content.ReadAsStringAsync();
                string[] sError = readTask.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                TempData["Message"] = ($"Server error.{sError[0].ToString()}  Please contact administrator.");
            }
            else
            {
                TempData["Message"] = ($"The Request Returned and Error Status Code : {errors.statuscode} : Reason : {errors.message}.");
            }

            return errors;


        }

        // GET: CSVReport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CSVReport/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CSVReport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CSVReport/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
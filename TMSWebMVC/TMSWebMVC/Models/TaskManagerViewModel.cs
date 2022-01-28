using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMSWebMVC.Class;
namespace TMSWebMVC.Models
{
    public class TaskManagerViewModel
    {
        public List<TaskManager> SubTasks;

        public TaskManager ParentTask;

        public TaskManager SubTask;

        public bool Parent;

        public Errors errors;

    }
}

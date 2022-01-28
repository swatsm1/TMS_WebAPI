using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_WebAPI.Models
{
    public class TaskExistsException : Exception
    {
        public TaskExistsException(string message)
      : base(message)
        {

        }

    }
}

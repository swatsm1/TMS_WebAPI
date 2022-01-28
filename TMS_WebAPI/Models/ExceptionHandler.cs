using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS_WebAPI.Models
{
    public static class ExceptionHandler
    {
        internal static Exception appException { get; set; }
        internal static string exceptionMessage { get; set; }

       
        public static void ExceptionHandlerMethod()
        {

        }

        public static void ClearException()
        {

            appException=null;
            exceptionMessage = string.Empty;

        }

        public static Exception GetException()
        {

            return appException;

        }
        public static string GetExceptionMessage()
        {

            return exceptionMessage;

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS_WebAPI.Models;

namespace TMS_WebAPI.Models
{
    public interface ILog
    {
        Task info(string str, TMS_WebAPIContext _context);
    }

    class appLogger : ILog, IDisposable
    {

        public appLogger()
        {
            
        }

        public async Task info(string str, TMS_WebAPIContext _context)

        {
            try
            {
                Logger mylog = new Logger();

                mylog.logDatTime = DateTime.Now;
                mylog.logText = str;

                _context.Logger.Add(mylog);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
     
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MyConsoleLogger()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}

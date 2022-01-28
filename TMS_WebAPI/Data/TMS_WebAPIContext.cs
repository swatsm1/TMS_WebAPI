using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TMS_WebAPI.Models
{
    public class TMS_WebAPIContext : DbContext
    {
        public TMS_WebAPIContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public TMS_WebAPIContext (DbContextOptions<TMS_WebAPIContext> options)
            : base(options)
        {
        }

        public DbSet<TaskManager> TaskManager { get; set; }
        public DbSet<Logger> Logger { get; set; }
    }
}

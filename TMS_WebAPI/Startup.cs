using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TMS_WebAPI.Models;

using TMS_WebAPI.Services;

namespace TMS_WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddDbContext<TMS_WebAPIContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("TMS_WebAPIContext")));

            services.AddDbContext<TMS_WebAPIContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;DataBase=TMSWebAPI;MultipleActiveResultSets=True;Trusted_Connection=True;"));

            services.AddScoped(typeof(IDal), typeof(Dal));

            //services.Add(new ServiceDescriptor(typeof(ILog), new appLogger()));

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

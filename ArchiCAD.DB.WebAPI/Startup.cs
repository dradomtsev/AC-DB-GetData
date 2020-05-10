using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiCAD.DB.WebAPI.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace ArchiCAD.DB.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }
        ServicesExt.ArchiCADWebApiService ACService;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            ACService = new ServicesExt.ArchiCADWebApiService(Configuration);
            //services.AddSingleton<ServicesExt.IArchiCADWebApiService>((ServicesExt.ArchiCADWebApiService)Configuration);

            //services.AddScoped<Controllers.IArchiCADController, Controllers.ArchiCADController>();
            //services.AddHttpClient<Controllers.IArchiCADController, Controllers.ArchiCADController>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            ACService.Dispose();
        }
    }
}

using HDFCMSILWebMVC.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC
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
            services.AddControllersWithViews();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();
            services.Configure<FormOptions>(x =>
            {
                x.BufferBody = false;
                x.KeyLengthLimit = 2048; // 2 KiB
                x.MultipartHeadersCountLimit = 32; // 16
                x.MultipartHeadersLengthLimit = 32768; // 16384
                x.MultipartBoundaryLengthLimit = 256; // 128
                x.ValueCountLimit = int.MaxValue; // Max number of form values
                x.ValueLengthLimit = int.MaxValue; // Max length of each form value
                x.MultipartBodyLengthLimit = long.MaxValue; // Max length of multipart body
            });
            services.AddSession(optons=>
                { optons.IdleTimeout = TimeSpan.FromMinutes(180);
                }
            );

            services.Configure<IISServerOptions>(options =>

            {
                options.MaxRequestBodySize = long.MaxValue;
            }); // Register the ExcelService
            services.AddScoped<IExcelService, ExcelService>();
            //services.AddScoped<AccessMenu, PageAccess>();
            services.AddTransient<DataService>(provider => new DataService(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSignalR();

        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
        .SuppressStatusMessages(true) //disable the status messages
        .UseStartup<Startup>();


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
          
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=LoginPage}/{id?}");
                endpoints.MapHub<UploadProgressHub>("/uploadProgressHub");
            });

          //  loggerFactory..AddFile($@"{Directory.GetCurrentDirectory()}\Log\log.txt");

        }
    }
}

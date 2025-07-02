using _001TN0172;
using HDFCMSILWebMVC.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using System.Security.Cryptography;
using System.Text;
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
                { optons.IdleTimeout = TimeSpan.FromMinutes(30);
                }
            );

            services.Configure<IISServerOptions>(options =>

            {
                options.MaxRequestBodySize = long.MaxValue;
            }); // Register the ExcelService
            services.AddScoped<IExcelService, ExcelService>();
            //services.AddScoped<AccessMenu, PageAccess>();
            string[] splitedpass = Configuration.GetConnectionString("DefaultConnection").ToString().Split(";");
            string exactpass = splitedpass[3].Remove(0, 9);
            var DecryptedPassword = Decrypted(exactpass);
            DecryptedPassword = Configuration.GetConnectionString("DefaultConnection").ToString().Replace(exactpass, DecryptedPassword);
            services.AddTransient<DataService>(provider => new DataService(DecryptedPassword));
            services.AddSignalR();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
            });

        }
        protected string Decrypted(string input)
        {
            try
            {
                string EncryptionKey = "MySuperSecureKeyHDFC_MSIL@123456"; // 32 chars = 256-bit
                byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);

                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                    throw new Exception("Key must be 16, 24, or 32 bytes.");

                byte[] cipherTextBytes = Convert.FromBase64String(input);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = new byte[16]; // Must match the IV used in encryption
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (MemoryStream ms = new MemoryStream(cipherTextBytes))
                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //"Encrypted Input value is not in proper value. Please enter proper encrypted input."
                Console.WriteLine("Decryption error: " + ex.Message);
                return null;
            }
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseMiddleware<SessionValidationMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=LoginPage}/{id?}");
                endpoints.MapHub<UploadProgressHub>("/uploadProgressHub");
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=LoginUAM}/{action=UAMLoginNew}/{id?}");
                //endpoints.MapHub<UploadProgressHub>("/uploadProgressHub");
            });

            //  loggerFactory..AddFile($@"{Directory.GetCurrentDirectory()}\Log\log.txt");


            //CREATE SYMMETRIC KEY MSIL_HDFC
            //WITH ALGORITHM = AES_256
            //ENCRYPTION BY PASSWORD = 'Hdfc@123456789';

        }
    }
}

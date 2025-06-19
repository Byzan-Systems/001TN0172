using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCrontab;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace HDFCMSILWebMVC.Controllers
{
    public class ServerController : Controller
    {
        private readonly ILogger _logger;
        public ServerController(ILogger<ServerController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomePage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
            
        }

        public IActionResult RunApplication()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    // run every 5 minutes
        //    var schedule = CrontabSchedule.Parse("*/5 * * * *");
        //    var nextRun = schedule.GetNextOccurrence(DateTime.Now);
        //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    do
        //    {
        //        if (DateTime.Now > nextRun)
        //        {
        //            _logger.LogInformation("Sending notifications at: {time}", DateTimeOffset.Now);
        //            await DoSomethingAsync();
        //            nextRun = schedule.GetNextOccurrence(DateTime.Now);
        //        }
        //        await Task.Delay(1000, stoppingToken);
        //    } while (!stoppingToken.IsCancellationRequested);
        //}

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace HDFCMSILWebMVC.Controllers
{
    public class EODProcessController : Controller
    {
        private readonly ILogger _logger;
        public EODProcessController(ILogger<EODProcessController> logger)
        {
            _logger = logger;
        }
        public IActionResult ShowEODProcess()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
            
        }
        public IActionResult Update()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        db.Database.ExecuteSqlRaw("update EOD_Reports set EODFlag=1");
                        db.SaveChanges();
                        var message = "EOD Reports Successfully updated.";
                        TempData["alertMessage"] = message;
                    }

                    _logger.LogInformation("Executed successfully" + " - EODProcessController;Update");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - EODProcessController;Update");
                }

                return RedirectToAction("ShowEODProcess", "EODProcess");
            }
        }


    }
}

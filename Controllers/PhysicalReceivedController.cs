using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace _001TN0172.Controllers
{
    public class PhysicalReceivedController : Controller
    {
        private readonly ILogger _logger;
        public PhysicalReceivedController(ILogger<PhysicalReceivedController> logger)
        {
            _logger = logger;
        }

        public IActionResult ShowPhysicalReceived()
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
                        db.Database.ExecuteSqlRaw("update Phy_Reports set PhyFlag=1");
                        db.SaveChanges();
                        var message = "Physical Invoice Successfully updated.";
                        TempData["alertMessage"] = message;
                    }

                    _logger.LogInformation("Executed successfully" + " - PhysicalReceivedController;Update");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - PhysicalReceivedController;Update");
                }
                return RedirectToAction("ShowPhysicalReceived", "PhysicalReceived");
            }
        }
    }
}

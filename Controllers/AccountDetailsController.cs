using HDFCMSILWebMVC.Models;
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
    public class AccountDetailsController : Controller
    {
    
        private readonly ILogger _logger;
        public AccountDetailsController(ILogger<AccountDetailsController> logger)
        {
            _logger = logger;
        }
        public ActionResult ShowAccountDetails()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                var AccountDetailss = new AccountDetails();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        var ss = db.Set<DBAccountDetails>().FromSqlRaw("Select * from Account_Details").ToList();

                        AccountDetailss.Payment_Type = ss[0].Payment_Type;
                        AccountDetailss.CR_Account_No = ss[0].CR_Account_No;
                        AccountDetailss.DR_Account_No = ss[0].DR_Account_No;

                        _logger.LogInformation("Executed successfully" + " - AccountDetailsController; ShowAccountDetails");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AccountDetailsController;ShowAccountDetails");
                }

                return View(AccountDetailss);
            }
        }

        [HttpPost]
        public ActionResult ShowAccountDetails(AccountDetails model)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                var employeeModel = new AccountDetails();

                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {

                        var ss = db.Set<DBAccountDetails>().FromSqlRaw("Select * from Account_Details where Payment_Type ='" + model.Payment_Type + "' ").ToList();
                        if (ss.Count > 0)
                        {
                            employeeModel.Payment_Type = ss[0].Payment_Type;
                            employeeModel.CR_Account_No = ss[0].CR_Account_No;
                            employeeModel.DR_Account_No = ss[0].DR_Account_No;
                        }
                        ModelState.Clear();

                        _logger.LogInformation("Executed successfully" + " - AccountDetailsController; ShowAccountDetails");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AccountDetailsController;ShowAccountDetails");
                }

                return View(employeeModel);

            }
        }

        [HttpPost]
        public ActionResult UpdateAccountDetails (AccountDetails model)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                var employeeModel = new DBAccountDetails();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        db.Database.ExecuteSqlRaw("Update Account_Details set CR_Account_No='" + model.CR_Account_No + "',DR_Account_No='" + model.DR_Account_No + "' where Payment_Type ='" + model.Payment_Type + "' ");
                        //  var ss = db.Set<DBAccountDetails>().FromSqlRaw().ToList();

                        TempData["alertMessage"] = "Account number update successfullly.";

                        _logger.LogInformation("Executed successfully" + " - AccountDetailsController; UpdateAccountDetails");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AccountDetailsController;UpdateAccountDetails");
                }
                return RedirectToAction("ShowAccountDetails");
            }
        }
    }
}

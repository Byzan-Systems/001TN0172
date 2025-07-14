using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Data;

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
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                var AccountDetailss = new AccountDetails();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        
                        AccountDetailss.Payment_Type = "Select Payment Type";
                        AccountDetailss.CR_Account_No = "";
                        AccountDetailss.DR_Account_No = "";


                        //var ss = db.Set<DBAccountDetails>().FromSqlRaw("Select * from Account_Details").ToList();

                        //string DT_CR = Methods.EncryptDecryptData("Decrypt", "", ss[0].CR_Account_No, _logger);
                        //string DT_DR = Methods.EncryptDecryptData("Decrypt", "", ss[0].DR_Account_No, _logger);
                        //AccountDetailss.Payment_Type = ss[0].Payment_Type;
                        //AccountDetailss.CR_Account_No = DT_CR;
                        //AccountDetailss.DR_Account_No = DT_DR;

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
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                var employeeModel = new AccountDetails();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        var ss = db.Set<DBAccountDetails>().FromSqlInterpolated($"Select * from Account_Details where Payment_Type = {model.Payment_Type}").ToList();
                        //var ss = db.Set<DBAccountDetails>().FromSqlRaw("Select * from Account_Details where Payment_Type ='" + model.Payment_Type + "' ").ToList();
                        _logger.LogInformation("data read from database." + " - AccountDetailsController; ShowAccountDetails");
                        if (ss.Count > 0)
                        {
                            string DT_CR = Methods.EncryptDecryptData("Decrypt", "", ss[0].CR_Account_No, _logger);
                            string DT_DR = Methods.EncryptDecryptData("Decrypt", "", ss[0].DR_Account_No, _logger);
                            employeeModel.Payment_Type = ss[0].Payment_Type;
                            if (DT_CR == null || DT_CR == "")
                                employeeModel.CR_Account_No = ss[0].CR_Account_No;
                            else
                                employeeModel.CR_Account_No = DT_CR;

                            if (DT_DR == null || DT_DR == "")
                                employeeModel.DR_Account_No = ss[0].DR_Account_No;
                            else
                                employeeModel.DR_Account_No = DT_DR;

                            _logger.LogInformation("Account details are present to show." + " - AccountDetailsController; ShowAccountDetails");
                        }
                        else
                        {
                            employeeModel.Payment_Type = model.Payment_Type;
                            employeeModel.CR_Account_No = "";
                            employeeModel.DR_Account_No = "";

                            _logger.LogInformation("No data to show" + " - AccountDetailsController; ShowAccountDetails");
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
        public ActionResult UpdateAccountDetails(AccountDetails model)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                var employeeModel = new DBAccountDetails();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        string encryptedBase64 = Methods.EncryptDecryptData("Encrypt", model.CR_Account_No, "", _logger);
                        string encryptedBase64DR = Methods.EncryptDecryptData("Encrypt", model.DR_Account_No, "", _logger);
                        db.Database.ExecuteSqlInterpolated($@"UPDATE Account_Details SET CR_Account_No = {encryptedBase64}, DR_Account_No = {encryptedBase64DR}  WHERE Payment_Type = {model.Payment_Type}");

                        //db.Database.ExecuteSqlRaw("Update Account_Details set CR_Account_No='" + encryptedBase64 + "',DR_Account_No='" + encryptedBase64DR + "' where Payment_Type ='" + model.Payment_Type + "' ");
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

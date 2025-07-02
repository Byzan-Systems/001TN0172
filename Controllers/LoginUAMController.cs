using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HDFCMSILWebMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using HDFCMSILWebMVC.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Novell.Directory.Ldap;
using System.Data;

using System.Security.Claims;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;

namespace HDFCMSILWebMVC.Controllers
{
    public class LoginUAMController : Controller
    {
        private readonly ILogger _logger;
        private IWebHostEnvironment Environment;
        //private readonly AccessMenu Accmenu;
        public LoginUAMController(ILogger<LoginUAMController> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Environment = _environment;
        }

        [HttpPost]
        public IActionResult UAMLoginPage(user_mst_temp LoginViewModel)
        {
            //try
            //{
            //    using (var db = new Entities.DatabaseContext())
            //    {
            //        //var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Password == LoginViewModel.Password && a.Login_Enable == 1).FirstOrDefault();
            //        var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Login_Enable == 1).FirstOrDefault();

            //        if (rec != null)
            //        {
            //            HttpContext.Session.SetString("UserName", LoginViewModel.LoginName);
            //            HttpContext.Session.SetString("LoginID", rec.LoginID.ToString());
            //            UserSession.LoginID = rec.LoginID.ToString();
            //            var builder = new ConfigurationBuilder()
            //          .SetBasePath(Directory.GetCurrentDirectory() + "\\")
            //          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //            IConfigurationRoot configuration = builder.Build();
            //            string contentPath = Environment.ContentRootPath + "\\";
            //            HttpContext.Session.SetString("InputFilePath", contentPath + configuration.GetSection("MSILSettings:InputFilePath").Value);
            //            HttpContext.Session.SetString("OutputFilePath", contentPath + configuration.GetSection("MSILSettings:OutputFilePath").Value);
            //            HttpContext.Session.SetString("BackupFilePath", contentPath + configuration.GetSection("MSILSettings:BackupFilePath").Value);
            //            HttpContext.Session.SetString("NonConvertedFile", contentPath + configuration.GetSection("MSILSettings:NonConvertedFile").Value);
            //            HttpContext.Session.SetString("ErrorLog", contentPath + configuration.GetSection("MSILSettings:ErrorLog").Value);
            //            HttpContext.Session.SetString("AuditLog", contentPath + configuration.GetSection("MSILSettings:AuditLog").Value);
            //            HttpContext.Session.SetString("TRADE_EMAIL", configuration.GetSection("MSILSettings:TRADE_EMAIL").Value);
            //            HttpContext.Session.SetString("Frequency", configuration.GetSection("MSILSettings:Frequency").Value);
            //            HttpContext.Session.SetString("INV_CONF_EMAIL", configuration.GetSection("MSILSettings:INV_CONF_EMAIL").Value);
            //            HttpContext.Session.SetString("INV_CONF_FNAME", configuration.GetSection("MSILSettings:INV_CONF_FNAME").Value);
            //            HttpContext.Session.SetString("ORD_MIS_EMAIL", configuration.GetSection("MSILSettings:ORD_MIS_EMAIL").Value);
            //            HttpContext.Session.SetString("PAYREC_TRADE_EMAIL", configuration.GetSection("MSILSettings:PAYREC_TRADE_EMAIL").Value);
            //            HttpContext.Session.SetString("PHY_INV", configuration.GetSection("MSILSettings:PHY_INV").Value);
            //            HttpContext.Session.SetString("NO_INV", configuration.GetSection("MSILSettings:NO_INV").Value);
            //            HttpContext.Session.SetString("EOD_MIS", configuration.GetSection("MSILSettings:EOD_MIS").Value);
            //            HttpContext.Session.SetString("ORD_DEL", configuration.GetSection("MSILSettings:ORD_DEL").Value);
            //            HttpContext.Session.SetString("INV_PHY_NTRC", configuration.GetSection("MSILSettings:INV_PHY_NTRC").Value);
            //            HttpContext.Session.SetString("IntraDayPath", configuration.GetSection("MSILSettings:IntraDayPath").Value);
            //            HttpContext.Session.SetString("EOD_File_EMail", configuration.GetSection("MSILSettings:EOD_File_EMail").Value);
            //            HttpContext.Session.SetString("FCC_EMail", configuration.GetSection("MSILSettings:FCC_EMail").Value);
            //            HttpContext.Session.SetString("DRC_EMail", configuration.GetSection("MSILSettings:DRC_EMail").Value);
            //            HttpContext.Session.SetString("Payment_Rejection_EMail", configuration.GetSection("MSILSettings:Payment_Rejection_EMail").Value);
            //            HttpContext.Session.SetString("DRC_EMail_BNGR", configuration.GetSection("MSILSettings:DRC_EMail_BNGR").Value);
            //            HttpContext.Session.SetString("DRC_EMail_SLGR", configuration.GetSection("MSILSettings:DRC_EMail_SLGR").Value);
            //            HttpContext.Session.SetString("DO_Cancel_Email", configuration.GetSection("MSILSettings:DO_Cancel_Email").Value);
            //            HttpContext.Session.SetString("DO_Invoice_Cancel_Email", configuration.GetSection("MSILSettings:DO_Invoice_Cancel_Email").Value);
            //            HttpContext.Session.SetString("Invoice_Cancel_Email", configuration.GetSection("MSILSettings:Invoice_Cancel_Email").Value);
            //            HttpContext.Session.SetString("Sleep_Time_in_Mint", configuration.GetSection("MSILSettings:Sleep_Time_in_Mint").Value);
            //            HttpContext.Session.SetString("Confirmation_Mail", configuration.GetSection("EmailSetting:Confirmation_Mail").Value);
            //            HttpContext.Session.SetString("SMTP_HOST", configuration.GetSection("EmailSetting:SMTP_HOST").Value);
            //            HttpContext.Session.SetString("Port", configuration.GetSection("EmailSetting:Port").Value);
            //            HttpContext.Session.SetString("Email_FromID", configuration.GetSection("EmailSetting:Email_FromID").Value);
            //            HttpContext.Session.SetString("UserID", configuration.GetSection("EmailSetting:UserID").Value);
            //            HttpContext.Session.SetString("Password", configuration.GetSection("EmailSetting:Password").Value);
            //            HttpContext.Session.SetString("SysEmail_FromID", configuration.GetSection("SystemSetting:Pwd").Value);
            //            HttpContext.Session.SetString("PWD", configuration.GetSection("SystemSetting:Pwd").Value);
            //            _logger.LogInformation("The MSIL application login : User Name - " + LoginViewModel.LoginName);


            //            //// comment validateLDAP if on UAT, otherwise check LDAP
            //            if (ValidateLDAP(LoginViewModel.LoginName, LoginViewModel.Password) == true)
            //            {
            //                _logger.LogInformation("Ldap Successfull" + LoginViewModel.LoginName);

            //                if (rec.LoginType == "SERVER")
            //                {
            //                    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition

            //                    return RedirectToAction("HomePage", "Server");
            //                }
            //                else if (rec.LoginType == "TRADE OPS")
            //                {
            //                    return RedirectToAction("Trade_OPSHomePage", "Login");
            //                }
            //                else if (rec.LoginType == "CASH OPS")
            //                {
            //                    return RedirectToAction("Cash_OPSHomePage", "Login");
            //                }
            //                else if (rec.LoginType == "CASH_Trade OPS")
            //                {
            //                    return RedirectToAction("Cash_TradeHomePage", "Login");
            //                }
            //                else
            //                {

            //                    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition
            //                    return RedirectToAction("HomePage", "Login");
            //                }
            //            }
            //            else
            //            {
            //                _logger.LogInformation("Ldap Fail" + "User" + LoginViewModel.LoginName + "Password" + LoginViewModel.Password + LoginViewModel.LoginName);
            //            }

            //        }
            //        else
            //        {
            //            ViewBag.LoginStatus = 0;
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.ToString() + " - LoginController;LoginPage");
            //}
            return View(LoginViewModel);
        }

        public IActionResult UAMLoginNew()
        {
            return View("UAMPage");
        }
        public IActionResult HomePage(string token)
        {


            //if (HttpContext.Session.GetString("LoginID") == null)
            //{ return RedirectToAction("LoginPage", "Login"); }
            //else
            //{
            //try
            //{
            //    var key = Encoding.UTF8.GetBytes("MSILApploginfail");
            //    var tokenHandler = new JwtSecurityTokenHandler();

            //    var validationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };

            //    tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            //    var jwtToken = (JwtSecurityToken)validatedToken;
            //    //var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            //    var username = jwtToken.Claims.ElementAt(0).ToString().Split(":");
            //    var Password = jwtToken.Claims.ElementAt(1).ToString().Split(":");
            //    using (var db = new Entities.DatabaseContext())
            //    {
            //        var strlogId = db.Set<UAM_LoginLogoutExist>().FromSqlRaw("select MSIL_LogoutDatetime, LogID from UAM_LoginLogout where User_id='" + username[1].ToString().Trim() + "' order by CONVERT(int, logID) desc ").ToList();

            //        if (strlogId[0].LogID == "" || strlogId[0].LogID == null)
            //        {
            //            TempData["alertMessageuam"] = "User " + username[1].ToString().Trim() +" already logined.";
            //            return View("LoginPageFail");
            //        }
                    
            //    }


            //        // Set username in session
            //        HttpContext.Session.SetString("LoginID", username[1].ToString());

            //    HttpContext.Session.SetString("UserName", username[1].ToString());
            //    HttpContext.Session.SetString("LoginID", username[1].ToString());
            //    UserSession.LoginID = HttpContext.Session.GetString("LoginID").ToString();
            //    var builder = new ConfigurationBuilder()
            //  .SetBasePath(Directory.GetCurrentDirectory() + "\\")
            //  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //    IConfigurationRoot configuration = builder.Build();
            //    string contentPath = Environment.ContentRootPath + "\\";
            //    HttpContext.Session.SetString("InputFilePath", contentPath + configuration.GetSection("MSILSettings:InputFilePath").Value);
            //    HttpContext.Session.SetString("OutputFilePath", contentPath + configuration.GetSection("MSILSettings:OutputFilePath").Value);
            //    HttpContext.Session.SetString("BackupFilePath", contentPath + configuration.GetSection("MSILSettings:BackupFilePath").Value);
            //    HttpContext.Session.SetString("NonConvertedFile", contentPath + configuration.GetSection("MSILSettings:NonConvertedFile").Value);
            //    HttpContext.Session.SetString("ErrorLog", contentPath + configuration.GetSection("MSILSettings:ErrorLog").Value);
            //    HttpContext.Session.SetString("AuditLog", contentPath + configuration.GetSection("MSILSettings:AuditLog").Value);
            //    HttpContext.Session.SetString("TRADE_EMAIL", configuration.GetSection("MSILSettings:TRADE_EMAIL").Value);
            //    HttpContext.Session.SetString("Frequency", configuration.GetSection("MSILSettings:Frequency").Value);
            //    HttpContext.Session.SetString("INV_CONF_EMAIL", configuration.GetSection("MSILSettings:INV_CONF_EMAIL").Value);
            //    HttpContext.Session.SetString("INV_CONF_FNAME", configuration.GetSection("MSILSettings:INV_CONF_FNAME").Value);
            //    HttpContext.Session.SetString("ORD_MIS_EMAIL", configuration.GetSection("MSILSettings:ORD_MIS_EMAIL").Value);
            //    HttpContext.Session.SetString("PAYREC_TRADE_EMAIL", configuration.GetSection("MSILSettings:PAYREC_TRADE_EMAIL").Value);
            //    HttpContext.Session.SetString("PHY_INV", configuration.GetSection("MSILSettings:PHY_INV").Value);
            //    HttpContext.Session.SetString("NO_INV", configuration.GetSection("MSILSettings:NO_INV").Value);
            //    HttpContext.Session.SetString("EOD_MIS", configuration.GetSection("MSILSettings:EOD_MIS").Value);
            //    HttpContext.Session.SetString("ORD_DEL", configuration.GetSection("MSILSettings:ORD_DEL").Value);
            //    HttpContext.Session.SetString("INV_PHY_NTRC", configuration.GetSection("MSILSettings:INV_PHY_NTRC").Value);
            //    HttpContext.Session.SetString("IntraDayPath", configuration.GetSection("MSILSettings:IntraDayPath").Value);
            //    HttpContext.Session.SetString("EOD_File_EMail", configuration.GetSection("MSILSettings:EOD_File_EMail").Value);
            //    HttpContext.Session.SetString("FCC_EMail", configuration.GetSection("MSILSettings:FCC_EMail").Value);
            //    HttpContext.Session.SetString("DRC_EMail", configuration.GetSection("MSILSettings:DRC_EMail").Value);
            //    HttpContext.Session.SetString("Payment_Rejection_EMail", configuration.GetSection("MSILSettings:Payment_Rejection_EMail").Value);
            //    HttpContext.Session.SetString("DRC_EMail_BNGR", configuration.GetSection("MSILSettings:DRC_EMail_BNGR").Value);
            //    HttpContext.Session.SetString("DRC_EMail_SLGR", configuration.GetSection("MSILSettings:DRC_EMail_SLGR").Value);
            //    HttpContext.Session.SetString("DO_Cancel_Email", configuration.GetSection("MSILSettings:DO_Cancel_Email").Value);
            //    HttpContext.Session.SetString("DO_Invoice_Cancel_Email", configuration.GetSection("MSILSettings:DO_Invoice_Cancel_Email").Value);
            //    HttpContext.Session.SetString("Invoice_Cancel_Email", configuration.GetSection("MSILSettings:Invoice_Cancel_Email").Value);
            //    HttpContext.Session.SetString("Sleep_Time_in_Mint", configuration.GetSection("MSILSettings:Sleep_Time_in_Mint").Value);
            //    HttpContext.Session.SetString("Confirmation_Mail", configuration.GetSection("EmailSetting:Confirmation_Mail").Value);
            //    HttpContext.Session.SetString("SMTP_HOST", configuration.GetSection("EmailSetting:SMTP_HOST").Value);
            //    HttpContext.Session.SetString("Port", configuration.GetSection("EmailSetting:Port").Value);
            //    HttpContext.Session.SetString("Email_FromID", configuration.GetSection("EmailSetting:Email_FromID").Value);
            //    HttpContext.Session.SetString("UserID", configuration.GetSection("EmailSetting:UserID").Value);
            //    HttpContext.Session.SetString("Password", configuration.GetSection("EmailSetting:Password").Value);
            //    HttpContext.Session.SetString("SysEmail_FromID", configuration.GetSection("SystemSetting:Pwd").Value);
            //    HttpContext.Session.SetString("PWD", configuration.GetSection("SystemSetting:Pwd").Value);
            //    HttpContext.Session.SetString("UAMPath", configuration.GetSection("MSILSettings:UAMPath").Value);
                
            //    _logger.LogInformation("The MSIL application login : User Name - " + username[1].ToString());
                return View();
            //    //return RedirectToAction("Index", "Home"); // Redirect to a desired page
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex.ToString() + " - LoginController;Save");
            //    TempData["alertMessageuam"] = "An error occurred while trying to access the provided URL. Please verify the link or provide more details for assistance.";
            //    return View("LoginPageFail");
            //}

            //}

        }     
      
        public IActionResult DashboardPage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                return View();
            }
        }

      

    }
}

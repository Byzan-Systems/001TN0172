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
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using _001TN0172;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HDFCMSILWebMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger _logger;
        private IWebHostEnvironment Environment;
        public string sessionToken = "";
        //private readonly AccessMenu Accmenu;
        public LoginController(ILogger<LoginController> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            Environment = _environment;

        }

        public void LoginLogDB(string pExpDescp, string pError_Id, string Type, string FrmUserID, string SessionID, string IsActive)
        {
            try
            {
                if (Type == "LOGIN")
                {
                    string D1 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    //string strId;
                    // string cmd;
                    using (var db = new Entities.DatabaseContext())
                    {
                        //var strlogId = db.Set<MSIL_LoginLogoutID>().FromSqlRaw("select Convert(varchar,max(Convert(int, logId)))LogID from MSIL_LoginLogout").ToList();

                        //if (strlogId[0].LogID == "" || strlogId[0].LogID == null)
                        //{
                        //    strId = 0.ToString();
                        //}
                        //else
                        //{
                        //    strId = strlogId[0].LogID;
                        //}
                        //strId = (int.Parse(strId) + 1).ToString();
                        DataTable DTFile = Methods.InsertDetailsAudit("Insert_MSIL_LoginLogout",FrmUserID.ToString(),D1,SessionID ,IsActive, "", "", "", _logger);


                      //  db.Database.ExecuteSqlRaw("insert into MSIL_LoginLogout (logId, User_id, LoginDate_Time,SessionID,IsActive) values('" + strId + "','" + FrmUserID.ToString() + "','" + D1 + "','" + SessionID + "','" + IsActive + "' ) ");
                    }
                }
                if (Type == "LOGOUT")
                {
                    string D1 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");
                    string strId;
                    // string cmd;
                    using (var db = new Entities.DatabaseContext())
                    {
                        DataTable dt = Methods.getDetailsAudit("Get_LoginDB", FrmUserID.ToString().Trim(), "", "", "", "", "", "", _logger);
                        if (dt.Rows.Count == 0)
                        {
                            strId = 0.ToString();
                        }
                        else
                        {
                            strId = dt.Rows[0][0].ToString();
                        }
                        //var strlogId = db.Set<MSIL_LoginLogoutID>().FromSqlRaw("select (Convert(varchar, max(Convert(int, LogID)))) LogID from MSIL_LoginLogout where User_id='" + FrmUserID.ToString().Trim() + "'").ToList();

                        //if (strlogId[0].LogID == "")
                        //{
                        //    strId = 0.ToString();
                        //}
                        //else
                        //{
                        //    strId = strlogId[0].LogID;
                        //}
                        DataTable dt_Up = Methods.UpdateDetailsAudit("Update_Logout", D1, FrmUserID.ToString().Trim(), strId, "", "", "","",_logger);

                }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - LoginController;LoginLogDB");
            }
            finally
            {
            }
        }

        [HttpPost]
        public IActionResult UpdateLogoutTime()
        {
            using (var db = new Entities.DatabaseContext())
            {
                var sessionToken = HttpContext.Session.GetString("SessionToken");
                
                    var validTokenlist = db.Set<MSIL_LoginLogout>().FromSqlRaw("select MSIL_LogoutDatetime,logID,SessionID,IsActive,IPAddress from MSIL_LoginLogout where User_Id='" + UserSession.LoginID + "' order by CONVERT(int, logID) desc").ToList();
                    var validToken = validTokenlist[0].SessionID;
                    if (sessionToken != validToken)
                    {

                    return RedirectToAction("HomePage", "Login");

                }
                
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult LoginPage(user_mst_temp LoginViewModel)
        {
            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    //var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Password == LoginViewModel.Password && a.Login_Enable == 1).FirstOrDefault();
                    var rec = db.user_mst_tempDB.Where(a => a.User_Id == LoginViewModel.User_Name).FirstOrDefault();

                    if (rec != null)
                    {
                        var recIsactive = db.user_mst_tempDB.Where(a => a.User_Id == LoginViewModel.User_Name && a.IsActive == "1").FirstOrDefault();
                        if (recIsactive != null)
                        {
                            sessionToken = Guid.NewGuid().ToString();
                            HttpContext.Session.SetString("SessionToken", sessionToken);
                            HttpContext.Session.SetString("UserName", LoginViewModel.User_Name);
                            HttpContext.Session.SetString("LoginID", rec.User_Id.ToString());
                            UserSession.LoginID = rec.User_Id.ToString();
                            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory() + "\\")
                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                            IConfigurationRoot configuration = builder.Build();
                            string contentPath = Environment.ContentRootPath + "\\";
                            HttpContext.Session.SetString("InputFilePath", contentPath + configuration.GetSection("MSILSettings:InputFilePath").Value);
                            HttpContext.Session.SetString("OutputFilePath", contentPath + configuration.GetSection("MSILSettings:OutputFilePath").Value);
                            HttpContext.Session.SetString("BackupFilePath", contentPath + configuration.GetSection("MSILSettings:BackupFilePath").Value);
                            HttpContext.Session.SetString("NonConvertedFile", contentPath + configuration.GetSection("MSILSettings:NonConvertedFile").Value);
                            HttpContext.Session.SetString("ErrorLog", contentPath + configuration.GetSection("MSILSettings:ErrorLog").Value);
                            HttpContext.Session.SetString("AuditLog", contentPath + configuration.GetSection("MSILSettings:AuditLog").Value);
                            HttpContext.Session.SetString("TRADE_EMAIL", configuration.GetSection("MSILSettings:TRADE_EMAIL").Value);
                            HttpContext.Session.SetString("Frequency", configuration.GetSection("MSILSettings:Frequency").Value);
                            HttpContext.Session.SetString("INV_CONF_EMAIL", configuration.GetSection("MSILSettings:INV_CONF_EMAIL").Value);
                            HttpContext.Session.SetString("INV_CONF_FNAME", configuration.GetSection("MSILSettings:INV_CONF_FNAME").Value);
                            HttpContext.Session.SetString("ORD_MIS_EMAIL", configuration.GetSection("MSILSettings:ORD_MIS_EMAIL").Value);
                            HttpContext.Session.SetString("PAYREC_TRADE_EMAIL", configuration.GetSection("MSILSettings:PAYREC_TRADE_EMAIL").Value);
                            HttpContext.Session.SetString("PHY_INV", configuration.GetSection("MSILSettings:PHY_INV").Value);
                            HttpContext.Session.SetString("NO_INV", configuration.GetSection("MSILSettings:NO_INV").Value);
                            HttpContext.Session.SetString("EOD_MIS", configuration.GetSection("MSILSettings:EOD_MIS").Value);
                            HttpContext.Session.SetString("ORD_DEL", configuration.GetSection("MSILSettings:ORD_DEL").Value);
                            HttpContext.Session.SetString("INV_PHY_NTRC", configuration.GetSection("MSILSettings:INV_PHY_NTRC").Value);
                            HttpContext.Session.SetString("IntraDayPath", configuration.GetSection("MSILSettings:IntraDayPath").Value);
                            HttpContext.Session.SetString("EOD_File_EMail", configuration.GetSection("MSILSettings:EOD_File_EMail").Value);
                            HttpContext.Session.SetString("FCC_EMail", configuration.GetSection("MSILSettings:FCC_EMail").Value);
                            HttpContext.Session.SetString("DRC_EMail", configuration.GetSection("MSILSettings:DRC_EMail").Value);
                            HttpContext.Session.SetString("Payment_Rejection_EMail", configuration.GetSection("MSILSettings:Payment_Rejection_EMail").Value);
                            HttpContext.Session.SetString("DRC_EMail_BNGR", configuration.GetSection("MSILSettings:DRC_EMail_BNGR").Value);
                            HttpContext.Session.SetString("DRC_EMail_SLGR", configuration.GetSection("MSILSettings:DRC_EMail_SLGR").Value);
                            HttpContext.Session.SetString("DO_Cancel_Email", configuration.GetSection("MSILSettings:DO_Cancel_Email").Value);
                            HttpContext.Session.SetString("DO_Invoice_Cancel_Email", configuration.GetSection("MSILSettings:DO_Invoice_Cancel_Email").Value);
                            HttpContext.Session.SetString("Invoice_Cancel_Email", configuration.GetSection("MSILSettings:Invoice_Cancel_Email").Value);
                            HttpContext.Session.SetString("Sleep_Time_in_Mint", configuration.GetSection("MSILSettings:Sleep_Time_in_Mint").Value);
                            HttpContext.Session.SetString("Confirmation_Mail", configuration.GetSection("EmailSetting:Confirmation_Mail").Value);
                            HttpContext.Session.SetString("SMTP_HOST", configuration.GetSection("EmailSetting:SMTP_HOST").Value);
                            HttpContext.Session.SetString("Port", configuration.GetSection("EmailSetting:Port").Value);
                            HttpContext.Session.SetString("Email_FromID", configuration.GetSection("EmailSetting:Email_FromID").Value);
                            HttpContext.Session.SetString("UserID", configuration.GetSection("EmailSetting:UserID").Value);
                            HttpContext.Session.SetString("Password", configuration.GetSection("EmailSetting:Password").Value);
                            HttpContext.Session.SetString("SysEmail_FromID", configuration.GetSection("SystemSetting:Pwd").Value);
                            HttpContext.Session.SetString("PWD", configuration.GetSection("SystemSetting:Pwd").Value);
                            _logger.LogInformation("The MSIL application login: User Name - {UserName}", LoginViewModel.User_Name);


                            //// comment validateLDAP if on UAT, otherwise check LDAP
                            ////////if (ValidateLDAP(LoginViewModel.User_Name, LoginViewModel.Password) == true)
                            ////////{

                            // Dormancy Check\
                            int isdormant = DaysCheck(rec);
                            _logger.LogInformation("LDAP login successful for user {UserName}", MaskUsername(LoginViewModel.User_Name));

                            _logger.LogInformation("LDAP login successful for user {UserName}", LoginViewModel.User_Name);
                            if (isdormant == 0)
                            {

                                //if (rec.LoginType == "SERVER")
                                //{
                                //    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition

                                //    return RedirectToAction("HomePage", "Server");
                                //}
                                //else if (rec.LoginType == "TRADE OPS")
                                //{
                                //    return RedirectToAction("Trade_OPSHomePage", "Login");
                                //}
                                //else if (rec.LoginType == "CASH OPS")
                                //{
                                //    return RedirectToAction("Cash_OPSHomePage", "Login");
                                //}
                                //else if (rec.LoginType == "CASH_Trade OPS")
                                //{
                                //    return RedirectToAction("Cash_TradeHomePage", "Login");
                                //}
                                //else
                                //{

                                var strlogId = db.Set<MSIL_LoginLogout>().FromSqlRaw("select MSIL_LogoutDatetime,logID,SessionID,IsActive,IPAddress from MSIL_LoginLogout where User_Id='" + recIsactive.User_Name.ToString() + "' order by CONVERT(int, logID) desc").ToList();
                                string curdatestr = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

                                if (strlogId[0].MSIL_LogoutDatetime is null || string.IsNullOrWhiteSpace(strlogId[0].MSIL_LogoutDatetime.ToString()))
                                {
                                    TempData["alertMessageDormant"] = $"User {recIsactive.User_Name} is already logged in.";
                                    ViewBag.ShowConfirmLogout = true;
                                    ViewBag.LoginStatus = -1;

                                }
                                else
                                {

                                    db.Database.ExecuteSqlRaw("update USER_Mst_Temp set LastLogin='" + curdatestr + "' ,[EmpType]='EXISTING' where User_Id='" + recIsactive.User_Name + "'");
                                    db.Database.ExecuteSqlRaw("update UserHistory set LastLogin='" + curdatestr + "' where User_Id='" + recIsactive.User_Name + "'");

                                    LoginLogDB("", "", "LOGIN", UserSession.LoginID.ToString(), sessionToken, "1");
                                    // 2. Create user claims
                                    var claims = new List<Claim>
                                        {
                                                new Claim(ClaimTypes.Name, UserSession.LoginID),
                                                new Claim(ClaimTypes.NameIdentifier, UserSession.LoginID),
                                                new Claim(ClaimTypes.Role, "Admin")
                                        };
                                   
                                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var principal = new ClaimsPrincipal(identity);
                                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                                    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition
                                    return RedirectToAction("HomePage", "Login");
                                    //}
                                }
                            }
                            else
                            { ViewBag.LoginStatus = -1; }
                            ////////}
                            ////////else
                            ////////{
                            ////////    _logger.LogInformation("Ldap Fail" + "User" + LoginViewModel.User_Name + "Password" + LoginViewModel.Password + LoginViewModel.User_Name);
                            ////////}
                        }

                        else
                        {
                            ViewBag.LoginStatus = -1;
                            TempData["alertMessageDormant"] = rec.Remark;
                        }



                        ////////if (rec.LoginType == "SERVER")
                        ////////{
                        ////////    return RedirectToAction("HomePage", "Server");
                        ////////}
                        ////////else
                        ////////{
                        ////////    return RedirectToAction("HomePage", "Login");
                        ////////}

                    }
                    else
                    {
                        ViewBag.LoginStatus = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - LoginController;LoginPage");
            }
            return View(LoginViewModel);
        }
        private string MaskUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return "UnknownUser";
            // Sanitize input by removing newline characters and other potentially harmful characters
            username = username.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            return username.Length <= 5 ? "**" : username.Substring(0, 5) + new string('*', username.Length - 5);
        }

        public int DaysCheck(user_mst_temp user_Mst_TempS)
        {
            DataTable dtGetUserDetails = new DataTable();
            int ISDormant = 0;
            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    DataTable dtDays = new DataTable();
                    //var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Password == LoginViewModel.Password && a.Login_Enable == 1).FirstOrDefault();
                    var row = db.user_mst_tempDB.Where(a => a.LastLogin != "NULL" && a.EmpType != "NULL" && a.IsActive != "3" && a.IsActive == "1" && a.User_Id == UserSession.LoginID).FirstOrDefault();
                    // _logger.LogInformation("NO OF RECORDS: " + row.);
                    if (row != null)
                    {
                        int dayCount = 0;

                        if (!string.IsNullOrEmpty(row.LastLogin.ToString()))
                        {
                            string lastLoginStr = row.LastLogin.ToString();
                            DateTime parsedDate = DateTime.ParseExact(lastLoginStr, "yyyy-MM-dd hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);

                            //DateTime dbLastLoginDate = DateTime.ParseExact(parsedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime currentDate = DateTime.Now;

                            dayCount = (currentDate - parsedDate).Days;
                            _logger.LogInformation("check user - " + row.User_Id);
                            _logger.LogInformation("total day differences - " + dayCount);
                        }
                        _logger.LogInformation("[ " + dayCount + " ] Days Difference");


                        string empType = row.EmpType.ToString().ToUpper();
                        string isActive = row.IsActive.ToString();
                        string userId = row.User_Id.ToString();

                        if (dayCount > 30 && empType == "NEW" && isActive != "3")
                        {
                            _logger.LogInformation($"Before value of daycount and isactive - {dayCount} and {isActive}");
                            var query = "UPDATE USER_Mst_Temp SET IsActive='5', Remark='" + userId + " IS LOCKED DUE TO DORMANT]'  WHERE User_ID = '" + userId + "'  AND isactive = '1' AND isactive <> '3'";
                            db.Database.ExecuteSqlRaw(query);
                            db.SaveChanges();

                            _logger.LogInformation("Update query for New Dormant users - " + query + "check user - " + row.User_Id);
                            _logger.LogInformation($"[{userId}] IS LOCKED DUE TO DORMANT NEW USER");
                            TempData["alertMessageDormant"] = $"[{ userId}] IS LOCKED DUE TO DORMANT. Kindly contact Administrator";
                            ISDormant = 1;
                        }
                        else if (dayCount > 90 && empType == "EXISTING" && isActive != "3")
                        {
                            _logger.LogInformation($"Before value for existing user - daycount and isactive - {dayCount} and {isActive}");
                            var query = "UPDATE USER_Mst_Temp SET IsActive='5', Remark='" + userId + " IS LOCKED DUE TO DORMANT]'  WHERE User_ID = '" + userId + "'  AND isactive = '1' AND isactive <> '3'";
                            db.Database.ExecuteSqlRaw(query);
                            db.SaveChanges();

                            _logger.LogInformation("Update query for Dormant Existing users - " + query);
                            _logger.LogInformation($"[{userId}] IS LOCKED DUE TO DORMANT EXISTING USER");
                            ISDormant = 1;
                            TempData["alertMessageDormant"] = $"[{ userId}] IS LOCKED DUE TO DORMANT. Kindly contact Administrator";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ISDormant = -1;
                TempData["alertMessageDormant"] = ex.ToString();
                _logger.LogError(ex.ToString() + " - LoginController;DaysCheck");
            }
            return ISDormant;
        }

        //public IActionResult LoginPage(LoginMST LoginViewModel)
        //{
        //    //try
        //    //{
        //    //    using (var db = new Entities.DatabaseContext())
        //    //    {
        //    //        //var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Password == LoginViewModel.Password && a.Login_Enable == 1).FirstOrDefault();
        //    //        var rec = db.LoginMSTs.Where(a => a.LoginName == LoginViewModel.LoginName && a.Login_Enable == 1).FirstOrDefault();

        //    //        if (rec != null)
        //    //        {
        //    //            HttpContext.Session.SetString("UserName", LoginViewModel.LoginName);
        //    //            HttpContext.Session.SetString("LoginID", rec.LoginID.ToString());
        //    //            UserSession.LoginID = rec.LoginID.ToString();
        //    //            var builder = new ConfigurationBuilder()
        //    //          .SetBasePath(Directory.GetCurrentDirectory() + "\\")
        //    //          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        //    //            IConfigurationRoot configuration = builder.Build();
        //    //            string contentPath = Environment.ContentRootPath + "\\";
        //    //            HttpContext.Session.SetString("InputFilePath", contentPath + configuration.GetSection("MSILSettings:InputFilePath").Value);
        //    //            HttpContext.Session.SetString("OutputFilePath", contentPath + configuration.GetSection("MSILSettings:OutputFilePath").Value);
        //    //            HttpContext.Session.SetString("BackupFilePath", contentPath + configuration.GetSection("MSILSettings:BackupFilePath").Value);
        //    //            HttpContext.Session.SetString("NonConvertedFile", contentPath + configuration.GetSection("MSILSettings:NonConvertedFile").Value);
        //    //            HttpContext.Session.SetString("ErrorLog", contentPath + configuration.GetSection("MSILSettings:ErrorLog").Value);
        //    //            HttpContext.Session.SetString("AuditLog", contentPath + configuration.GetSection("MSILSettings:AuditLog").Value);
        //    //            HttpContext.Session.SetString("TRADE_EMAIL", configuration.GetSection("MSILSettings:TRADE_EMAIL").Value);
        //    //            HttpContext.Session.SetString("Frequency", configuration.GetSection("MSILSettings:Frequency").Value);
        //    //            HttpContext.Session.SetString("INV_CONF_EMAIL", configuration.GetSection("MSILSettings:INV_CONF_EMAIL").Value);
        //    //            HttpContext.Session.SetString("INV_CONF_FNAME", configuration.GetSection("MSILSettings:INV_CONF_FNAME").Value);
        //    //            HttpContext.Session.SetString("ORD_MIS_EMAIL", configuration.GetSection("MSILSettings:ORD_MIS_EMAIL").Value);
        //    //            HttpContext.Session.SetString("PAYREC_TRADE_EMAIL", configuration.GetSection("MSILSettings:PAYREC_TRADE_EMAIL").Value);
        //    //            HttpContext.Session.SetString("PHY_INV", configuration.GetSection("MSILSettings:PHY_INV").Value);
        //    //            HttpContext.Session.SetString("NO_INV", configuration.GetSection("MSILSettings:NO_INV").Value);
        //    //            HttpContext.Session.SetString("EOD_MIS", configuration.GetSection("MSILSettings:EOD_MIS").Value);
        //    //            HttpContext.Session.SetString("ORD_DEL", configuration.GetSection("MSILSettings:ORD_DEL").Value);
        //    //            HttpContext.Session.SetString("INV_PHY_NTRC", configuration.GetSection("MSILSettings:INV_PHY_NTRC").Value);
        //    //            HttpContext.Session.SetString("IntraDayPath", configuration.GetSection("MSILSettings:IntraDayPath").Value);
        //    //            HttpContext.Session.SetString("EOD_File_EMail", configuration.GetSection("MSILSettings:EOD_File_EMail").Value);
        //    //            HttpContext.Session.SetString("FCC_EMail", configuration.GetSection("MSILSettings:FCC_EMail").Value);
        //    //            HttpContext.Session.SetString("DRC_EMail", configuration.GetSection("MSILSettings:DRC_EMail").Value);
        //    //            HttpContext.Session.SetString("Payment_Rejection_EMail", configuration.GetSection("MSILSettings:Payment_Rejection_EMail").Value);
        //    //            HttpContext.Session.SetString("DRC_EMail_BNGR", configuration.GetSection("MSILSettings:DRC_EMail_BNGR").Value);
        //    //            HttpContext.Session.SetString("DRC_EMail_SLGR", configuration.GetSection("MSILSettings:DRC_EMail_SLGR").Value);
        //    //            HttpContext.Session.SetString("DO_Cancel_Email", configuration.GetSection("MSILSettings:DO_Cancel_Email").Value);
        //    //            HttpContext.Session.SetString("DO_Invoice_Cancel_Email", configuration.GetSection("MSILSettings:DO_Invoice_Cancel_Email").Value);
        //    //            HttpContext.Session.SetString("Invoice_Cancel_Email", configuration.GetSection("MSILSettings:Invoice_Cancel_Email").Value);
        //    //            HttpContext.Session.SetString("Sleep_Time_in_Mint", configuration.GetSection("MSILSettings:Sleep_Time_in_Mint").Value);
        //    //            HttpContext.Session.SetString("Confirmation_Mail", configuration.GetSection("EmailSetting:Confirmation_Mail").Value);
        //    //            HttpContext.Session.SetString("SMTP_HOST", configuration.GetSection("EmailSetting:SMTP_HOST").Value);
        //    //            HttpContext.Session.SetString("Port", configuration.GetSection("EmailSetting:Port").Value);
        //    //            HttpContext.Session.SetString("Email_FromID", configuration.GetSection("EmailSetting:Email_FromID").Value);
        //    //            HttpContext.Session.SetString("UserID", configuration.GetSection("EmailSetting:UserID").Value);
        //    //            HttpContext.Session.SetString("Password", configuration.GetSection("EmailSetting:Password").Value);
        //    //            HttpContext.Session.SetString("SysEmail_FromID", configuration.GetSection("SystemSetting:Pwd").Value);
        //    //            HttpContext.Session.SetString("PWD", configuration.GetSection("SystemSetting:Pwd").Value);
        //    //            _logger.LogInformation("The MSIL application login : User Name - " + LoginViewModel.LoginName);


        //    //            //// comment validateLDAP if on UAT, otherwise check LDAP
        //    //            if (ValidateLDAP(LoginViewModel.LoginName, LoginViewModel.Password) == true)
        //    //            {
        //    //                _logger.LogInformation("Ldap Successfull" + LoginViewModel.LoginName);

        //    //                if (rec.LoginType == "SERVER")
        //    //                {
        //    //                    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition

        //    //                    return RedirectToAction("HomePage", "Server");
        //    //                }
        //    //                else if (rec.LoginType == "TRADE OPS")
        //    //                {
        //    //                    return RedirectToAction("Trade_OPSHomePage", "Login");
        //    //                }
        //    //                else if (rec.LoginType == "CASH OPS")
        //    //                {
        //    //                    return RedirectToAction("Cash_OPSHomePage", "Login");
        //    //                }
        //    //                else if (rec.LoginType == "CASH_Trade OPS")
        //    //                {
        //    //                    return RedirectToAction("Cash_TradeHomePage", "Login");
        //    //                }
        //    //                else
        //    //                {

        //    //                    ViewBag.DownloadInvoice = User.IsInRole("False"); // or any other condition
        //    //                    return RedirectToAction("HomePage", "Login");
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                _logger.LogInformation("Ldap Fail" + "User" + LoginViewModel.LoginName + "Password" + LoginViewModel.Password + LoginViewModel.LoginName);
        //    //            }

        //    //        }
        //    //        else
        //    //        {
        //    //            ViewBag.LoginStatus = 0;
        //    //        }

        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    _logger.LogError(ex.ToString() + " - LoginController;LoginPage");
        //    //}
        //    return View(LoginViewModel);
        //}



        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                using (var db = new Entities.DatabaseContext())
                {

                    var inv1 = await db.Set<ShowUserMaster>().FromSqlRaw("Select LoginID, LoginName,LoginType,Login_Enable from Login").ToListAsync();

                    return View(inv1);
                }
            }
        }
        public async Task<IActionResult> Details(int? LoginID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (LoginID == null)
                {
                    return NotFound();
                }
                using (var db = new Entities.DatabaseContext())
                {
                    // var employee = await db.Set<DetailsUserMaster>().FromSqlRaw("Select LoginID, LoginName,LoginType,Login_Enable from Login Where LoginID=" + LoginID +"").ToListAsync();
                    var employee = await db.LoginMSTs.FirstOrDefaultAsync(m => m.LoginID == LoginID);
                    if (employee == null)
                    {
                        return NotFound();
                    }
                    return View(employee);
                }
            }
        }
        // GET: Employees/Delete/1
        public async Task<IActionResult> Delete(int? LoginID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (LoginID == null)
                {
                    return NotFound();
                }
                using (var db = new Entities.DatabaseContext())
                {
                    var employee = await db.LoginMSTs.FirstOrDefaultAsync(m => m.LoginID == LoginID);

                    if (employee == null)
                    {
                        return NotFound();
                    }

                    return View(employee);
                }
            }
        }

        // POST: Employees/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int LoginID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                using (var db = new Entities.DatabaseContext())
                {
                    var employee = await db.LoginMSTs.FindAsync(LoginID);
                    db.LoginMSTs.Remove(employee);
                    await db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
        }
        //AddOrEdit Get Method
        public async Task<IActionResult> AddOrEdit(int? LoginID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                ViewBag.PageName = LoginID == null ? "Create User" : "Edit User";
                ViewBag.IsEdit = LoginID == null ? false : true;
                if (LoginID == null)
                {
                    return View();
                }
                else
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        var employee = await db.LoginMSTs.FindAsync(LoginID);

                        if (employee == null)
                        {
                            return NotFound();
                        }
                        return View(employee);
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult Editupdate(UserMaster userMaster)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                int x = 0;
                if (userMaster.LoginType == "0" || userMaster.LoginType == "Select Role Type")
                {
                    TempData["alertMessage"] = "Please select the Role Type";
                    return RedirectToAction("Index", "Login");
                }
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        //var reg = db.Database.ExecuteSqlRaw("");
                        //db.SaveChanges();
                        //https://www.aspsnippets.com/questions/377667/How-to-validate-user-for-Signup-using-Stored-Procedure-in-SQL-Server/

                        using (var command = db.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "Customer_Signup";
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@LoginName", userMaster.LoginName));
                            command.Parameters.Add(new SqlParameter("@Password", string.Empty));
                            command.Parameters.Add(new SqlParameter("@ReTypePassword", string.Empty));
                            command.Parameters.Add(new SqlParameter("@LoginType", userMaster.LoginType));
                            command.Parameters.Add(new SqlParameter("@Flag", 2));
                            db.Database.OpenConnection();
                            using (var result = command.ExecuteReader())
                            {
                                if (result.HasRows)
                                {
                                    result.Read();
                                    x = result.GetInt32(0); // x = your sp count value
                                                            //return x;
                                }
                            }
                            db.Database.CloseConnection();
                        }
                    }
                    string message = string.Empty;
                    switch (x)
                    {
                        case -1:
                            message = "User Name has already been used \\nPlease choose a different User Name.";
                            break;
                        case -2:
                            message = "Passwords do not match.";
                            break;
                        default:
                            message = "User update successful.";
                            //SendActivationEmail(userId);
                            break;
                    }
                    TempData["alertMessage"] = message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - LoginController;Editupdate");
                }
                //return View(userMaster);
                return RedirectToAction("Index", "Login");
            }
        }
        public IActionResult LoginPage()
        {
            return View();
        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_RESTORE = 3;

        public IActionResult Logout()
        {
            try
            {
                //Process[] processes = Process.GetProcessesByName("MSIL_LOCAL 1.1");
                //if (HttpContext.Session.GetString("UAMPath") == "") { Console.WriteLine("No process found with the name: " + "MSIL_LOCAL 1.1.exe"); }
                //else
                //{
                //string UAMPath = HttpContext.Session.GetString("UAMPath");
                //string filename = Path.GetFileNameWithoutExtension(UAMPath);
                //Process[] processes = Process.GetProcessesByName(filename);
                LoginLogDB("", "", "LOGOUT", UserSession.LoginID.ToString(), sessionToken, "0");
                //if (processes.Length > 0)
                //{
                //    foreach (var process in processes)
                //    {
                //        IntPtr hWnd = FindWindow(null, process.MainWindowTitle);

                //        if (hWnd != IntPtr.Zero)
                //        {
                //            // Restore and bring the window to the front
                //            ShowWindow(hWnd, SW_RESTORE);
                //            SetForegroundWindow(hWnd);
                //            _logger.LogError("Window brought to the foreground." + " - LoginController;Logout");
                //        }
                //        else
                //        {
                //            _logger.LogError("Window handle not found." + " - LoginController;Logout");
                //        }

                //    }
                //}
                //}
                HttpContext.Session.Clear();
                //return View("LoginPage");
                return RedirectToAction("LoginPage", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - LoginController;Logout");
                TempData["alertMessageuam"] = "An error occurred while trying to access the provided URL. Please verify the link or provide more details for assistance.";
                return View("LoginPage");
            }
        }

        public IActionResult DashboardPage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            {
                Logout();
                return RedirectToAction("UAMLoginNew", "LoginUAM");
            }
            else
            {
                return View();
            }
        }
        public IActionResult HomePage()
        {

            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                //DataSet dt = Methods.getDetails_Web("Get_SubMenuAccessAsper", UserSession.LoginID, "", "", "", "", "", "", _logger);
                //List<AccessItem> lst = Accmenu.GetMenuItems(dt.Tables[0]);
                //if (dt.Tables.Count > 0 && dt.Tables[0].Rows.Count > 0)
                //{
                //    ViewData["userMaster"] = false; ViewData["FinancerMaster"] = false; ViewData["AccountDetails"] = false; 
                //    ViewData["ChangeTradeRefNo"] = false; ViewData["EODProcess"] = false; ViewData["PhysicalReceived"] = false;
                //    ViewData["DownloadInvoice"] = false; ViewData["UploadReceivedInvoiceData"] = false; ViewData["TallyBookingFCC"] = false;
                //    ViewData["AdditionalMISControlPoint"] = false; ViewData["MISLiquidation"] = false; ViewData["MISLiquidationDetailInvoice"] = false;
                //    ViewData["CancelationInvoiceAndDO"] = false; ViewData["Authorisation"] = false; ViewData["PaymentInformationCSVNew"] = false;
                //    ViewData["DOLiquidation"] = false; ViewData["FTPayment"] = false; ViewData["PaymentReport"] = false;
                //    ViewData["DocumentReleasedCnf"] = false; ViewData["PendingReport"] = false; ViewData["Gefu"] = false;
                //    ViewData["DoInformation"] = false;
                //    for (int i = 0; i < dt.Tables[0].Rows.Count - 1; i++)
                //    {
                //        if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim().Trim() == "UserMaster".ToString())
                //        { ViewData["userMaster"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]); }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim().Trim() == "FinancerMaster".ToString())
                //        { ViewData["FinancerMaster"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]); }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "AccountDetails".ToString())
                //        { ViewData["AccountDetails"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]); }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "ChangeTradeRefNo".ToString())
                //        { ViewData["ChangeTradeRefNo"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]); }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "EODProcess".ToString())
                //        { ViewData["EODProcess"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]); }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "PhysicalReceived".ToString())
                //        {
                //            ViewData["PhysicalReceived"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "DownloadInvoice".ToString())
                //        {
                //            ViewData["DownloadInvoice"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "UploadReceivedInvoiceData".ToString())
                //        {
                //            ViewData["UploadReceivedInvoiceData"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "TallyBookingFCC".ToString())
                //        {
                //            ViewData["TallyBookingFCC"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "AdditionalMISControlPoint".ToString())
                //        {
                //            ViewData["AdditionalMISControlPoint"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "MISLiquidation".ToString())
                //        {
                //            ViewData["MISLiquidation"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "MISLiquidationDetailInvoice".ToString())
                //        {
                //            ViewData["MISLiquidationDetailInvoice"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "CancelationInvoiceAndDO".ToString())
                //        {
                //            ViewData["CancelationInvoiceAndDO"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "Authorisation".ToString())
                //        {
                //            ViewData["Authorisation"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "PaymentInformationCSVNew".ToString())
                //        {
                //            ViewData["PaymentInformationCSVNew"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "DOLiquidation".ToString())
                //        {
                //            ViewData["DOLiquidation"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "FTPayment".ToString())
                //        {
                //            ViewData["FTPayment"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "PaymentReport".ToString())
                //        {
                //            ViewData["PaymentReport"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "DocumentReleasedCnf".ToString())
                //        {
                //            ViewData["DocumentReleasedCnf"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "PendingReport".ToString())
                //        {
                //            ViewData["PendingReport"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "Gefu".ToString())
                //        {
                //            ViewData["Gefu"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //        else if (dt.Tables[0].Rows[i]["SubMenuName"].ToString().Trim() == "DoInformation".ToString())
                //        {
                //            ViewData["DoInformation"] = Convert.ToBoolean(dt.Tables[0].Rows[i]["IsAccessible"]);
                //        }
                //    }
                //}

                return View();
            }
        }
        //public IActionResult HomePage(string token)
        //{


        //    //if (HttpContext.Session.GetString("LoginID") == null)
        //    //{ return RedirectToAction("LoginPage", "Login"); }
        //    //else
        //    //{
        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes("MSILApploginfail");
        //        var tokenHandler = new JwtSecurityTokenHandler();

        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false
        //        };

        //        tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        //var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        //        var username = jwtToken.Claims.ElementAt(0).ToString().Split(":");
        //        var Password = jwtToken.Claims.ElementAt(1).ToString().Split(":");
        //        using (var db = new Entities.DatabaseContextUAM())
        //        {
        //            var strlogId = db.Set<UAM_LoginLogoutExist>().FromSqlRaw("select MSIL_LogoutDatetime, LogID from MSIL_LoginLogout where User_id='" + username[1].ToString().Trim() + "' order by CONVERT(int, logID) desc ").ToList();
        //            if (strlogId.Count != 0)
        //            {
        //                if (strlogId[0].MSIL_LogoutDatetime == "" || strlogId[0].MSIL_LogoutDatetime == null)
        //                {
        //                    TempData["alertMessageuam"] = "User " + username[1].ToString().Trim() + " already logined.";
        //                    return View("LoginPageFail");
        //                }
        //            }
        //        }
        //        // Set username in session
        //        HttpContext.Session.SetString("LoginID", username[1].ToString().Trim());
        //        HttpContext.Session.SetString("UserName", username[1].ToString().Trim());
        //        HttpContext.Session.SetString("LoginID", username[1].ToString().Trim());
        //        UserSession.LoginID = HttpContext.Session.GetString("LoginID").ToString();
        //        LoginLogDB("", "", "LOGIN", UserSession.LoginID.ToString());
        //        //UserSession.LoginID = rec.LoginID.ToString();
        //        var builder = new ConfigurationBuilder()
        //      .SetBasePath(Directory.GetCurrentDirectory() + "\\")
        //      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        //        IConfigurationRoot configuration = builder.Build();
        //        string contentPath = Environment.ContentRootPath + "\\";
        //        HttpContext.Session.SetString("InputFilePath", contentPath + configuration.GetSection("MSILSettings:InputFilePath").Value);
        //        HttpContext.Session.SetString("OutputFilePath", contentPath + configuration.GetSection("MSILSettings:OutputFilePath").Value);
        //        HttpContext.Session.SetString("BackupFilePath", contentPath + configuration.GetSection("MSILSettings:BackupFilePath").Value);
        //        HttpContext.Session.SetString("NonConvertedFile", contentPath + configuration.GetSection("MSILSettings:NonConvertedFile").Value);
        //        HttpContext.Session.SetString("ErrorLog", contentPath + configuration.GetSection("MSILSettings:ErrorLog").Value);
        //        HttpContext.Session.SetString("AuditLog", contentPath + configuration.GetSection("MSILSettings:AuditLog").Value);
        //        HttpContext.Session.SetString("TRADE_EMAIL", configuration.GetSection("MSILSettings:TRADE_EMAIL").Value);
        //        HttpContext.Session.SetString("Frequency", configuration.GetSection("MSILSettings:Frequency").Value);
        //        HttpContext.Session.SetString("INV_CONF_EMAIL", configuration.GetSection("MSILSettings:INV_CONF_EMAIL").Value);
        //        HttpContext.Session.SetString("INV_CONF_FNAME", configuration.GetSection("MSILSettings:INV_CONF_FNAME").Value);
        //        HttpContext.Session.SetString("ORD_MIS_EMAIL", configuration.GetSection("MSILSettings:ORD_MIS_EMAIL").Value);
        //        HttpContext.Session.SetString("PAYREC_TRADE_EMAIL", configuration.GetSection("MSILSettings:PAYREC_TRADE_EMAIL").Value);
        //        HttpContext.Session.SetString("PHY_INV", configuration.GetSection("MSILSettings:PHY_INV").Value);
        //        HttpContext.Session.SetString("NO_INV", configuration.GetSection("MSILSettings:NO_INV").Value);
        //        HttpContext.Session.SetString("EOD_MIS", configuration.GetSection("MSILSettings:EOD_MIS").Value);
        //        HttpContext.Session.SetString("ORD_DEL", configuration.GetSection("MSILSettings:ORD_DEL").Value);
        //        HttpContext.Session.SetString("INV_PHY_NTRC", configuration.GetSection("MSILSettings:INV_PHY_NTRC").Value);
        //        HttpContext.Session.SetString("IntraDayPath", configuration.GetSection("MSILSettings:IntraDayPath").Value);
        //        HttpContext.Session.SetString("EOD_File_EMail", configuration.GetSection("MSILSettings:EOD_File_EMail").Value);
        //        HttpContext.Session.SetString("FCC_EMail", configuration.GetSection("MSILSettings:FCC_EMail").Value);
        //        HttpContext.Session.SetString("DRC_EMail", configuration.GetSection("MSILSettings:DRC_EMail").Value);
        //        HttpContext.Session.SetString("Payment_Rejection_EMail", configuration.GetSection("MSILSettings:Payment_Rejection_EMail").Value);
        //        HttpContext.Session.SetString("DRC_EMail_BNGR", configuration.GetSection("MSILSettings:DRC_EMail_BNGR").Value);
        //        HttpContext.Session.SetString("DRC_EMail_SLGR", configuration.GetSection("MSILSettings:DRC_EMail_SLGR").Value);
        //        HttpContext.Session.SetString("DO_Cancel_Email", configuration.GetSection("MSILSettings:DO_Cancel_Email").Value);
        //        HttpContext.Session.SetString("DO_Invoice_Cancel_Email", configuration.GetSection("MSILSettings:DO_Invoice_Cancel_Email").Value);
        //        HttpContext.Session.SetString("Invoice_Cancel_Email", configuration.GetSection("MSILSettings:Invoice_Cancel_Email").Value);
        //        HttpContext.Session.SetString("Sleep_Time_in_Mint", configuration.GetSection("MSILSettings:Sleep_Time_in_Mint").Value);
        //        HttpContext.Session.SetString("Confirmation_Mail", configuration.GetSection("EmailSetting:Confirmation_Mail").Value);
        //        HttpContext.Session.SetString("SMTP_HOST", configuration.GetSection("EmailSetting:SMTP_HOST").Value);
        //        HttpContext.Session.SetString("Port", configuration.GetSection("EmailSetting:Port").Value);
        //        HttpContext.Session.SetString("Email_FromID", configuration.GetSection("EmailSetting:Email_FromID").Value);
        //        HttpContext.Session.SetString("UserID", configuration.GetSection("EmailSetting:UserID").Value);
        //        HttpContext.Session.SetString("Password", configuration.GetSection("EmailSetting:Password").Value);
        //        HttpContext.Session.SetString("SysEmail_FromID", configuration.GetSection("SystemSetting:Pwd").Value);
        //        HttpContext.Session.SetString("PWD", configuration.GetSection("SystemSetting:Pwd").Value);
        //        HttpContext.Session.SetString("UAMPath", configuration.GetSection("MSILSettings:UAMPath").Value);
        //        //HttpContext.Session.SetString("TabCount", "1");
        //        _logger.LogInformation("The MSIL application login : User Name - " + username[1].ToString().Trim());
        //        return View();
        //        //return RedirectToAction("Index", "Home"); // Redirect to a desired page
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString() + " - LoginController;Save");
        //        TempData["alertMessageuam"] = "An error occurred while trying to access the provided URL. Please verify the link or provide more details for assistance.";
        //        return View("LoginPageFail");
        //    }

        //    //}

        //}
        [HttpPost]
        public IActionResult TrackWindowClose()
        {
            // Your logic for handling window close event, e.g., logging, session cleanup, etc.
            Logout();
            // This action will be called when the client sends the beacon request.
            return Ok();  // You can return any response (like a status message)
        }

        public IActionResult UserMaster()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }

        public IActionResult SaveUser()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }

        public IActionResult RegisterNew()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult RegisterNew(RegisterNew registerNew)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                int x = 0;


                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        //var reg = db.Database.ExecuteSqlRaw("");
                        //db.SaveChanges();
                        //https://www.aspsnippets.com/questions/377667/How-to-validate-user-for-Signup-using-Stored-Procedure-in-SQL-Server/

                        using (var command = db.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "Customer_Signup";
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@LoginName", registerNew.LoginName));
                            command.Parameters.Add(new SqlParameter("@Password", registerNew.Password));
                            command.Parameters.Add(new SqlParameter("@ReTypePassword", registerNew.ReTypePassword));
                            command.Parameters.Add(new SqlParameter("@LoginType", registerNew.LoginType));
                            command.Parameters.Add(new SqlParameter("@Flag", 1));

                            db.Database.OpenConnection();
                            using (var result = command.ExecuteReader())
                            {
                                if (result.HasRows)
                                {
                                    result.Read();
                                    x = result.GetInt32(0); // x = your sp count value
                                                            //return x;
                                }
                            }
                            db.Database.CloseConnection();
                        }
                    }
                    string message = string.Empty;
                    switch (x)
                    {
                        case -1:
                            message = "User Name has already been used \\nPlease choose a different User Name.";
                            break;
                        case -2:
                            message = "Passwords do not match.";
                            break;
                        default:
                            message = "Registration successful. Activation User Name has been sent.";
                            //SendActivationEmail(userId);
                            break;
                    }
                    TempData["alertMessage"] = message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - LoginController;RegisterNew");
                }
                return View(registerNew);
            }

        }

        [HttpPost]
        public IActionResult Save(UserMaster userMaster)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                int x = 0;

                if (userMaster.LoginType == "0" || userMaster.LoginType == "Select Role Type")
                {
                    TempData["alertMessage"] = "Please select the Role Type";
                    return RedirectToAction("UserMaster", "Login");
                }

                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        //var reg = db.Database.ExecuteSqlRaw("");
                        //db.SaveChanges();
                        //https://www.aspsnippets.com/questions/377667/How-to-validate-user-for-Signup-using-Stored-Procedure-in-SQL-Server/

                        using (var command = db.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "Customer_Signup";
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@LoginName", userMaster.LoginName));
                            command.Parameters.Add(new SqlParameter("@Password", userMaster.Password));
                            command.Parameters.Add(new SqlParameter("@ReTypePassword", userMaster.ReTypePassword));
                            command.Parameters.Add(new SqlParameter("@LoginType", userMaster.LoginType));
                            command.Parameters.Add(new SqlParameter("@Flag", 1));
                            db.Database.OpenConnection();
                            using (var result = command.ExecuteReader())
                            {
                                if (result.HasRows)
                                {
                                    result.Read();
                                    x = result.GetInt32(0); // x = your sp count value
                                                            //return x;
                                }
                            }
                            db.Database.CloseConnection();
                        }
                    }
                    string message = string.Empty;
                    switch (x)
                    {
                        case -1:
                            message = "User Name has already been used \\nPlease choose a different User Name.";
                            break;
                        case -2:
                            message = "Passwords do not match.";
                            break;
                        default:
                            message = "Registration successful. Activation User Name has been sent.";
                            //SendActivationEmail(userId);
                            break;
                    }
                    TempData["alertMessage"] = message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - LoginController;Save");

                }
                //return View(userMaster);
                return RedirectToAction("UserMaster", "Login");
            }
        }

        [HttpPost]
        public IActionResult New(RegisterNew registerNew)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                try
                {
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - LoginController;New");
                }
                return RedirectToAction("UserMaster", "Login");
            }
        }

        public IActionResult AccountDetails()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }



        public IActionResult Trade_OPSHomePage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            {
                Logout();
                return RedirectToAction("UAMLoginNew", "LoginUAM");
            }
            else
            {
                return View();
            }
        }
        public IActionResult Cash_OPSHomePage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { Logout(); return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }
        public IActionResult Cash_TradeHomePage()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { Logout(); return RedirectToAction("UAMLoginNew", "LoginUAM"); }
            else
            {
                return View();
            }
        }

        //[HttpPost]
        //public IActionResult AccountDetails(AccountDetails accountDetails)
        //{
        //    using (var db = new Entities.DatabaseContext())
        //    {
        //        // var reg = db.Database.ExecuteSqlRaw("");
        //        var result = db.Account_Details.Add(accountDetails);
        //        db.SaveChanges();
        //       var  message = "Payment Type added successful.";             

        //    TempData["alertMessage"] = message;
        //        ModelState.Clear();

        //    return View();
        //    }
        //}             

        //public ActionResult FillModel()
        //{

        //    return View();

        //}

        //[HttpPost]
        //public ActionResult FillModel(AccountDetails accde)
        //{
        //    //  AccountDetails accde = new AccountDetails();

        //    using (var db = new Entities.DatabaseContext())
        //    {
        //        var inv1 = db.Set<AccountDetails>().FromSqlRaw("Select * from Account_Details where Payment_Type='" + accde.Payment_Type + "'").ToList();
        //        if (inv1.Count > 0)
        //        {

        //            accde.CR_Account_No = inv1[0].CR_Account_No.ToString();
        //            accde.DR_Account_No = inv1[0].DR_Account_No.ToString();
        //        }

        //        //if (Paymenttype == "Select Payment Type")
        //        //{
        //        //    accde.CR_Account_No = "";
        //        //    accde.DR_Account_No = "";
        //        //}
        //    }
        //  //return View(accde);
        //     return RedirectToPage("FillModel", accde);
        //}

    }

}

using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Controllers
{
    public class StockyardController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
      
        public StockyardController(ILogger<PaymentInformationController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;            
        }

        public async Task<IActionResult> ShowStockyard(int pg = 1)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            {  return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        var inv1 = await db.Set<ShowStockyardMaster>().FromSqlRaw("Select ID, Name, Code, DO_Number, Email, IsActive from Stockyard_Master").ToListAsync();
                        //TempData["alertMessage"]=null;
                        //return View(inv1);

                        const int pageSize = 15;
                        if (pg < 1)
                            pg = 1;

                        int recsCount = inv1.Count();
                        var pager = new Pager(recsCount, pg, pageSize);
                        int recSkip = (pg - 1) * pageSize;
                        var data = inv1.Skip(recSkip).Take(pager.PageSize).ToList();

                        this.ViewBag.Pager = pager;
                        TempData["alertMessage"] = null;

                        return View(data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - LoginController;ShowStockyard");
                    return View("ShowStockyard");
                }
            }
        }



        public IActionResult CreatestockyardDetails()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View("SaveStockyardDetails");
            }

        }
        public async Task<IActionResult> UpdatestockyardDetails(int? ID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (ID == null)
                {
                    return NotFound();
                }
                using (var db = new Entities.DatabaseContext())
                {
                    // var employee = await db.Set<DetailsUserMaster>().FromSqlRaw("Select LoginID, LoginName,LoginType,Login_Enable from Login Where LoginID=" + LoginID +"").ToListAsync();
                    var StockyardMaster = await db.StockyardMST.FirstOrDefaultAsync(m => m.ID == ID);
                    if (StockyardMaster == null)
                    {
                        return NotFound();
                    }

                    return View(StockyardMaster);
                }
            }

        }
        public async Task<IActionResult> StockyardDisplay(int? ID)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (ID == null)
                {
                    return NotFound();
                }
                using (var db = new Entities.DatabaseContext())
                {
                    // var employee = await db.Set<DetailsUserMaster>().FromSqlRaw("Select LoginID, LoginName,LoginType,Login_Enable from Login Where LoginID=" + LoginID +"").ToListAsync();
                    var StockyardMaster = await db.StockyardMST.FirstOrDefaultAsync(m => m.ID == ID);
                    if (StockyardMaster == null)
                    {
                        return NotFound();
                    }
                    return View(StockyardMaster);
                }
            }

        }
        public IActionResult SaveStockyardDetails(string Task, StockyardMaster stockyard)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                 
                    if (stockyard.ID == null && stockyard.Name == null && stockyard.Code == null && stockyard.DO_Number == null)
                    {
                        using (var db = new Entities.DatabaseContext())
                        {

                            var inv1 = db.Set<ShowStockyardMaster>().FromSqlRaw("Select ID, Name, Code, DO_Number, Email, IsActive from Stockyard_Master").ToList();
                            return View("ShowStockyard", inv1);
                        }
                    }
                    string status = "";
                    if (stockyard.Code == null || stockyard.Code == "0")
                    {
                        TempData["alertMessage"] = "Please Enter Code";
                        status = "Error";
                    }
                    else if (stockyard.Name == null || stockyard.Name == "")
                    {
                        TempData["alertMessage"] = "Please Enter Name";
                        status = "Error";
                    }
                    else if (stockyard.DO_Number == null || stockyard.DO_Number == "")
                    {
                        TempData["alertMessage"] = "Please Enter Do Number";
                        status = "Error";
                    }
                    else if (stockyard.Email == null || stockyard.Email == "")
                    {
                        TempData["alertMessage"] = "Please Enter at least Email ID";
                        status = "Error";
                    }
                    var IsValid = EmailValidator.ValidateEmails(stockyard.Email);
                    if (IsValid.Count != 0)
                    {
                        string InvalidEMail = "";
                        foreach (var email in IsValid)
                        {
                            InvalidEMail = InvalidEMail + email+",";
                        }
                        InvalidEMail = InvalidEMail.Substring(0, InvalidEMail.Length - 1);
                        TempData["alertMessage"] = "Please enter the correct EmailID for " + InvalidEMail;
                        status = "Error";
                    }
                    //else if (Financer.FCode == null || Financer.FCode == "")
                    //{
                    //    TempData["alertMessage"] = "Please Enter FCode";
                    //    status = "Error";
                    //}
                    if (status == "")
                    {
                        if (Task == "save")
                            stockyard.ID = 0;
                        DataTable dataTable = new DataTable();
                        using (var db = new Entities.DatabaseContext())
                        {
                            if (stockyard.Code == null)
                                stockyard.Code = "";
                            if (stockyard.DO_Number == null)
                                stockyard.DO_Number = "";
                            if (stockyard.Email == null)
                                stockyard.Email = "";
                            DbConnection connection = db.Database.GetDbConnection();
                            using var cmd = db.Database.GetDbConnection().CreateCommand();
                            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                            cmd.CommandText = "SP_StockyardMaster";

                            //common
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                            cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar) { Value = stockyard.ID });
                            cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = stockyard.Name.ToUpper().Trim() });
                            cmd.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar) { Value = stockyard.Code.ToUpper().Trim() });
                            cmd.Parameters.Add(new SqlParameter("@DO_Number", SqlDbType.VarChar) { Value = stockyard.DO_Number.ToUpper().Trim() });
                            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = stockyard.Email.ToUpper().Trim() });
                            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.VarChar) { Value = UserSession.LoginID.ToString() });

                            //cmd.ExecuteReader();  
                            using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                            {
                                adapter.SelectCommand = cmd;
                                adapter.Fill(dataTable);
                            }
                            cmd.Connection.Close();
                        }
                        //if (Task=="Save")
                        TempData["alertMessage"] = dataTable.Rows[0][0].ToString();
                        ModelState.Clear();
                    }
                }
                catch (Exception ex)
                {
                    if (Task == "Update" && stockyard.ID == null)
                        TempData["alertMessage"] = "stockyard can not be created in Update Details.";
                    _logger.LogError(ex.ToString() + " - LoginController;RegisterNew");
                }



                string ViewName = "";
                if (Task == "save")
                    ViewName = "SaveStockyardDetails";
                else if (Task == "Update")
                    ViewName = "UpdateStockyardDetails";

                return View(ViewName);
            }

        }
        public class EmailValidator
        {
            private static readonly Regex emailRegex = new Regex(
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            public static List<string> ValidateEmails(string emailInput)
            {
                var invalidEmails = new List<string>();
                var emailArray = emailInput.Split(',');

                foreach (var email in emailArray)
                {
                    var trimmedEmail = email.Trim();
                    if (!emailRegex.IsMatch(trimmedEmail))
                    {
                        invalidEmails.Add(trimmedEmail);
                    }
                }

                return invalidEmails;
            }
        }
    }
}

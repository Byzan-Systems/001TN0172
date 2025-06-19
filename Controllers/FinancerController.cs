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
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Controllers
{
    public class FinancerController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
      
        public FinancerController(ILogger<PaymentInformationController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;            
        }

        public async Task<IActionResult> ShowFinancer(int pg = 1)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            {  return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        var inv1 = await db.Set<ShowFinancerMaster>().FromSqlRaw("Select ID, FType,FName,IFSCPart,FCode from FinancerDetails").ToListAsync();
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
                    _logger.LogError(ex.ToString() + " - LoginController;ShowFinancer");
                    return View("ShowFinancer");
                }
            }
        }



        public IActionResult CreateFinancerDetails()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View("SaveFinancerDetails");
            }

        }
        public async Task<IActionResult> UpdateFinancerDetails(int? ID)
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
                    var financerMaster = await db.FinancerMST.FirstOrDefaultAsync(m => m.ID == ID);
                    if (financerMaster == null)
                    {
                        return NotFound();
                    }

                    return View(financerMaster);
                }
            }

        }
        public async Task<IActionResult> FinancerDisplay(int? ID)
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
                    var financerMaster = await db.FinancerMST.FirstOrDefaultAsync(m => m.ID == ID);
                    if (financerMaster == null)
                    {
                        return NotFound();
                    }
                    return View(financerMaster);
                }
            }

        }
        public IActionResult SaveFinancerDetails(string Task, FinancerMaster Financer)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    if (Financer.ID == null && Financer.FType == null && Financer.FName == null && Financer.FCode == null)
                    {
                        using (var db = new Entities.DatabaseContext())
                        {

                            var inv1 = db.Set<ShowFinancerMaster>().FromSqlRaw("Select ID, FType,FName,IFSCPart,FCode from FinancerDetails").ToList();
                            return View("ShowFinancer", inv1);
                        }
                    }
                    string status = "";
                    if (Financer.FType == null || Financer.FType == "0")
                    {
                        TempData["alertMessage"] = "Please Select FType";
                        status = "Error";
                    }
                    else if (Financer.FName == null || Financer.FName == "")
                    {
                        TempData["alertMessage"] = "Please Enter FName";
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
                            Financer.ID = 0;
                        DataTable dataTable = new DataTable();
                        using (var db = new Entities.DatabaseContext())
                        {
                            if (Financer.IFSCPart == null)
                                Financer.IFSCPart = "";
                            if (Financer.FCode == null)
                                Financer.FCode = "";
                            DbConnection connection = db.Database.GetDbConnection();
                            using var cmd = db.Database.GetDbConnection().CreateCommand();
                            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                            cmd.CommandText = "SP_FinancerMaster";

                            //common
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                            cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar) { Value = Financer.ID });
                            cmd.Parameters.Add(new SqlParameter("@FType", SqlDbType.VarChar) { Value = Financer.FType.ToUpper() });
                            cmd.Parameters.Add(new SqlParameter("@FName", SqlDbType.VarChar) { Value = Financer.FName.ToUpper() });
                            cmd.Parameters.Add(new SqlParameter("@IFSCPart", SqlDbType.VarChar) { Value = Financer.IFSCPart.ToUpper() });
                            cmd.Parameters.Add(new SqlParameter("@FCode", SqlDbType.VarChar) { Value = Financer.FCode.ToUpper() });
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
                    if (Task == "Update" && Financer.ID == null)
                        TempData["alertMessage"] = "Financer can not be created in Update Details.";
                    _logger.LogError(ex.ToString() + " - LoginController;RegisterNew");
                }



                string ViewName = "";
                if (Task == "save")
                    ViewName = "SaveFinancerDetails";
                else if (Task == "Update")
                    ViewName = "UpdateFinancerDetails";

                return View(ViewName);
            }

        }

    }
}

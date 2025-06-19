using ClosedXML.Excel;
using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace HDFCMSILWebMVC.Controllers
{
    public class TallyBookingFCCController : Controller
    {
        public Entities.DatabaseContext db = new Entities.DatabaseContext();
        public static TallyBookingFCC TBFccc = new TallyBookingFCC();
        public static IList<TallyBookingFCCData> TallyFCCData = new List<TallyBookingFCCData>();

        private readonly ILogger _logger;
        public TallyBookingFCCController(ILogger<TallyBookingFCCController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }

        public IActionResult TallyBookingFCC()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult TallyBookingFCC_Show(TallyBookingFCC Fcc)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    string Fromdate = "";
                    string Todate = "";
                    try
                    {

                        Fromdate = DateTime.ParseExact(Fcc.DateFrom.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        Todate = DateTime.ParseExact(Fcc.DateTo.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd");
                    }
                    catch (Exception )
                    {
                        TempData["alertMessage"] = "Please Please proper date.";
                        return PartialView("TallyBookingFCC");
                    }
                    TBFccc = Fcc;
                    TallyFCCData = db.Set<TallyBookingFCCData>().FromSqlRaw("EXEC SP_GetDetails_Web @Task = 'Get_TallyBookingFCC_Report',@Search1='" + Fromdate + "',@Search2='" + Todate + "',@Search3='',@Search4='',@Search5='',@Search6='',@Search7=''").ToList();
                    DataTable dtR = getDetails("Get_TallyBookingFCC_Report", Fromdate, Todate, "", "", "", "", "");

                    return View(TallyFCCData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - TallyBookingFCCController;TallyBookingFCC_Show");
                    return PartialView("TallyBookingFCC");
                }
            }
        }
        [HttpPost]

        public IActionResult ExportExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {

                    if (TallyFCCData.Count > 0)
                    {
                        var DetailsList = TallyFCCData.ToList();
                        DataTable Details = DetailsList.ToDataTable();
                        Details.TableName = "Sheet1";
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(Details);
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TallyBookingFCC.xlsx");
                            }
                        }
                    }
                    else
                    {
                        TempData["alertMessage"] = "No Data to Download.";
                        return PartialView("TallyBookingFCC_Show", TallyFCCData);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - TallyBookingFCCController;ExportExcel");
                }

                return View();
            }
        }
        public static DataTable getDetails(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_GetDetails_Web";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }


                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception ) { return null; }
        }

    }
}

using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Data;
using MoreLinq;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace HDFCMSILWebMVC.Controllers
{
    public class MISLiquidationController : Controller
    {
        private readonly ILogger _logger;
        public MISLiquidationController(ILogger<MISLiquidationController> logger)
        {
            _logger = logger;
        }

        //public static IList<TallyBookingFCCData> TallyFCCData = new List<TallyBookingFCCData>();

        public static IList<MISLiquidation> misLiquidation = new List<MISLiquidation>();
        public IActionResult ShowMISLiquidation()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ShowTableMISLiquidation(ShowMISLiquidation misliq)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View(GetCustomerList(misliq.DateAsOn.ToString(), misliq.TradeStartDate.ToString(), misliq.TradeEndtDate.ToString()));
            }
            
        }

        public IList<MISLiquidation> GetCustomerList(string  currentdate ,string fromdate, string todate)
        {
            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    string currentdate1 = DateTime.ParseExact(currentdate.Split(' ')[0].ToString().ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string todate1 = todate.Split(' ')[0].ToString();
                    string fromdate1 = fromdate.Split(' ')[0].ToString();
                    string Fromdate_F = DateTime.ParseExact(fromdate.Split(' ')[0].ToString().ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string Todate_F = DateTime.ParseExact(todate.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd");

                    misLiquidation = db.Set<MISLiquidation>().FromSqlRaw("EXEC MIS_Consolidated_Liqudation @Currentdate ='" + currentdate1.ToString() + "',@FromDate='" + Fromdate_F.ToString() + "',@ToDate='" + Todate_F.ToString() + "'").ToList();
                }

                _logger.LogInformation("Executed successfully" + " - MISLiquidationController;GetCustomerList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - MISLiquidationController;GetCustomerList");
            }

            return misLiquidation;
        }

        public IActionResult GenExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                string message = string.Empty;
                var misLiquidations = misLiquidation.ToList();
                DataTable Details = misLiquidations.ToDataTable();
                Details.TableName = "Sheet1";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(Details);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        var filename = "MISLiquidationReport_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
                        ViewBag.Message = string.Format("MIS Liquidation Report {0}.\\ File Dowload successfully:", filename);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
            }
            //return View();
        }

    }
}

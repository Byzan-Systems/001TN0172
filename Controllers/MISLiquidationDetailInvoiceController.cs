using ClosedXML.Excel;
using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace HDFCMSILWebMVC.Controllers
{
    public class MISLiquidationDetailInvoiceController : Controller
    {
        private readonly ILogger _logger;
        public MISLiquidationDetailInvoiceController(ILogger<MISLiquidationDetailInvoiceController> logger)
        {
            _logger = logger;
        }

        public static IList<MISLiquidationDetailInvoice> misLiquidation = new List<MISLiquidationDetailInvoice>();
        public IActionResult ShowMISLiquidationDetailInvoice()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult ShowTableMISLiquidationDetailInvoice(ShowMISLiquidationDetailInvoice misliq)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View(GetCustomerList(misliq.StartDate.ToString(), misliq.EndtDate.ToString()));
            }
            
        }

        public IList<MISLiquidationDetailInvoice> GetCustomerList(string fromdate, string todate)
        {
            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    misLiquidation.Clear();
                    string todate1 = todate.Split(' ')[0].ToString();
                    string fromdate1 = fromdate.Split(' ')[0].ToString();
                    string Fromdate_F = DateTime.ParseExact(fromdate.Split(' ')[0].ToString().ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    string Todate_F = DateTime.ParseExact(todate.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd");


                    misLiquidation = db.Set<MISLiquidationDetailInvoice>().FromSqlRaw("EXEC MIS_Consolidated_Liqudation_Detail @Fromdate='" + Fromdate_F.ToString() + "',@Todate='" + Todate_F.ToString() + "'").ToList();

                    _logger.LogInformation("Executed successfully" + " - MISLiquidationDetailInvoiceController;GetCustomerList");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - MISLiquidationDetailInvoiceController;GetCustomerList");
            }

            return misLiquidation;
        }

        public IActionResult ExportExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                    var DetailsList = misLiquidation.ToList();
                    DataTable Details = DetailsList.ToDataTable();
                    Details.TableName = "Sheet1";
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(Details);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MIS Liquidation-DetailInvoice.xlsx");
                        }
                    }

                    _logger.LogInformation("Data exported successfully" + " - MISLiquidationDetailInvoiceController;ExportExcel");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - MISLiquidationDetailInvoiceController;ExportExcel");
                }

                return View();
            }
        }
    }
}

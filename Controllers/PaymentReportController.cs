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
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace HDFCMSILWebMVC.Controllers
{
    public class PaymentReportController : Controller
    {
        private readonly ILogger _logger;
        public PaymentReportController(ILogger<PaymentReportController> logger)
        {
            _logger = logger;
        }

        private static List<ShowCashOps_Upload> ShowCashOps_UploadList = new List<ShowCashOps_Upload>();
        public IActionResult ShowPaymentReport()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
            
        }

        public IActionResult ShowTablePaymentReport(ShowPaymentReportSelect showpay)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                DataTable dt = new DataTable();

                try
                {

                    using (var db = new Entities.DatabaseContext())
                    {
                        db.Database.ExecuteSqlRaw("Update CashOps_Upload set Payment_Status='Pending Credit' where Payment_Status='REJECT - Order Mismatch'");
                        db.SaveChanges();

                        if (showpay.chkpayrecDate == true)
                        {
                            ShowCashOps_UploadList = db.Set<ShowCashOps_Upload>().FromSqlRaw("EXEC uspShowCashOps_Upload @ToCash_Ops_Date='" + showpay.StartDate + "',@FromCash_Ops_Date='" + showpay.EndDate + "',@Payment_Status='',@Flag=1").ToList();
                            ListtoDataTable lsttodt = new ListtoDataTable();
                            dt = lsttodt.ToDataTable(ShowCashOps_UploadList);

                        }
                        else if (showpay.Status == true)
                        {
                            ShowCashOps_UploadList = db.Set<ShowCashOps_Upload>().FromSqlRaw("EXEC uspShowCashOps_Upload @ToCash_Ops_Date='',@FromCash_Ops_Date='',@Payment_Status='" + showpay.ReportType + "',@Flag=2").ToList();
                            ListtoDataTable lsttodt = new ListtoDataTable();
                            dt = lsttodt.ToDataTable(ShowCashOps_UploadList);
                        }

                        _logger.LogInformation("Executed successfully" + " - PaymentReportController;ShowTablePaymentReport");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - PaymentReportController;ShowTablePaymentReport");
                }

                return View(dt);
            }
        }
        public IActionResult ExportExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    var DetailsList = ShowCashOps_UploadList.ToList();
                    DataTable Details = DetailsList.ToDataTable();
                    Details.TableName = "Sheet1";
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(Details);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            _logger.LogInformation("Data exported successfully" + " - PaymentReportController;ExportExcel");
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PaymentReport.xlsx");
                        }

                      
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - PaymentReportController;ExportExcel");
                }

                return View();
            }
        }
        public class ListtoDataTable
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
        }
    }
}

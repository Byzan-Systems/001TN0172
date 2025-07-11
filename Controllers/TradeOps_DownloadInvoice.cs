using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;

namespace HDFCMSILWebMVC.Controllers
{
    public class TradeOps_DownloadInvoice : Controller
    {
        private readonly ILogger _logger;
        public TradeOps_DownloadInvoice(ILogger<TradeOps_DownloadInvoice> logger)
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

        public IActionResult DownloadInvoice()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
            
        }
        // https://aspnet-core-grid.azurewebsites.net/?page=2
        //public JsonResult Show(DownloadinvFrmDeta DInvDa)
        //{
        //    int TotInv = 0;
        //    IList<DownloadFillInvoice> inv = new List<DownloadFillInvoice>();
        //    try
        //    {
        //        using (var db = new Entities.DatabaseContext())
        //        {

        //            if (DInvDa.ChkInvoiceNo == true)
        //            {
        //                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number ='" + DInvDa.Invoice_Number + "',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=1").ToList();

        //            }
        //            else if (DInvDa.ChkDate == true)
        //            {
        //                var Fromdate = DInvDa.DateFrom;
        //                var Todate = DInvDa.DateTo;


        //                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='',@Flag=2").ToList();


        //            }
        //            else if (DInvDa.ChkReporttype == true)
        //            {
        //                if (DInvDa.RerportType == "With Trade Ref.No")
        //                {

        //                    inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();


        //                }
        //                else if (DInvDa.RerportType == "Without Trade Ref.No")
        //                {
        //                    inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();


        //                }
        //                else
        //                {

        //                }
        //            }
        //            else
        //            {
        //                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=4").ToList();




        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(inv);
        //}


        [HttpPost]
        public IActionResult Show(DownloadinvFrmDeta DInvDa)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                //int TotInv = 0;
                IList<DownloadFillInvoice> inv = new List<DownloadFillInvoice>();
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {

                        if (DInvDa.ChkInvoiceNo == true)
                        {
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number, @ToInvoiceDate, @FromInvoiceDate, @ReportType, @Flag",
                                new SqlParameter("@Invoice_Number", DInvDa.Invoice_Number ?? (object)DBNull.Value),
                                new SqlParameter("@ToInvoiceDate", DBNull.Value),
                                new SqlParameter("@FromInvoiceDate", DBNull.Value),
                                new SqlParameter("@ReportType", DBNull.Value),
                                new SqlParameter("@Flag", 1)
                            ).ToList();
                            //inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number ='" + DInvDa.Invoice_Number + "',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=1").ToList();

                        }
                        else if (DInvDa.ChkDate == true)
                        {
                            var Fromdate = DInvDa.DateFrom;
                            var Todate = DInvDa.DateTo;

                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw(
                                "EXEC uspDownloadInvoice @Invoice_Number, @ToInvoiceDate, @FromInvoiceDate, @ReportType, @Flag",
                                new SqlParameter("@Invoice_Number", DBNull.Value),
                                new SqlParameter("@ToInvoiceDate", Fromdate ?? (object)DBNull.Value),
                                new SqlParameter("@FromInvoiceDate", Todate ?? (object)DBNull.Value),
                                new SqlParameter("@ReportType", DBNull.Value),
                                new SqlParameter("@Flag", 2)
                            ).ToList();
                            //inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='',@Flag=2").ToList();


                        }
                        else if (DInvDa.ChkReporttype == true)
                        {
                            if (DInvDa.RerportType == "With Trade Ref.No")
                            {
                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw(
                                    "EXEC uspDownloadInvoice @Invoice_Number, @ToInvoiceDate, @FromInvoiceDate, @ReportType, @Flag",
                                    new SqlParameter("@Invoice_Number", DBNull.Value),
                                    new SqlParameter("@ToInvoiceDate", DBNull.Value),
                                    new SqlParameter("@FromInvoiceDate", DBNull.Value),
                                    new SqlParameter("@ReportType", DInvDa.RerportType ?? (object)DBNull.Value),
                                    new SqlParameter("@Flag", 3)
                                ).ToList();
                                //inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();


                            }
                            else if (DInvDa.RerportType == "Without Trade Ref.No")
                            {
                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw(
                                    "EXEC uspDownloadInvoice @Invoice_Number, @ToInvoiceDate, @FromInvoiceDate, @ReportType, @Flag",
                                    new SqlParameter("@Invoice_Number", DBNull.Value),
                                    new SqlParameter("@ToInvoiceDate", DBNull.Value),
                                    new SqlParameter("@FromInvoiceDate", DBNull.Value),
                                    new SqlParameter("@ReportType", DInvDa.RerportType ?? (object)DBNull.Value),
                                    new SqlParameter("@Flag", 3)
                                ).ToList();


                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw(
                                "EXEC uspDownloadInvoice @Invoice_Number, @ToInvoiceDate, @FromInvoiceDate, @ReportType, @Flag",
                                new SqlParameter("@Invoice_Number", DBNull.Value),
                                new SqlParameter("@ToInvoiceDate", DBNull.Value),
                                new SqlParameter("@FromInvoiceDate", DBNull.Value),
                                new SqlParameter("@ReportType", DBNull.Value),
                                new SqlParameter("@Flag", 4)
                            ).ToList();
                            //inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=4").ToList();
                        }
                    }

                    _logger.LogInformation("Executed successfully" + " - TradeOps_DownloadInvoice;Show");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - TradeOps_DownloadInvoice;Show");
                }


                // var json1 = JsonConvert.SerializeObject(inv); // returns "{\"PracticeName\":\"1\"}"
                // var x1 = JsonConvert.DeserializeObject<DownloadFillInvoice>(json1); // correctly builds a C1          


                return Json(inv);
            }
            //return View();//RedirectToAction("DownloadInvoice", "TradeOps_DownloadInvoice");
        }

    }
}

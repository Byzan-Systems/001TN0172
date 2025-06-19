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

namespace HDFCMSILWebMVC.Controllers
{
    public class AdditionalMISControlPoint : Controller
    {

        private readonly ILogger _logger;
        public AdditionalMISControlPoint(ILogger<AdditionalMISControlPoint> logger)
        {
            _logger = logger;
        }

        public static IList<showAddMISControlPointDT> inv = new List<showAddMISControlPointDT>();
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
        }

        public IActionResult Show(showAddMISControlPoint Selectdata)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                showAddMISControlPointDT invdata = new showAddMISControlPointDT();
                if (Selectdata == null)
                {
                    return NotFound();
                }
                IList<AddMISControlPoint_INVDate> INvdate = new List<AddMISControlPoint_INVDate>();
                IList<AddMISControlPoint_Order_Rec> Order_Rec = new List<AddMISControlPoint_Order_Rec>();
                IList<AddMISControlPoint_PHyInvRec> PHyInvRec = new List<AddMISControlPoint_PHyInvRec>();
                IList<AddMISControlPoint_Payment_Recev> Payment_Recev = new List<AddMISControlPoint_Payment_Recev>();
                string Invoice_data_Received_Date; string TOTAL_Invoice; string stepdate; string PHyInvRec1; string Order_Rec1; string Payment_Recev1;

                try
                {
                    inv.Clear();
                    using (var db = new Entities.DatabaseContext())
                    {
                        var Fromdate = Selectdata.FromDate.ToString("yyyy-MM-dd");
                        var Todate = Selectdata.ToDate.AddDays(1).ToString("yyyy-MM-dd");
                        INvdate = db.Set<AddMISControlPoint_INVDate>().FromSqlRaw("SELECT distinct convert(varchar(10),Invoice_data_Received_Date,121) as Invoice_data_Received_Date,convert(varchar(50),count(convert(int,Invoice_ID))) AS Total_Invoice  From Invoice WHERE (Invoice_data_Received_Date BETWEEN '" + Fromdate + "' and '" + Todate + "') GROUP BY convert(varchar(10),Invoice_data_Received_Date,121) order BY convert(varchar(10),Invoice_data_Received_Date,121)").ToList();

                        if (INvdate.Count > 0)
                        {
                            foreach (var row in INvdate)
                            {
                                Invoice_data_Received_Date = "";
                                TOTAL_Invoice = ""; stepdate = ""; PHyInvRec1 = ""; Order_Rec1 = ""; Payment_Recev1 = "";

                                Invoice_data_Received_Date = Convert.ToDateTime(row.Invoice_data_Received_Date).ToString("yyyy-MM-dd");
                                TOTAL_Invoice = row.Total_Invoice;

                                PHyInvRec = db.Set<AddMISControlPoint_PHyInvRec>().FromSqlRaw("select convert(varchar(20),count(*) )as PHyInvRec,convert(varchar(30),StepDate,121) as StepDate from Invoice where Tradeop_Selected_invoice_Flag=1 and convert(date,Invoice_data_Received_Date) = convert(date, '" + Invoice_data_Received_Date + "') and StepDate<>'' and StepDate is not null group by convert(varchar(30),StepDate,121) order by  convert(varchar(30),StepDate,121)").ToList();

                                if (PHyInvRec.Count > 0)
                                {
                                    foreach (var row1 in PHyInvRec)
                                    {
                                        stepdate = Convert.ToDateTime(row1.StepDate).ToString("yyyy-MM-dd");
                                        PHyInvRec1 = row1.PHyInvRec;
                                        Order_Rec = db.Set<AddMISControlPoint_Order_Rec>().FromSqlRaw("SELECT convert(varchar(30),count(*)) AS Order_Rec From Invoice WHERE convert(date,Invoice_data_Received_Date) =  convert(date, '" + Invoice_data_Received_Date + "') AND convert(date,StepDate) =  convert(date,'" + stepdate + "') AND (Ord_Inv_ID <> '') AND (Ord_Inv_ID IS NOT NULL)").ToList();

                                        if (Order_Rec.Count > 0)
                                        {
                                            Order_Rec1 = Order_Rec[0].Order_Rec.ToString();
                                        }
                                        else
                                        {
                                            Order_Rec1 = "0";
                                        }

                                        Payment_Recev = db.Set<AddMISControlPoint_Payment_Recev>().FromSqlRaw("SELECT convert(varchar(30),count(*)) AS Payment_Recev From Invoice WHERE convert(date,Invoice_data_Received_Date) =  convert(date,'" + Invoice_data_Received_Date + "') AND convert(date,StepDate) =  convert(date,'" + stepdate + "') AND (Cash_Ops_ID <> '') AND (Cash_Ops_ID IS NOT NULL)").ToList();

                                        if (Payment_Recev.Count > 0)
                                        {
                                            Payment_Recev1 = Payment_Recev[0].Payment_Recev.ToString();
                                        }
                                        else
                                        {
                                            Payment_Recev1 = "0";
                                        }

                                        invdata.Invoice_data_Received_Date = Invoice_data_Received_Date;
                                        invdata.TOTAL_Invoice = TOTAL_Invoice;
                                        invdata.stepdate = stepdate;
                                        invdata.PHyInvRec = PHyInvRec1;
                                        invdata.Order_Rec = Order_Rec1;
                                        invdata.Payment_Recev = Payment_Recev1;
                                        inv.Add(invdata);

                                    }
                                }
                                else
                                {
                                    //return NotFound();
                                }
                            }
                        }
                        else
                        {
                            TempData["alertMessage"] = "Record Not Found.";
                            return View("Index");
                        }

                        _logger.LogInformation("Executed successfully" + " - AdditionalMISControlPoint;Show");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AdditionalMISControlPoint;Show");
                }

                return View(inv);
            }
        }

        public IActionResult ExportExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                var DetailsList = inv.ToList();
                DataTable Details = DetailsList.ToDataTable();
                Details.TableName = "Sheet1";
                try
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(Details);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            _logger.LogInformation("Data exported successfully" + " - AdditionalMISControlPoint;ExportExcel");
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdditionalMISReport.xlsx");
                        }
                    }

                   
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AdditionalMISControlPoint;ExportExcel");
                }
                return View();
            }
        }
    }
}

using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
using System.Web;
using System.Text;

namespace HDFCMSILWebMVC.Controllers
{
    public class PendingReportController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public Boolean genrpt = false;
        public PendingReportController(ILogger<PendingReportController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        
        //public PendingReportController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        public IActionResult ShowPendingReport()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
            
        }

        [HttpPost]
        public IActionResult ShowPendingReport(ShowPending_Report shopend)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                genrpt = false;
                DataTable dt = new DataTable();
                var stream1 = new MemoryStream();
                string OutFileName = String.Empty;
                string FileLastPart = String.Empty;


                try
                {

                    String strTempFolder = String.Empty;
                    if (shopend.ReportType == null || shopend.ReportType == "" || shopend.ReportType == "0")
                    {
                        { TempData["shortMessage"] = "Please Select Report Type"; return View("ShowPendingReport", shopend); }
                    }
                    if ((shopend.MISReportType == null || shopend.MISReportType == "" || shopend.MISReportType == "Select MIS Report Type" || shopend.MISReportType == "0") && (shopend.ReportType == "MIS1"))
                    {
                        { TempData["shortMessage"] = "Please Select MIS Report Type"; return View("ShowPendingReport", shopend); }
                    }

                    if (shopend.ReportType.ToString().ToUpper() == "Invoice Level".ToString().ToUpper())
                    {
                        //InvoiceLevel(shopend.StartDate, shopend.EndDate);

                        using (var db = new Entities.DatabaseContext())
                        {
                            var todate = shopend.EndDate;
                            var fromdate = shopend.StartDate;
                            var Inv = db.Set<PendingRet_InvoiceLevel>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=1 ").ToList();

                            if (Inv.Count > 0)
                            {
                                //  dataGridView1.DataSource = Inv;
                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "INVOICE_LEVEL_DATA." + FileLastPart + ".Csv";
                                using (var sw = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {

                                    sw.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment Status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        sw.WriteLine(rec.InvoiceNo + ";" + rec.InvoiceAmount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                    }
                                    sw.Close();
                                    genrpt = true;
                                }

                            }
                            else
                            {

                            }
                        }

                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }

                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "Pending Order".ToString().ToUpper())
                    {
                        //PendingOrder_cur(shopend.StartDate, shopend.EndDate, shopend.chkSelectDate);

                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        Boolean chk_InvoiceDate = shopend.chkSelectDate;
                        List<PendingRet_PendingOrder> Inv;
                        using (var db = new Entities.DatabaseContext())
                        {
                            if (chk_InvoiceDate == true)
                            {
                                Inv = db.Set<PendingRet_PendingOrder>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=2 ").ToList();
                            }
                            else
                            {
                                Inv = db.Set<PendingRet_PendingOrder>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='',@From_date='',@Flag=2 ").ToList();
                            }

                            if (Inv.Count > 0)
                            {
                                //  dataGridView1.DataSource = Inv;

                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "PENDING_ORDER_DATA." + FileLastPart + ".Csv";
                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {
                                    outputFile.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.StepDate + ";" + rec.IMEX_DEAL_NUMBER + ";" + rec.Dealer_Code + ";" + rec.Dealer_Name + ";" + rec.Dealer_Address4);
                                    }
                                    outputFile.Close();
                                    genrpt = true;
                                }

                                //MessageBox.Show("Invoice File downloaded Successfully.", "PendingOrder", MessageBoxButtons.OK);

                            }
                            else
                            {
                                //MessageBox.Show("Invoice File Record not found.", "PendingOrder", MessageBoxButtons.OK);
                            }

                        }


                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "Pending Payment".ToString().ToUpper())
                    {
                        //PendingPayment_cur(shopend.StartDate, shopend.EndDate, shopend.chkSelectDate);

                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        Boolean chk_InvoiceDate = shopend.chkSelectDate;
                        List<PendingRet_PendingPayment> Inv;
                        using (var db = new Entities.DatabaseContext())
                        {
                            if (chk_InvoiceDate == true)
                            {
                                Inv = db.Set<PendingRet_PendingPayment>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=3 ").ToList();
                            }
                            else
                            {
                                Inv = db.Set<PendingRet_PendingPayment>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='',@From_date='',@Flag=3 ").ToList();
                            }
                            if (Inv.Count > 0)
                            {
                                //    dataGridView1.DataSource = Inv;
                                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "PENDING_PAYMENT_DATA." + FileLastPart + ".Csv";
                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {
                                    outputFile.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.StepDate + ";" + rec.IMEX_DEAL_NUMBER + ";" + rec.OrderUpdationStatus + ";" + rec.Dealer_Code + ";" + rec.Dealer_Name + ";" + rec.Dealer_Address4);
                                    }
                                    outputFile.Close();
                                    genrpt = true;
                                }

                                //MessageBox.Show("Pending Payment File downloaded Successfully.", "PendingPayment", MessageBoxButtons.OK);

                            }
                            else
                            {
                                //MessageBox.Show("Pending Invoice File Record not found.", "PendingPayment", MessageBoxButtons.OK);
                            }
                        }

                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "Financer Name".ToString().ToUpper())
                    {
                        //FinancerName(shopend.StartDate, shopend.EndDate);

                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        Boolean chk_InvoiceDate = shopend.chkSelectDate;
                        List<PendingRet_FinancerName> Inv;
                        using (var db = new Entities.DatabaseContext())
                        {
                            Inv = db.Set<PendingRet_FinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=4 ").ToList();

                            if (Inv.Count > 0)
                            {
                                // dataGridView1.DataSource = Inv;

                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "FINANCER_LEVEL_DATA." + FileLastPart + ".Csv";
                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {
                                    outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                    }
                                    outputFile.Close();
                                    genrpt = true;
                                }

                                //MessageBox.Show("FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);

                            }
                            else
                            {
                                //MessageBox.Show("FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                            }
                        }


                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "Non Financer Name".ToString().ToUpper())
                    {
                        //NoNFinancerName(shopend.StartDate, shopend.EndDate);


                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        Boolean chk_InvoiceDate = shopend.chkSelectDate;
                        List<PendingRet_NONFinancerName> Inv;
                        using (var db = new Entities.DatabaseContext())
                        {
                            Inv = db.Set<PendingRet_NONFinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=5 ").ToList();

                            if (Inv.Count > 0)
                            {
                                //  dataGridView1.DataSource = Inv;

                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "NonFINANCER_LEVEL_DATA." + FileLastPart + ".Csv";

                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {
                                    outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                    }
                                    outputFile.Close();
                                    genrpt = true;
                                }

                                //MessageBox.Show("Non FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);

                            }
                            else
                            {
                                //MessageBox.Show("Non FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                            }
                        }


                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "Both Financer & Non Financer".ToString().ToUpper())
                    {
                        //BothFinancerName(shopend.StartDate, shopend.EndDate);

                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        Boolean chk_InvoiceDate = shopend.chkSelectDate;
                        List<PendingRet_BothFinancerName> Inv = null;
                        using (var db = new Entities.DatabaseContext())
                        {
                            //List<PendingRet_BothFinancerName> objBothFncAndNonFncList = new List<PendingRet_BothFinancerName>();
                            //objBothFncAndNonFncList = db.PendingRet_BothFinancerNames.FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();
                            Inv = db.Set<PendingRet_BothFinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();



                            if (Inv.Count > 0)
                            {
                                //    dataGridView1.DataSource = Inv;

                                FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                OutFileName = "All_Financer_NonFinancer_LEVEL_DATA." + FileLastPart + ".Csv";
                                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                {
                                    outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                                    foreach (var rec in Inv)
                                    {
                                        outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                    }
                                    outputFile.Close();
                                    genrpt = true;
                                }
                                //MessageBox.Show("All FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                            }
                            else
                            {
                                //MessageBox.Show("All FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                            }
                        }


                        if (genrpt == true)
                        {
                            // TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                        //var rpt = BothFinancerName(shopend.StartDate, shopend.EndDate).ToList();
                        //ViewBag.rpt = rpt;
                        //dt = ViewBag.rpt; 
                    }
                    else if (shopend.ReportType.ToString().ToUpper() == "MIS1".ToString().ToUpper())
                    {
                        if (shopend.MISReportType == "")
                        {
                            //MessageBox.Show("Please select MIS report type");
                            //CmbMISRpt.Focus();
                            //return;
                        }
                        //MSI1(shopend.StartDate, shopend.EndDate, shopend.MISReportType);

                        var todate = shopend.EndDate;
                        var fromdate = shopend.StartDate;
                        string CmbMISRpt = shopend.MISReportType;
                        using (var db = new Entities.DatabaseContext())
                        {
                            if (CmbMISRpt.ToString().ToUpper() == "DO Cancellation And Retain Invoice Report".ToString().ToUpper())
                            {
                                List<PendingRet_DO_CancellationAndRetainInvoiceReport> Inv;
                                Inv = db.Set<PendingRet_DO_CancellationAndRetainInvoiceReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                                if (Inv.Count > 0)
                                {
                                    //  dataGridView1.DataSource = Inv;
                                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                                    //strTempFolder = reportPATH;
                                    FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                    OutFileName = "MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report." + FileLastPart + ".Csv";
                                    //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                                    using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                    {
                                        outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Dealer Code;Dealer Name;Location");

                                        foreach (var rec in Inv)
                                        {
                                            outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                        }
                                        outputFile.Close();
                                        genrpt = true;
                                    }
                                    //MessageBox.Show("MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    //  MessageBox.Show("MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                                }
                            }
                            else if (CmbMISRpt.ToString().ToUpper() == "DO And Invoice Cancellation Report".ToString().ToUpper())
                            {
                                List<PendingRet_DOAndInvoiceCancellationReport> Inv;
                                Inv = db.Set<PendingRet_DOAndInvoiceCancellationReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                                if (Inv.Count > 0)
                                {
                                    //   dataGridView1.DataSource = Inv;
                                    //strTempFolder = reportPATH;
                                    FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                    OutFileName = "MIS1_LEVEL_DATA-DO And Invoice Cancellation Report." + FileLastPart + ".Csv";
                                    using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                    {
                                        outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Delivery Order No;Dealer Code;Dealer Name;Location");

                                        foreach (var rec in Inv)
                                        {
                                            outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                        }
                                        outputFile.Close();
                                    }
                                    // MessageBox.Show("MIS1_LEVEL_DATA-DO And Invoice Cancellation Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    // MessageBox.Show("MIS1_LEVEL_DATA-DO And Invoice Cancellation Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                                }
                            }
                            else if (CmbMISRpt.ToString().ToUpper() == "Invoice Cancellation Report".ToString().ToUpper())
                            {

                                List<PendingRet_InvoiceCancellationReport> Inv;
                                Inv = db.Set<PendingRet_InvoiceCancellationReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                                if (Inv.Count > 0)
                                {

                                    //    dataGridView1.DataSource = Inv;
                                    //strTempFolder = reportPATH;
                                    FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                    OutFileName = "MIS1_LEVEL_DATA-Invoice Cancellation Report." + FileLastPart + ".Csv";
                                    using (var outputFile = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                                    {
                                        outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Delivery Order No;Dealer Code;Dealer Name;Location");

                                        foreach (var rec in Inv)
                                        {
                                            outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                        }
                                        outputFile.Close();
                                    }
                                    //  MessageBox.Show("MIS1_LEVEL_DATA-Invoice Cancellation Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    //   MessageBox.Show("MIS1_LEVEL_DATA-Invoice Cancellation Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                                }
                            }
                        }


                        if (genrpt == true)
                        {
                            //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                            TempData["shortMessage"] = "Download Successfully";
                        }
                        else
                        {
                            TempData["shortMessage"] = "No Record Found";
                        }
                    }

                    _logger.LogInformation("Executed successfully" + " - PendingReportController;ShowTablePendingReport");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - PendingReportController;ShowTablePendingReport");
                }


                //return View();
                //return RedirectToAction("ShowPendingReport");


                if (genrpt == true)
                {
                    //TempData["shortMessage"] = "Download Successfully at Path " + _configuration.GetValue<string>("MSILSettings:Report");
                    TempData["shortMessage"] = "Download Successfully";
                    return File(stream1.ToArray(), "text/csv", OutFileName);
                }
                else
                {
                    TempData["shortMessage"] = "No Record Found";
                    return RedirectToAction("ShowPendingReport");
                }
                //return File(stream1.ToArray(), "text/csv", OutFileName);
            }
        }

        public void InvoiceLevel(DateTime? datetime_From, DateTime? datetimeTo)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            StringBuilder sb = new StringBuilder();
            var stream1 = new MemoryStream();
            
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                _logger.LogInformation("Log 1");
                var todate = datetimeTo;
                var fromdate = datetime_From;
                _logger.LogInformation(" Dt1 " + fromdate + " Dt2 " + todate);
                //strTempFolder = @"D:\Office\HDFC MSIL\";
                using (var db = new Entities.DatabaseContext())
                {
                    _logger.LogInformation("Log 2");
                    var Inv = db.Set<PendingRet_InvoiceLevel>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=1 ").ToList();
                    _logger.LogInformation("Log 3");
                    if (Inv.Count > 0)
                    {
                        //  dataGridView1.DataSource = Inv;
                        _logger.LogInformation("Count " + Inv.Count);



                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "INVOICE_LEVEL_DATA." + FileLastPart + ".Csv";

                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        //using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        //using (var sw = new StreamWriter(stream: stream1, encoding: new UTF8Encoding(true)))
                        {

                            outputFile.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment Status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.InvoiceNo + ";" + rec.InvoiceAmount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }

                        //MessageBox.Show("Invoice File downloaded Successfully.", "InvoiceLevel", MessageBoxButtons.OK);
                    }
                    else
                    {
                        //MessageBox.Show("Invoice File Record not found.", "InvoiceLevel", MessageBoxButtons.OK);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;InvoiceLevel");
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "InvoiceLevel");
            }

            //stream1.Position = 0;
            //return File(stream1.ToArray(), "text/csv", OutFileName);
           
        }

        private Encoding newUTF8Encoding(bool v)
        {
            throw new NotImplementedException();
        }

        private void PendingOrder_cur(DateTime?  datetime_From, DateTime? datetimeTo, bool chk_InvoiceDate)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                var todate = datetimeTo;
                var fromdate = datetime_From;
                //strTempFolder = @"D:\Office\HDFC MSIL\";
                List<PendingRet_PendingOrder> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    if (chk_InvoiceDate == true)
                    {
                        Inv = db.Set<PendingRet_PendingOrder>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=2 ").ToList();
                    }
                    else
                    {
                        Inv = db.Set<PendingRet_PendingOrder>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='',@From_date='',@Flag=2 ").ToList();
                    }

                    if (Inv.Count > 0)
                    {
                        //  dataGridView1.DataSource = Inv;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "PENDING_ORDER_DATA." + FileLastPart + ".Csv";
                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        {
                            outputFile.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.StepDate + ";" + rec.IMEX_DEAL_NUMBER + ";" + rec.Dealer_Code + ";" + rec.Dealer_Name + ";" + rec.Dealer_Address4);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }

                        //MessageBox.Show("Invoice File downloaded Successfully.", "PendingOrder", MessageBoxButtons.OK);

                    }
                    else
                    {
                        //MessageBox.Show("Invoice File Record not found.", "PendingOrder", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;PendingOrder_cur");
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "PendingOrder_cur()");
            }
        }

        private void PendingPayment_cur(DateTime? datetimeTo, DateTime? datetime_From, bool chk_InvoiceDate)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            try
            {

                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                var todate = datetimeTo.Value;
                var fromdate = datetime_From;
                //strTempFolder = @"D:\Office\HDFC MSIL\";
                List<PendingRet_PendingPayment> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    if (chk_InvoiceDate == true)
                    {
                        Inv = db.Set<PendingRet_PendingPayment>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=3 ").ToList();
                    }
                    else
                    {
                        Inv = db.Set<PendingRet_PendingPayment>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='',@From_date='',@Flag=3 ").ToList();
                    }
                    if (Inv.Count > 0)
                    {
                        //    dataGridView1.DataSource = Inv;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "PENDING_PAYMENT_DATA." + FileLastPart + ".Csv";
                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        {
                            outputFile.WriteLine("Invoice No;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.StepDate + ";" + rec.IMEX_DEAL_NUMBER + ";" + rec.OrderUpdationStatus + ";" + rec.Dealer_Code + ";" + rec.Dealer_Name + ";" + rec.Dealer_Address4);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }

                        //MessageBox.Show("Pending Payment File downloaded Successfully.", "PendingPayment", MessageBoxButtons.OK);

                    }
                    else
                    {
                        //MessageBox.Show("Pending Invoice File Record not found.", "PendingPayment", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;PendingPayment_cur");
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "PendingPayment_cur()");
            }
        }


        private void FinancerName(DateTime? datetimeTo, DateTime? datetime_From)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                var todate = datetimeTo;
                var fromdate = datetime_From;
                List<PendingRet_FinancerName> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    Inv = db.Set<PendingRet_FinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=4 ").ToList();

                    if (Inv.Count > 0)
                    {
                        // dataGridView1.DataSource = Inv;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "FINANCER_LEVEL_DATA." + FileLastPart + ".Csv";
                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        {
                            outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }

                        //MessageBox.Show("FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);

                    }
                    else
                    {
                        //MessageBox.Show("FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;FinancerName");
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "FinancerName()");
            }
        }

        private void NoNFinancerName(DateTime? datetimeTo, DateTime? datetime_From)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                var todate = datetimeTo;
                var fromdate = datetime_From;
                List<PendingRet_NONFinancerName> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    Inv = db.Set<PendingRet_NONFinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=5 ").ToList();
                    
                    if (Inv.Count > 0)
                    {
                        //  dataGridView1.DataSource = Inv;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "NonFINANCER_LEVEL_DATA." + FileLastPart + ".Csv";

                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        {
                            outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }

                        //MessageBox.Show("Non FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);

                    }
                    else
                    {
                        //MessageBox.Show("Non FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;NoNFinancerName");
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "NoNFinancerName()");
            }
        }




        private void BothFinancerName(DateTime? datetime_From, DateTime? datetimeTo)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            List<PendingRet_BothFinancerName> Inv = null;
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);

                var todate = datetimeTo;
                var fromdate = datetime_From;
                //List<PendingRet_BothFinancerName> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    //List<PendingRet_BothFinancerName> objBothFncAndNonFncList = new List<PendingRet_BothFinancerName>();
                    //objBothFncAndNonFncList = db.PendingRet_BothFinancerNames.FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();
                    Inv = db.Set<PendingRet_BothFinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();
                    
                    

                    if (Inv.Count > 0)
                    {
                        //    dataGridView1.DataSource = Inv;
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "All_Financer_NonFinancer_LEVEL_DATA." + FileLastPart + ".Csv";
                        //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                        {
                            outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                            }
                            outputFile.Close();
                            genrpt = true;
                        }
                        //MessageBox.Show("All FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                    }
                    else
                    {
                        //MessageBox.Show("All FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "BothFinancerName()");
                _logger.LogError(ex.ToString() + " - PendingReportController;BothFinancerName");
            }
            
        }

        private List<PendingRet_BothFinancerName> BothFinancerNameOld(DateTime? datetime_From, DateTime? datetimeTo)
        {
            String strTempFolder = String.Empty;
            string FileLastPart = String.Empty;
            string OutFileName = String.Empty;
            List<PendingRet_BothFinancerName> Inv = null;
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);

                var todate = datetimeTo;
                var fromdate = datetime_From;
                //List<PendingRet_BothFinancerName> Inv;
                using (var db = new Entities.DatabaseContext())
                {
                    //List<PendingRet_BothFinancerName> objBothFncAndNonFncList = new List<PendingRet_BothFinancerName>();
                    //objBothFncAndNonFncList = db.PendingRet_BothFinancerNames.FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();
                    Inv = db.Set<PendingRet_BothFinancerName>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='',@Flag=6 ").ToList();
                    List<PendingRet_NONFinancerName> PendRpt = new List<PendingRet_NONFinancerName>();
                    ViewData.Model = Inv;

                    if (Inv.Count > 0)
                    {
                        //    dataGridView1.DataSource = Inv;
                        strTempFolder = reportPATH;
                        FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        OutFileName = "All_Financer_NonFinancer_LEVEL_DATA." + FileLastPart + ".Csv";
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                        {
                            outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Payment_status;Payment Receipt Mode;Payment Receipt Date;Payment Reference No;Financed_NotFinanced;Financier Code;Financer Name;Dealer Code;Dealer Name;Location");

                            foreach (var rec in Inv)
                            {
                                outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.Payment_status + ";" + rec.PaymentReceiptMode + ";" + rec.PaymentReceiptDate + ";" + rec.PaymentReferenceNo + ";" + rec.Financed_NotFinanced + ";" + rec.FinancierCode + ";" + rec.FinancerName + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                            }
                            outputFile.Close();
                        }
                        //MessageBox.Show("All FinancerName Data File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                    }
                    else
                    {
                        //MessageBox.Show("All FinancerName data Record not found.", "FinancerName", MessageBoxButtons.OK);
                    }
                }

            }
            catch (Exception ex)
            {
                //clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "BothFinancerName()");
                _logger.LogError(ex.ToString() + " - PendingReportController;BothFinancerName");
            }
            return Inv;
        }

        

        private void MSI1(DateTime? datetimeTo, DateTime? datetime_From,string CmbMISRpt)
        {
            try
            {
                var reportPATH = _configuration.GetValue<string>("MSILSettings:Report");
                if (!Directory.Exists(reportPATH))
                    Directory.CreateDirectory(reportPATH);
                String strTempFolder = String.Empty;
                string FileLastPart = String.Empty;
                string OutFileName = String.Empty;
                var todate = datetimeTo;
                var fromdate = datetime_From;
                using (var db = new Entities.DatabaseContext())
                {
                    if (CmbMISRpt.ToString().ToUpper() == "DO Cancellation And Retain Invoice Report".ToString().ToUpper())
                    {
                        List<PendingRet_DO_CancellationAndRetainInvoiceReport> Inv;
                        Inv = db.Set<PendingRet_DO_CancellationAndRetainInvoiceReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                        if (Inv.Count > 0)
                        {
                            //  dataGridView1.DataSource = Inv;
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                            strTempFolder = reportPATH;
                            FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                            OutFileName = "MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report." + FileLastPart + ".Csv";
                            //using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                            using (StreamWriter outputFile = new StreamWriter(path + "\\" + OutFileName, true))
                            {
                                outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Order Updation Status;Delivery Order No;Dealer Code;Dealer Name;Location");

                                foreach (var rec in Inv)
                                {
                                    outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.OrderUpdationStatus + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                }
                                outputFile.Close();
                                genrpt = true;
                            }
                            //MessageBox.Show("MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                        }
                        else
                        {
                          //  MessageBox.Show("MIS1_LEVEL_DATA-DO Cancellation And Retain Invoice Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                        }
                    }
                    else if (CmbMISRpt.ToString().ToUpper() == "DO And Invoice Cancellation Report".ToString().ToUpper())
                    {
                        List<PendingRet_DOAndInvoiceCancellationReport> Inv;
                        Inv = db.Set<PendingRet_DOAndInvoiceCancellationReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                        if (Inv.Count > 0)
                        {
                            //   dataGridView1.DataSource = Inv;
                            strTempFolder = reportPATH;
                            FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                            OutFileName = "MIS1_LEVEL_DATA-DO And Invoice Cancellation Report." + FileLastPart + ".Csv";
                            using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                            {
                                outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Delivery Order No;Dealer Code;Dealer Name;Location");

                                foreach (var rec in Inv)
                                {
                                    outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                }
                                outputFile.Close();
                            }
                           // MessageBox.Show("MIS1_LEVEL_DATA-DO And Invoice Cancellation Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                        }
                        else
                        {
                           // MessageBox.Show("MIS1_LEVEL_DATA-DO And Invoice Cancellation Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                        }
                    }
                    else if (CmbMISRpt.ToString().ToUpper() == "Invoice Cancellation Report".ToString().ToUpper())
                    {

                        List<PendingRet_InvoiceCancellationReport> Inv;
                        Inv = db.Set<PendingRet_InvoiceCancellationReport>().FromSqlRaw("EXEC uspFrm_PendingReport @To_date='" + todate + "',@From_date='" + fromdate + "',@MISReport='" + CmbMISRpt.ToString().ToUpper() + "',@Flag=5 ").ToList();
                        if (Inv.Count > 0)
                        {

                            //    dataGridView1.DataSource = Inv;
                            strTempFolder = reportPATH;
                            FileLastPart = DateTime.Now.ToString("ddMMyyyyHHmmss");
                            OutFileName = "MIS1_LEVEL_DATA-Invoice Cancellation Report." + FileLastPart + ".Csv";
                            using (StreamWriter outputFile = new StreamWriter(Path.Combine(strTempFolder, OutFileName), true))
                            {
                                outputFile.WriteLine("Invoice Number;Invoice Amount;Physically Receipt Date;Trade Reference No;Delivery Order No;Dealer Code;Dealer Name;Location");

                                foreach (var rec in Inv)
                                {
                                    outputFile.WriteLine(rec.Invoice_Number + ";" + rec.Invoice_Amount + ";" + rec.PhysicallyReceiptDate + ";" + rec.TradeReferenceNo + ";" + rec.DeliveryOrderNo + ";" + rec.DealerCode + ";" + rec.DealerName + ";" + rec.Location);
                                }
                                outputFile.Close();
                            }
                            //  MessageBox.Show("MIS1_LEVEL_DATA-Invoice Cancellation Report File downloaded Successfully..", "FinancerName", MessageBoxButtons.OK);
                        }
                        else
                        {
                            //   MessageBox.Show("MIS1_LEVEL_DATA-Invoice Cancellation Report data Record not found.", "FinancerName", MessageBoxButtons.OK);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - PendingReportController;MSI1");
                //  clserr.WriteErrorToTxtFile(ex.Message, "PendingReport", "MSI1()");
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

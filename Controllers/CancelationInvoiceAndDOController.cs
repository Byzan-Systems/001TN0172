using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Controllers
{
    public class CancelationInvoiceAndDOController : Controller
    {
        public static IList<cancellationInvDORetainInv> inv = new List<cancellationInvDORetainInv>();
        public  static IList<cancellationInvoicesonly> inv1 = new List<cancellationInvoicesonly>();
        public  static IList<cancellationoderNoInv> inv2 = new List<cancellationoderNoInv>();

        public static IList<Order_Invoice_Order_DesForCanfrm> ordinv = new List<Order_Invoice_Order_DesForCanfrm>();

        public static DataTable dt = new DataTable();


        public static ShowCancelationInvoiceAndDO SCID = new ShowCancelationInvoiceAndDO();

        private readonly ILogger _logger;
        public CancelationInvoiceAndDOController(ILogger<CancelationInvoiceAndDOController> logger)
        {
            _logger = logger;
        }

        public IActionResult ShowCancelationInvoiceAndDO()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }

 


        public IActionResult Show(ShowCancelationInvoiceAndDO DInvDa)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                //  DataTable dt = new DataTable();
                SCID = DInvDa;

                if (DInvDa == null)
                {
                    return View("Show");
                }
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {

                        if (DInvDa.chkReportType == true)
                        {

                            if (DInvDa.RerportType == "Cancel DO and Retain Invoices")
                            {

                                //string   Fromdate = DateTime.ParseExact(DInvDa.DateTo.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                //string Todate = DateTime.ParseExact(DInvDa.DateFrom.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                var Fromdate = DInvDa.DateFrom;
                                var Todate = DInvDa.DateTo;
                                inv = db.Set<cancellationInvDORetainInv>().FromSqlRaw("EXEC uspcancellationInvoiceDO @ToOrder_date = {0}, @FromOrder_date = {1}, @Flag = {2}", Todate, Fromdate, 1).ToList();

                                //inv = db.Set<cancellationInvDORetainInv>().FromSqlRaw("EXEC uspcancellationInvoiceDO @ToOrder_date ='" + Todate + "',@FromOrder_date='" + Fromdate + "',@Flag=1").ToList();

                                return View("ShowCancellationInvDORetain", inv);
                                //ListtoDataTable lsttodt = new ListtoDataTable();
                                //dt = lsttodt.ToDataTable(inv);

                            }
                            else if (DInvDa.RerportType == "Cancel Invoices only")
                            {
                                //string Fromdate = DateTime.ParseExact(DInvDa.DateTo.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                //string Todate = DateTime.ParseExact(DInvDa.DateFrom.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                var Fromdate = DInvDa.DateFrom; //DateTime.ParseExact(DInvDa.DateTo.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                var Todate = DInvDa.DateTo; //DateTime.ParseExact(DInvDa.DateFrom.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                inv1 = db.Set<cancellationInvoicesonly>().FromSqlInterpolated($"EXEC uspcancellationInvoiceDO @FromOrder_date={Fromdate}, @ToOrder_date={Todate}, @Flag=3").ToList();
                               // inv1 = db.Set<cancellationInvoicesonly>().FromSqlRaw("EXEC uspcancellationInvoiceDO  @FromOrder_date ='" + Fromdate + "',@ToOrder_date ='" + Todate + "',@Flag=3").ToList();


                                return View("ShowCancelInvoicesonly", inv1);

                                //ListtoDataTable lsttodt = new ListtoDataTable();
                                //dt = lsttodt.ToDataTable(inv1);
                            }
                            else if (DInvDa.RerportType == "Cancel Order number and Invoices")
                            {
                                //string Fromdate = DateTime.ParseExact(DInvDa.DateTo.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                //string Todate = DateTime.ParseExact(DInvDa.DateFrom.ToString().Substring(0, 10), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                                var Fromdate = DInvDa.DateFrom;
                                var Todate = DInvDa.DateTo;
                                inv2 = db.Set<cancellationoderNoInv>().FromSqlRaw("EXEC uspcancellationInvoiceDO @ToOrder_date ='" + Todate + "',@FromOrder_date='" + Fromdate + "',@Flag=2").ToList();




                                return View("ShowCancelOrdernumberandInvoices", inv2);

                                //ListtoDataTable lsttodt = new ListtoDataTable();
                                //dt = lsttodt.ToDataTable(inv2);
                            }
                        }

                    }



                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - Show cancellation invoice and DO");
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

        public IActionResult RecordsDeleteCancelDOandRetainInvoices(string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    var DetailsList = inv.ToList();
                    DataTable dtFilterData = new DataTable();
                    DataTable Details = DetailsList.ToDataTable();
                    using (var db = new Entities.DatabaseContext())
                    {
                        int i = 0;
                        dtFilterData = Details.Clone();
                        for (i = 0; i <= IsSelect.Length - 1; i++)
                        {
                            if (IsSelect[i].ToString() != "false")
                            {
                                string DO_number = IsSelect[i].ToString();
                                DataRow dtFilter = Details.Select("[DO_number]='" + DO_number + "'").FirstOrDefault();
                                dtFilterData.ImportRow(dtFilter);
                            }
                        }

                        foreach (DataRow row in dtFilterData.Rows)
                        {

                            var ord_desc = db.Set<Order_Des>().FromSqlRaw("select * from Order_Desc where DO_Number='" + row.ItemArray[1].ToString() + "' and Order_Status ='Payment Received'").ToList();

                            if (ord_desc.Count > 0)
                            {
                                TempData["alertMessage"] = "Payment received for above Records!!";
                                _logger.LogInformation("Payment received for above Records!!");


                            }
                            else
                            {
                                var ord_desc1 = db.Set<Order_Des>().FromSqlRaw("select * from Order_Desc where DO_Number='" + row.ItemArray[1].ToString() + "'").ToList();

                                if (ord_desc1.Count > 0)
                                {

                                    db.Database.ExecuteSqlRaw("INSERT INTO OrderBackup SELECT * FROM Order_Desc where (DO_Number ='" + row.ItemArray[1].ToString() + "')");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (order_number ='" + row.ItemArray[1].ToString() + "')");
                                    db.SaveChanges();
                                }
                                db.Database.ExecuteSqlRaw("Update Invoice set Order_Number='NULL',Order_Data_Received_Date='',Ord_Inv_ID='NULL',Invoice_Status='PHYSICAL INV REC' where order_number='" + row.ItemArray[1].ToString() + "'");
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("Update Do_desc set Delete_Flag=1, Deleted_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "' where DO_Number='" + row.ItemArray[1].ToString() + "'");
                                db.SaveChanges();

                                db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                                db.SaveChanges();

                                var stuffToRemove = inv.SingleOrDefault(s => s.DO_number == row["DO_Number"].ToString());

                                if (stuffToRemove != null)
                                {
                                    if (stuffToRemove.DO_number != "")
                                    {
                                        inv.Remove(stuffToRemove);
                                    }
                                    TempData["alertMessage"] = "Deleted record has been sent for authorization";
                                    _logger.LogInformation("Record has been Deleted successfully");
                                }

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - RecordsDeleteCancelDOandRetainInvoices");
                }
                return View("ShowCancellationInvDORetain", inv);
            }
        }


        public IActionResult RecordDeleteShowCancelInvoicesonly(string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {

                try
                {
                    var DetailsList = inv1.ToList();
                    DataTable dtFilterData = new DataTable();
                    DataTable Details = DetailsList.ToDataTable();
                    using (var db = new Entities.DatabaseContext())
                    {
                        int i = 0;
                        dtFilterData = Details.Clone();
                        for (i = 0; i <= IsSelect.Length - 1; i++)
                        {
                            if (IsSelect[i].ToString() != "false")
                            {
                                string Invoice_Number = IsSelect[i].ToString();
                                DataRow dtFilter = Details.Select("[Invoice_Number]='" + Invoice_Number + "'").FirstOrDefault();
                                dtFilterData.ImportRow(dtFilter);
                            }
                        }
                        foreach (DataRow row in dtFilterData.Rows)
                        {

                            var Inv_cal_desc = db.Set<invoice_cancel_desc>().FromSqlRaw("Select * from Invoice_cancel_desc where invoice_number='" + row[1].ToString() + "' and Reason IN ('Payment Received')").ToList();

                            if (Inv_cal_desc.Count > 0)
                            {
                                TempData["alertMessage"] = "Payment received for above Records!!";
                                _logger.LogInformation("Payment received for above Records!!");
                            }
                            else
                            {
                                db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (invoice_number='" + row[1].ToString() + "')");
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("update invoice_cancel_desc set Cancelled_Flag=1, Deleted_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "' where Invoice_Number='" + row[1].ToString() + "'");
                                db.SaveChanges();

                                db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                                db.SaveChanges();

                                var stuffToRemove1 = inv1.SingleOrDefault(s => s.Invoice_Number == row["Invoice_Number"].ToString());

                                if (stuffToRemove1 != null)
                                {
                                    if (stuffToRemove1.Invoice_Number != "")
                                    {
                                        inv1.Remove(stuffToRemove1);
                                    }
                                    //TempData["alertMessage"] = "Deleted record has been sent for authorization";
                                    TempData["alertMessage"] = "Selected record has been sent for authorization successfully";
                                    _logger.LogInformation("Record has been Deleted successfully");
                                }



                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - RecordDeleteShowCancelInvoicesonly");
                }


                return View("ShowCancelInvoicesonly", inv1);
            }
        }

        public IActionResult RecordDeleteShowCancelOrdernumberandInvoices(string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    var DetailsList = inv2.ToList();
                    DataTable dtFilterData = new DataTable();
                    DataTable Details = DetailsList.ToDataTable();
                    using (var db = new Entities.DatabaseContext())
                    {
                        int i = 0;
                        dtFilterData = Details.Clone();
                        for (i = 0; i <= IsSelect.Length - 1; i++)
                        {
                            if (IsSelect[i].ToString() != "false")
                            {
                                string Invoice_Number = IsSelect[i].ToString();
                                DataRow dtFilter = Details.Select("[Invoice_Number]='" + Invoice_Number + "'").FirstOrDefault();
                                dtFilterData.ImportRow(dtFilter);
                            }
                        }

                        foreach (DataRow row in dtFilterData.Rows)
                        {
                            var ord_desc = db.Set<Order_Des>().FromSqlRaw("select * from Order_Desc where DO_Number='" + row.ItemArray[3].ToString() + "' and Order_Status ='Payment Received'").ToList();

                            if (ord_desc.Count > 0)
                            {
                                TempData["alertMessage"] = "Payment received for above Records!!";
                                _logger.LogInformation("Payment received for above Records!!");
                            }
                            else
                            {
                                var ordinv = db.Set<Order_Invoice_Order_DesForCanfrm>().FromSqlRaw("SELECT Order_Inv_Number,DO_Number FROM Order_Invoice INNER JOIN Order_Desc ON Order_Invoice.Order_ID = Order_Desc.Order_ID WHERE (Order_Invoice.Order_Inv_Status = 'Found') and order_desc.DO_Number='" + row.ItemArray[3].ToString() + "'").ToList();
                                if (ordinv.Count > 0) /////Correction 19-12-2023
                                {

                                    db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (Invoice_Number='" + ordinv[0].Order_Inv_Number + "')");
                                    db.SaveChanges();
                                    db.Database.ExecuteSqlRaw("INSERT INTO OrderBackup SELECT * FROM Order_Desc where (DO_Number ='" + ordinv[0].DO_Number + "')");
                                    db.SaveChanges();

                                    db.Database.ExecuteSqlRaw("Update Do_desc set Delete_Flag=1, Deleted_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "' where DO_Number='" + ordinv[0].DO_Number + "'");
                                    db.SaveChanges();

                                    db.Database.ExecuteSqlRaw("Update invoice_cancel_desc set Cancelled_Flag=1, Deleted_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "' where DO_Number='" + ordinv[0].DO_Number + "'");
                                    db.SaveChanges();

                                    db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                                    db.SaveChanges();
                                    var stuffToRemove = inv2.SingleOrDefault(s => s.Invoice_Number == row["Invoice_Number"].ToString());

                                    if (stuffToRemove != null)
                                    {
                                        if (stuffToRemove.Invoice_Number != "")
                                        {
                                            inv2.Remove(stuffToRemove);
                                        }
                                        //TempData["alertMessage"] = "Deleted record has been sent for authorization";
                                        TempData["alertMessage"] = "Selected record has been sent for authorization successfully";
                                        _logger.LogInformation("Record has been Deleted successfully");
                                    }
                                }
                                else
                                {
                                    db.Database.ExecuteSqlRaw("Update invoice_cancel_desc set Cancelled_Flag=1 where DO_Number='" + row.ItemArray[3].ToString() + "'");
                                    db.SaveChanges();



                                    var stuffToRemove = inv2.SingleOrDefault(s => s.Invoice_Number == row["Invoice_Number"].ToString());

                                    if (stuffToRemove != null)
                                    {
                                        if (stuffToRemove.Invoice_Number != "")
                                        {
                                            inv2.Remove(stuffToRemove);
                                        }
                                        //TempData["alertMessage"] = "Deleted record has been sent for authorization";
                                        TempData["alertMessage"] = "Selected record has been sent for authorization successfully";
                                        _logger.LogInformation("Record has been Deleted successfully");
                                    }
                                }
                            }
                        }
                        //TempData["alertMessage"] = "Record has been Deleted successfully";
                        //_logger.LogInformation("Record has been Deleted successfully");

                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - RecordDeleteShowCancelOrdernumberandInvoices");
                }
                return View("ShowCancelOrdernumberandInvoices", inv2);
            }
        }
    }


}

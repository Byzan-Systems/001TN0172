using ClosedXML.Excel;
using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HDFCMSILWebMVC.Controllers
{
    public class AuthorisationController : Controller
    {
        public static List<AuthorizInvDORetainInv> AuthorizDORetainList = new List<AuthorizInvDORetainInv>();
        public static List<AuthorizoderNoInv> AuthorizOrNoInvoicesList = new List<AuthorizoderNoInv>();
        public static List<AuthorizInvoicesonly> AuthorizInvoiceonlyList = new List<AuthorizInvoicesonly>();

        public List<Order_Invoice_Order_Des> ordinv = new List<Order_Invoice_Order_Des>();

        private readonly ILogger _logger;
        public AuthorisationController(ILogger<AuthorisationController> logger)
        {
            _logger = logger;
        }

        public IActionResult ShowAuthorisation()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
            
        }
        public IActionResult ShowTableAuthorisation(ShowAuthoriz shAuth)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {

                DataTable dt = new DataTable();

                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {

                        if (shAuth.Reporttype == "Cancel DO and Retain Invoices")
                        {
                            AuthorizDORetainList = db.Set<AuthorizInvDORetainInv>().FromSqlRaw("EXEC uspAuthorize @Flag=1").ToList();
                            //ListtoDataTable lsttodt = new ListtoDataTable();
                            //dt = lsttodt.ToDataTable(AuthorizDORetainList);

                            return View("ShowTableAuthorisation", AuthorizDORetainList);
                        }
                        else if (shAuth.Reporttype == "Cancel Order number and Invoices")
                        {
                            AuthorizOrNoInvoicesList.Clear();
                            AuthorizOrNoInvoicesList = db.Set<AuthorizoderNoInv>().FromSqlRaw("EXEC uspAuthorize @Flag=2").ToList();

                            //ListtoDataTable lsttodt = new ListtoDataTable();
                            //dt = lsttodt.ToDataTable(AuthorizOrNoInvoicesList);

                            return View("ShowAuthCancelOrdernumberandinvoices", AuthorizOrNoInvoicesList);
                        }
                        else if (shAuth.Reporttype == "Cancel Invoices only")
                        {
                            AuthorizInvoiceonlyList.Clear();
                            AuthorizInvoiceonlyList = db.Set<AuthorizInvoicesonly>().FromSqlRaw("EXEC uspAuthorize @Flag=3").ToList();
                            //ListtoDataTable lsttodt = new ListtoDataTable();
                            //dt = lsttodt.ToDataTable(AuthorizInvoiceonlyList);

                            return View("ShowAuthCancelInvoicesonly", AuthorizInvoiceonlyList);
                        }

                    }

                    _logger.LogInformation("Executed successfully" + " - AuthorisationController; ShowTableAuthorisation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - AuthorisationController;ShowTableAuthorisation");
                }

                return View(dt);
            }
        }
        public IActionResult DeleteCancel_DORetain_Invoices(IList<AuthorizInvDORetainInv> DInvDa, string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                XLWorkbook wb = new XLWorkbook();
                try
                {
                    //IEnumerable<object> DetailsList = DInvDa as IEnumerable<object>;
                    var DetailsList = AuthorizDORetainList.ToList();
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

                            var ord_desc1 = db.Set<Order_Des>().FromSqlRaw("select * from Order_Desc where DO_Number='" + row.ItemArray[1].ToString() + "'").ToList();

                            if (ord_desc1.Count > 0)
                            {

                                //    db.Database.ExecuteSqlRaw("INSERT INTO OrderBackup SELECT * FROM Order_Desc where (DO_Number ='" + row.ItemArray[1].ToString() + "')");
                                //    db.SaveChanges();
                                //    db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (order_number ='" + row.ItemArray[1].ToString() + "')");
                                //    db.SaveChanges();
                                db.Database.ExecuteSqlRaw("Delete from Order_Invoice where  (Order_ID ='" + ord_desc1[0].Order_ID.ToString() + "')");
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("Delete from Order_Desc where (DO_Number ='" + row.ItemArray[1].ToString() + "')");
                                db.SaveChanges();

                            }
                            db.Database.ExecuteSqlRaw("Update Invoice set Order_Number='NULL',Order_Data_Received_Date='',Ord_Inv_ID='NULL',Invoice_Status='PHYSICAL INV REC' where order_number='" + row.ItemArray[1].ToString() + "'");
                            db.SaveChanges();
                            db.Database.ExecuteSqlRaw("Update Do_desc set Authorize_Flag=1,Authorized_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "', DO_Updated_date='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "',reason='Deleted successfully' where DO_Number='" + row.ItemArray[1].ToString() + "'");
                            db.SaveChanges();

                            db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                            db.SaveChanges();

                            var stuffToRemove = AuthorizDORetainList.SingleOrDefault(s => s.DO_number == row["DO_Number"].ToString());

                            if (stuffToRemove != null)
                            {
                                if (stuffToRemove.DO_number != "")
                                {
                                    AuthorizDORetainList.Remove(stuffToRemove);
                                }
                                TempData["alertMessage"] = "Records has been Deleted successfully";
                                _logger.LogInformation("DO_Number ='" + row.ItemArray[1].ToString() + "' has been permanently Deleted.");
                            }

                        }
                        return View("ShowTableAuthorisation", AuthorizDORetainList);
                    }

                    //DataTable DT = dtFilterData.Copy();
                    //DataTable dtIncremented = new DataTable();
                    //DataColumn dc = new DataColumn("Sr.No");
                    //dc.AutoIncrement = true;
                    //dc.AutoIncrementSeed = 1;
                    //dc.AutoIncrementStep = 1;
                    //dc.DataType = typeof(Int32);
                    //dtIncremented.Columns.Add(dc);

                    //dtIncremented.BeginLoadData();

                    //DataTableReader dtReader = new DataTableReader(DT);
                    //dtIncremented.Load(dtReader);

                    //dtIncremented.EndLoadData();
                    //dtIncremented.Columns.RemoveAt(1);
                    //dtIncremented.Columns["DO_number"].ColumnName = "DO Number";
                    //dtIncremented.Columns["Do_Date"].ColumnName = "DO Date";
                    //dtIncremented.Columns["Dealer_Code"].ColumnName = "Dealer Code";
                    //dtIncremented.Columns["Dealer_Destination_Code"].ColumnName = "Dealer Destination Code";
                    //dtIncremented.Columns["Dealer_Outlet_Code"].ColumnName = "Dealer Outlet Code";
                    //dtIncremented.Columns["Order_Amount"].ColumnName = "Order Amount";
                    //dtIncremented.Columns["DONumber"].ColumnName = "DO Number";

                    //dtIncremented.TableName = "Sheet1";
                    //using (wb)
                    //{
                    //    wb.Worksheets.Add(dtIncremented);
                    //    using (MemoryStream stream = new MemoryStream())
                    //    {
                    //        wb.SaveAs(stream);

                    //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Authorize.xlsx");
                    //    }

                    //}
                }
                catch (Exception ex)
                { _logger.LogInformation(ex.Message); return View(); }
                finally
                {
                    wb.Dispose();

                }


                //var DetailsList = AuthorizDORetainList.ToList();
                //DataTable Details = DetailsList.ToDataTable();
                //Details.TableName = "Sheet1";
                //using (XLWorkbook wb = new XLWorkbook())
                //{
                //    wb.Worksheets.Add(Details);
                //    using (MemoryStream stream = new MemoryStream())
                //    {
                //        wb.SaveAs(stream);
                //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Authorize.xlsx");
                //    }
                //}

                return View();
            }
            
        }
        public IActionResult DeleteCancel_InvoicesOnly(IList<AuthorizInvDORetainInv> DInvDa, string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                XLWorkbook wb = new XLWorkbook();
                try
                {
                    //IEnumerable<object> DetailsList = DInvDa as IEnumerable<object>;
                    var DetailsList = AuthorizInvoiceonlyList.ToList();
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
                                string[] Invoice_NUM_ID = IsSelect[i].ToString().Split(',');
                                DataRow dtFilter = Details.Select("[Invoice_Number]='" + Invoice_NUM_ID[0] + "' and [Invoice_ID]='" + Invoice_NUM_ID[1] + "' ").FirstOrDefault();
                                dtFilterData.ImportRow(dtFilter);
                            }
                        }
                        foreach (DataRow row in dtFilterData.Rows)
                        {
                            //var ord_desc1 = db.Set<invoice>().FromSqlRaw("select * from Invoice where Invoice_Number='" + row.ItemArray[1].ToString() + "'  and [Invoice_ID]='" + row.ItemArray[4].ToString() + "'").ToList();
                            var ord_desc1 = db.Set<invoice>().FromSqlRaw("select * from Invoice where Invoice_Number='" + row.ItemArray[1].ToString() + "'").ToList();   /////Remove invoice id on confirmation with Rahul pawar on 03-01-2023
                            if (ord_desc1.Count > 0)
                            {
                                //    db.Database.ExecuteSqlRaw("INSERT INTO OrderBackup SELECT * FROM Order_Desc where (DO_Number ='" + row.ItemArray[1].ToString() + "')");
                                //    db.SaveChanges();
                                //    db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (order_number ='" + row.ItemArray[1].ToString() + "')");
                                //    db.SaveChanges();
                                db.Database.ExecuteSqlRaw("Delete from Invoice where Invoice_Number ='" + ord_desc1[0].Invoice_Number.ToString() + "'");
                                db.SaveChanges();
                                db.Database.ExecuteSqlRaw("Delete from Invoice_Received where Invoice_Number='" + row.ItemArray[1].ToString() + "'");
                                db.SaveChanges();
                            }
                            db.Database.ExecuteSqlRaw("Update invoice_cancel_desc set Authorize_Flag=1,Authorized_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "', Updated_Date='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "',reason='Invoice deleted successfully' where Invoice_Number='" + row.ItemArray[1].ToString() + "' and Invoice_ID='" + row.ItemArray[4].ToString() + "'");
                            db.SaveChanges();

                            db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                            db.SaveChanges();
                            var stuffToRemove = AuthorizInvoiceonlyList.Where(s => s.InvoiceNumber == row["Invoice_Number"].ToString() && s.Invoice_ID == row["Invoice_ID"].ToString()).ToList();

                            if (stuffToRemove != null)
                            {
                                if (stuffToRemove[0].InvoiceNumber != "")
                                {
                                    AuthorizInvoiceonlyList.Remove(stuffToRemove[0]);
                                }
                                TempData["alertMessage"] = "Records has been Deleted successfully";
                                _logger.LogInformation("Invoice Number ='" + row.ItemArray[1].ToString() + "' has been permanently Deleted.");
                            }

                        }
                        return View("ShowAuthCancelInvoicesonly", AuthorizInvoiceonlyList);
                    }

                }
                catch (Exception ex)
                { _logger.LogError(ex.Message); 
                    return View(); }
                finally
                {
                    wb.Dispose();

                }
                return View();
            }
            
        }
        public IActionResult DeleteCancel_OrderNumber_Invoices(string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {

                try
                {
                    var DetailsList = AuthorizOrNoInvoicesList.ToList();
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
                                string[] Invoice_NUM_ID = IsSelect[i].ToString().Split(',');
                                DataRow dtFilter = Details.Select("[Invoice_Number]='" + Invoice_NUM_ID[0] + "'  and [Invoice_ID]='" + Invoice_NUM_ID[1] + "'  and [DO_Number]='" + Invoice_NUM_ID[2] + "' ").FirstOrDefault();
                                dtFilterData.ImportRow(dtFilter);
                            }
                        }

                        foreach (DataRow row in dtFilterData.Rows)
                        {

                            ordinv = db.Set<Order_Invoice_Order_Des>().FromSqlRaw("SELECT Order_Invoice.*,Order_Desc.Cash_Ops_ID,Order_Desc.DO_Number,Order_Date ,Do_Date,Financier_Code,Financier_Name,Email_IDs,Order_Amount,Order_Status,Order_Data_Received_On,Ord_Rej_Flag,Ord_Rej_Reason,Order_Desc.Dealer_Code,Order_Desc.Dealer_Destination_Code ,Order_Desc.Dealer_Outlet_Code FROM Order_Invoice INNER JOIN Order_Desc ON Order_Invoice.Order_ID = Order_Desc.Order_ID WHERE (Order_Invoice.Order_Inv_Status = 'Found') and order_desc.DO_Number='" + row.ItemArray[1].ToString() + "'").ToList();
                            if (ordinv.Count > 0)
                            {
                                //    db.Database.ExecuteSqlRaw("INSERT INTO InvoiceBackup SELECT * FROM Invoice where (Invoice_Number='" + ordinv[0].Order_Inv_Number + "')");
                                //    db.SaveChanges();
                                //    db.Database.ExecuteSqlRaw("INSERT INTO OrderBackup SELECT * FROM Order_Desc where (DO_Number ='" + ordinv[0].DO_Number + "')");
                                //    db.SaveChanges();

                                //db.Database.ExecuteSqlRaw("Delete from Order_Invoice where Order_Inv_Number ='" + ordinv[0].Order_Inv_Number + "' and Order_Inv_Number ='" + ordinv[0].Order_Inv_Number + "'");
                                for (int a = 0; a <= ordinv.Count - 1; a++) ////for loop Added on 27-12-2023
                                {

                                    //db.Database.ExecuteSqlRaw("Delete from  Invoice where Invoice_Number ='" + ordinv[a].Order_Inv_Number + "' and Invoice_ID ='" + ordinv[a].Ord_Inv_ID + "'");
                                    db.Database.ExecuteSqlRaw("Delete from  Invoice where Invoice_Number ='" + ordinv[a].Order_Inv_Number + "' and Order_Number ='" + ordinv[a].DO_Number + "'");  ////for loop Added on 29-12-2023
                                    db.SaveChanges();
                                    _logger.LogInformation("Delete from Invoice");

                                    db.Database.ExecuteSqlRaw("Delete from Order_Invoice where Order_Inv_Number ='" + ordinv[a].Order_Inv_Number + "'");
                                    db.SaveChanges();
                                    _logger.LogInformation("Delete from Order_Invoice");

                                    db.Database.ExecuteSqlRaw("Delete from Order_Desc  where DO_Number='" + ordinv[a].DO_Number + "' ");
                                    db.SaveChanges();
                                    _logger.LogInformation("Delete from Order_Desc");

                                    //db.Database.ExecuteSqlRaw("Delete from Invoice_Received where Invoice_Number='" + ordinv[0].DO_Number + "'");
                                    db.Database.ExecuteSqlRaw("Delete from Invoice_Received where Invoice_Number='" + ordinv[a].Order_Inv_Number + "'");
                                    db.SaveChanges();
                                    _logger.LogInformation("Delete from Invoice_Received");
                                }
                            }
                            db.Database.ExecuteSqlRaw("Update Do_desc set Authorize_Flag=1,Authorized_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "', DO_Updated_date='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "',reason='Order and Invoice deleted successfully' where DO_Number='" + row["DO_Number"].ToString() + "'");
                            db.SaveChanges();
                            _logger.LogInformation("Update Do_desc");

                            db.Database.ExecuteSqlRaw("Update invoice_cancel_desc set Authorize_Flag=1,Authorized_On='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "', Updated_Date='" + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "',reason='Order and Invoice deleted successfully' where DO_Number='" + row["DO_Number"].ToString() + "'");
                            db.SaveChanges();
                            _logger.LogInformation("Update invoice_cancel_desc");

                            db.Database.ExecuteSqlRaw("Update CancelDOIn_Reports set CancelDoInFlag=1"); /////Added on 10/01/2024
                            db.SaveChanges();

                            var stuffToRemove = AuthorizOrNoInvoicesList.SingleOrDefault(s => s.Invoice_Number == row["Invoice_Number"].ToString() && s.DO_Number == row["DO_Number"].ToString() && s.Invoice_ID == row["Invoice_ID"].ToString());

                            if (stuffToRemove != null)
                            {
                                if (stuffToRemove.Invoice_Number != "")
                                {
                                    AuthorizOrNoInvoicesList.Remove(stuffToRemove);
                                }
                                TempData["alertMessage"] = "Record has been Deleted successfully";
                                _logger.LogInformation("Invoice Number " + stuffToRemove.Invoice_Number + " has been Deleted successfully");

                            }

                        }
                        return View("ShowAuthCancelOrdernumberandinvoices", AuthorizOrNoInvoicesList);
                        //TempData["alertMessage"] = "Record has been Deleted successfully";
                        //_logger.LogInformation("Record has been Deleted successfully");

                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - RecordDeleteShowCancelOrdernumberandInvoices");
                }
            }
            

            return View();
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

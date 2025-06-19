using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Data;
using MoreLinq;
using ClosedXML.Excel;
using System.IO;
using System.Reflection;

namespace HDFCMSILWebMVC.Controllers
{
    public class GefuController : Controller
    {
       public  DataTable dt = new DataTable();
        private readonly ILogger _logger;
        public GefuController(ILogger<GefuController> logger)
        {
            _logger = logger;
        }
        private static List<Gefu> GefuLists = new List<Gefu>();
        public IActionResult ShowGefu()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }        
        public IActionResult Show(ShowGefuSelect gefu)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                //return View(GetGefuList(gefu.Cash_Ops_ID));
                return View(GetGefuList(gefu.SelectDate));
            }
        }

        //public List<Gefu> GetGefuList(DateTime geffu)
        public DataTable GetGefuList(DateTime geffu)
        {
            //List<Gefu> GefuLists = new List<Gefu>();
            
            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                  //var GefuLists = db.Set<Gefu>().FromSqlRaw("exec uspGefu @FromDate=''").ToList();
                 GefuLists = db.Set<Gefu>().FromSqlRaw("exec uspGefu @FromDate='" + geffu + "'").ToList();
                 ListtoDataTable lsttodt = new ListtoDataTable();
                 dt = lsttodt.ToDataTable(GefuLists);
                }

                _logger.LogInformation("Executed successfully" + " - GefuController;GetGefuList");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - GefuController;GetGefuList");
            }

            //return GefuLists;
            return dt;
        }        
        public IActionResult GenExcel()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    //var DetailsList = GefuLists.ToList();
                    //DataTable Details = DetailsList.ToDataTable();
                    ListtoDataTable lsttodt = new ListtoDataTable();
                    DataTable Details = lsttodt.ToDataTable(GefuLists);
                    Details.TableName = "Sheet1";

                    string ab = lsttodt.updaeGefu(Details);
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(Details);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            _logger.LogInformation("Data exported successfully" + " - PaymentReportController;ExportExcel");
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GefuReport.xlsx");
                        }
                     
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - GefuController;ExportExcel");
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

                //foreach (PropertyInfo prop in Props)
                //{
                //    //Setting column names as Property names  
                //    //dataTable.Columns.Add(prop.Name);
                //}

                dataTable.Columns.Add("Cash OpsID");
                dataTable.Columns.Add("Account No");
                dataTable.Columns.Add("DR/CR Number");
                dataTable.Columns.Add("DR/CR Status");
                dataTable.Columns.Add("Amount");
                dataTable.Columns.Add("UTR/VIRTUAL");
                dataTable.Columns.Add("VIRTUAL Account No");

                foreach (T item in items)
                {
                    var row1values = new object[7];
                    var row2values = new object[7];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        if (i ==0 )
                        { 
                        row1values[0] = Props[i].GetValue(item, null);
                        row2values[0] = Props[i].GetValue(item, null);
                        }
                        if (i == 1)
                        {
                            row1values[1] = Props[i].GetValue(item, null);
                        }
                        if (i == 2)
                        {
                            row2values[1] = Props[i].GetValue(item, null);
                        }
                        if (i == 3)
                        {
                            row1values[2] = Props[i].GetValue(item, null);
                        }
                        if (i == 4)
                        {
                            row2values[2] = Props[i].GetValue(item, null);
                        }
                        if (i == 5)
                        {
                            row1values[3] = Props[i].GetValue(item, null);
                        }
                        if (i == 6)
                        {
                            row2values[3] = Props[i].GetValue(item, null);
                        }
                        if (i == 7)
                        {
                            row1values[4] = Props[i].GetValue(item, null);
                            row2values[4] = Props[i].GetValue(item, null);
                        }
                        if (i == 8)
                        {
                            row1values[5] = Props[i].GetValue(item, null);
                            row2values[5] = Props[i].GetValue(item, null);
                        }
                        if (i == 9)
                        {
                            row1values[6] = Props[i].GetValue(item, null);
                            row2values[6] = Props[i].GetValue(item, null);
                        }


                    }
                    dataTable.Rows.Add(row1values);
                    dataTable.Rows.Add(row2values);
                }

                return dataTable;
            }

            public string updaeGefu(DataTable dt)
            {
                Entities.DatabaseContext db = new Entities.DatabaseContext();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[3].ToString()=="D")
                    {
                        //var inv = db.Set<Gefu>().FromSqlRaw("exec uspGefu @FromDate='', @gefuFlag='1',@GEFO_Date='" + DateTime.Now.ToString("yyyy-MM-dd")+ "',@DR_Ac_No= '" + dt.Rows[i].ItemArray[1].ToString() + "', @LoginID='',@Cash_Ops_ID='" + dt.Rows[i].ItemArray[0].ToString() + "',@CR_Ac_No='',@Task=updateCashOpsGefuForDebit");
                        db.Database.ExecuteSqlRaw("Update CashOps_Upload set GEFO_Flag=1,GEFO_Date='" + DateTime.Now.ToString("yyyy-MM-dd") + "',DR_Account_No='" + dt.Rows[i].ItemArray[1].ToString() + "',LoginID=''  where Cash_Ops_ID='" + dt.Rows[i].ItemArray[0].ToString() + "'");
                        db.SaveChanges();
                    }
                    else if (dt.Rows[i].ItemArray[3].ToString() == "C")
                    {
                        //var inv = db.Set<Gefu>().FromSqlRaw("exec uspGefu @FromDate='',@gefuFlag='',@GEFO_Date='',@DR_Ac_No='',@LoginID='',@Cash_Ops_ID='" + dt.Rows[i].ItemArray[0].ToString() + "', @CR_Ac_No='" + dt.Rows[i].ItemArray[1].ToString() + "',@Task=updateCashOpsGefuForCredit");
                        db.Database.ExecuteSqlRaw("Update CashOps_Upload set CR_Account_No='" + dt.Rows[i].ItemArray[1].ToString() + "' where Cash_Ops_ID='" + dt.Rows[i].ItemArray[0].ToString() + "'");
                        db.SaveChanges();
                    }
                }
                string msg = "UPDATE";
                return msg;
            }
        }
    }
}

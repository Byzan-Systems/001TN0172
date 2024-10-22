using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using System.IO;
using System.Data;
using MoreLinq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HDFCMSILWebMVC.Controllers
{
    public class DownloadInvoice : Controller
    {
        public static IList<DownloadFillInvoice> inv = new List<DownloadFillInvoice>();
        private readonly ILogger _logger;
        public DownloadInvoice(ILogger<DownloadInvoice> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();

        }
        public Boolean CheckForSpecial(string donumber)
        {
            for (int i = 0; i < donumber.Length; i++)
            {
                if (!(char.IsLetter(donumber[i])) && (!(char.IsNumber(donumber[i]))))
                    return false;
            }
            return true;
        }
        public IActionResult Show(DownloadinvFrmDeta DInvDa)
        {
            if (DInvDa == null)
            {
                _logger.LogError("Data Is Null return from model");   //log enhance by chaitrali 10/10/2024
                return View("Index");
            }
            try
            {

                _logger.LogError("values of filters ChkDate:" + DInvDa.ChkDate + "; ChkInvoiceNo: " + DInvDa.ChkInvoiceNo + " DateFrom: " + DInvDa.ChkReporttype+";"+DInvDa.DateFrom + " ; Invoice_Number: " + DInvDa.DateTo+""+DInvDa.Invoice_Number+ " ; RerportType:" + DInvDa.RerportType);
                using (var db = new Entities.DatabaseContext())
                {
                    if (DInvDa.ChkInvoiceNo == true)
                    {
                        if (DInvDa.Invoice_Number == null || DInvDa.Invoice_Number == "")
                        {
                            TempData["alertMessage"] = "Please Enter Invoice No";
                            ModelState.AddModelError("", "Please Enter Invoice No");
                            _logger.LogError("Please Enter Invoice No");   //log enhance by chaitrali 4/7/2024

                            return PartialView("Index");
                        }
                        Boolean IsAlphanumeric = CheckForSpecial(DInvDa.Invoice_Number);
                        if (IsAlphanumeric == false)
                        {
                            TempData["alertMessage"] = "Please Enter valid Invoice No";
                            ModelState.AddModelError("", "Please Enter Invoice No");
                            _logger.LogError("Please Enter Invoice No");   //log enhance by chaitrali 4/7/2024
                            return PartialView("Index");
                        }
                        //log enhance by chaitrali 4/7/2024
                        _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number ='" + DInvDa.Invoice_Number + "',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=1 - DownloadInvoice;Show");

                        inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number ='" + DInvDa.Invoice_Number + "',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=1").ToList();

                    }
                    //else if (DInvDa.ChkReporttype == false || (DInvDa.RerportType != "Without Trade Ref.No" && DInvDa.RerportType != "With Trade Ref.No"))
                    else if ((DInvDa.RerportType != "Without Trade Ref.No" && DInvDa.RerportType != "With Trade Ref.No"))
                    {
                        TempData["alertMessage"] = "Please Select Report Type";
                        ModelState.AddModelError("", "Please Select Report Type");
                        _logger.LogError("Please Select Report Type");   //log enhance by chaitrali 4/7/2024
                        return PartialView("Index");
                    }
                    //else if (DInvDa.ChkDate == true && DInvDa.ChkReporttype == false)
                    else if (DInvDa.ChkDate == true && (DInvDa.RerportType != "Without Trade Ref.No" && DInvDa.RerportType != "With Trade Ref.No"))
                    {
                        var Fromdate = DInvDa.DateFrom;
                        var Todate = DInvDa.DateTo;
                        if (Fromdate == null || Todate == null)
                        {
                            TempData["alertMessage"] = "Please Select Valid Date";
                            _logger.LogError("Please Select Valid Date");   //log enhance by chaitrali 4/7/2024

                            return PartialView("Index");
                        }
                        //log enhance by chaitrali 4/7/2024
                        _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '', @ToInvoiceDate = '" + Fromdate + "', @FromInvoiceDate = '" + Todate + "', @ReportType = '', @Flag = 2  - DownloadInvoice;Show");

                        inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='',@Flag=2").ToList();
                    }
                    //else if (DInvDa.ChkReporttype == true && DInvDa.ChkDate == true)
                    else if (DInvDa.ChkDate == true)
                    {
                        var Fromdate = DInvDa.DateFrom;
                        var Todate = DInvDa.DateTo;
                        if (Fromdate == null || Todate == null)
                        {
                            TempData["alertMessage"] = "Please Select Valid Date";

                            _logger.LogError("Please Select Valid Date");   //log enhance by chaitrali 4/7/2024
                            return PartialView("Index");
                        }
                        if (DInvDa.RerportType == "With Trade Ref.No")
                        { //log enhance by chaitrali 4/7/2024
                            _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=3 +  - DownloadInvoice;Show");

                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();
                            if (inv.Count == 0)
                            {
                                TempData["alertMessage"] = "No Record Found";
                                return PartialView("Index");
                            }

                        }
                        else if (DInvDa.RerportType == "Without Trade Ref.No")
                        {
                            //log enhance by chaitrali 4/7/2024
                            _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=3 +  - DownloadInvoice;Show");

                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();
                            if (inv.Count == 0)
                            {
                                TempData["alertMessage"] = "No Record Found";

                                return PartialView("Index");
                            }

                        }
                    }
                    //else if (DInvDa.ChkReporttype == true && DInvDa.ChkDate == false)
                    else if (DInvDa.ChkDate == false)
                    {
                        if (DInvDa.RerportType == "With Trade Ref.No")
                        {
                            //log enhance by chaitrali 4/7/2024
                            _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=5 +  - DownloadInvoice;Show");
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=5").ToList();
                            if (inv.Count == 0)
                            {
                                TempData["alertMessage"] = "No Record Found";

                                return PartialView("Index");
                            }

                        }
                        else if (DInvDa.RerportType == "Without Trade Ref.No")
                        {
                            //log enhance by chaitrali 4/7/2024
                            _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=5 +  - DownloadInvoice;Show");
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=5").ToList();
                            if (inv.Count == 0)
                            {
                                TempData["alertMessage"] = "No Record Found";
                                return PartialView("Index");
                            }

                        }
                    }
                    else
                    {
                        //log enhance by chaitrali 4/7/2024
                        _logger.LogError("EXEC uspDownloadInvoice @Invoice_Number = '', @ToInvoiceDate = '', @FromInvoiceDate = '', @ReportType = '', @Flag = 4 +  - DownloadInvoice;Show");
                        inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=4").ToList();
                    }

                }
                //log enhance by chaitrali 4/7/2024
                _logger.LogInformation("Executed successfully" + " - DownloadInvoice;Show");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - DownloadInvoice;Show");
            }

            return View(inv);

        }
        public ActionResult GenExcel(IList<DownloadFillInvoice> DInvDa, string[] IsSelect)
        {
            _logger.LogInformation("Generating excel" + " - DownloadInvoice;GenExcel");//log enhance by chaitrali 10/10/2024
            XLWorkbook wb = new XLWorkbook();
            try
            {
                //IEnumerable<object> DetailsList = DInvDa as IEnumerable<object>;
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
                            string InvoiceNumber = IsSelect[i].ToString();
                            DataRow dtFilter = Details.Select("[Invoice_Number]='" + InvoiceNumber + "'").FirstOrDefault();
                            dtFilterData.ImportRow(dtFilter);
                        }
                    }
                }


                //var DetailsList = inv.ToList();
                //DataTable Details = DetailsList.ToDataTable();
                //DataRow[] dr = Details.Select("IsSelect = false");
                //DataTable DT = dr.CopyToDataTable();
                DataTable DT = dtFilterData.Copy();
                DataTable dtIncremented = new DataTable();
                DataColumn dc = new DataColumn("Sr.No");
                dc.AutoIncrement = true;
                dc.AutoIncrementSeed = 1;
                dc.AutoIncrementStep = 1;
                dc.DataType = typeof(Int32);
                dtIncremented.Columns.Add(dc);

                dtIncremented.BeginLoadData();

                DataTableReader dtReader = new DataTableReader(DT);
                dtIncremented.Load(dtReader);

                dtIncremented.EndLoadData();
                dtIncremented.Columns.RemoveAt(1);
                dtIncremented.Columns["INVOICE_NUMBER"].ColumnName = "INVOICE NUMBER";
                dtIncremented.Columns["Invoice_Amount"].ColumnName = "INVOICE Amount";
                dtIncremented.Columns["Currency"].ColumnName = "CURRENCY";
                dtIncremented.Columns["Vehical_ID"].ColumnName = "DESCRIPTION OF GOODS";
                dtIncremented.Columns["DueDate"].ColumnName = "DUE DATE";
                dtIncremented.Columns["Dealer_Name"].ColumnName = "BUYER NAME";
                dtIncremented.Columns["Dealer_Address1"].ColumnName = "ADDRESS";
                dtIncremented.Columns["Dealer_City"].ColumnName = "CITY";
                dtIncremented.Columns["Transporter_Name"].ColumnName = "TRANSPORTER NAME";
                dtIncremented.Columns["Transport_Number"].ColumnName = "TRANSPORT NUMBER(L/R or D/C or GRN or MRIR)";
                dtIncremented.Columns["Transport_Date"].ColumnName = "TRANSPORT DATE";
                dtIncremented.Columns["Dealer_Code"].ColumnName = "DEALER CODE";
                dtIncremented.Columns["Transporter_Code"].ColumnName = "TRANSPORTERCODE";
                dtIncremented.Columns["Dealer_Address2"].ColumnName = "DEALER ADDRESS LINE 2";
                dtIncremented.Columns["Dealer_Address3"].ColumnName = "DEALER ADDRESS LINE 3";
                dtIncremented.Columns["Dealer_Address4"].ColumnName = "DEALER ADDRESS LINE 4";
                dtIncremented.Columns["IMEX_DEAL_NUMBER"].ColumnName = "TRADE REFERENCE NO";
                dtIncremented.Columns["TradeOp_Selected_Invoice_Date"].ColumnName = "PHYSICAL RECIEVED DATE";
                dtIncremented.Columns["Trade_OPs_Remarks"].ColumnName = "REMARK";
                dtIncremented.TableName = "Sheet1";
                using (wb)
                {
                    wb.Worksheets.Add(dtIncremented);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Phy_Pending" + System.DateTime.Now.ToString("ddMMyyyySStt") + ".xlsx");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - DownloadInvoice;GenExcel");//log enhance by chaitrali 4/7/2024
                return View();
            }
            finally
            {
                wb.Dispose();

            }

        }

    }
}

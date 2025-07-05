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
using System.Data.SqlClient;

namespace HDFCMSILWebMVC.Controllers
{
    public class UpdateInvoice : Controller
    {
        public static IList<DownloadFillInvoice> inv = new List<DownloadFillInvoice>();
        private readonly ILogger _logger;
        public UpdateInvoice(ILogger<UpdateInvoice> logger)
        {
            _logger = logger;
        }
        public IActionResult UpdateInvoices()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        }

        public IActionResult ShowInvoices(DownloadinvFrmDeta DInvDa)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (DInvDa == null)
                {
                    return NotFound();
                }

                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {

                        if (DInvDa.ChkInvoiceNo == true)
                        {
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number ='" + DInvDa.Invoice_Number + "',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=1").ToList();

                        }
                        else if (DInvDa.ChkDate == true && DInvDa.ChkReporttype == true)
                        {
                            var Fromdate = DInvDa.DateFrom;
                            var Todate = DInvDa.DateTo;
                            if (DInvDa.RerportType == "Select All")
                            {
                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=6").ToList();


                            }
                            else if (DInvDa.RerportType == "With Trade Ref.No")
                            {

                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=6").ToList();


                            }
                            else if (DInvDa.RerportType == "Without Trade Ref.No")
                            {
                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='" + DInvDa.RerportType + "',@Flag=6").ToList();

                            }
                        }
                        else if (DInvDa.ChkDate == true)
                        {
                            var Fromdate = DInvDa.DateFrom;
                            var Todate = DInvDa.DateTo;


                            //inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='" + Fromdate + "',@FromInvoiceDate='" + Todate + "',@ReportType='',@Flag=2").ToList();
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '', @ToInvoiceDate = @ToDate, @FromInvoiceDate = @FromDate, @ReportType = '', @Flag = 2",new SqlParameter("@ToDate", Fromdate),new SqlParameter("@FromDate", Todate)).ToList();


                        }
                        else if (DInvDa.ChkReporttype == true)
                        {
                            if (DInvDa.RerportType == "With Trade Ref.No")
                            {

                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();


                            }
                            else if (DInvDa.RerportType == "Without Trade Ref.No")
                            {
                                inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='" + DInvDa.RerportType + "',@Flag=3").ToList();

                            }
                        }
                        else
                        {
                            inv = db.Set<DownloadFillInvoice>().FromSqlRaw("EXEC uspDownloadInvoice @Invoice_Number = '',@ToInvoiceDate='',@FromInvoiceDate='',@ReportType='',@Flag=4").ToList();
                        }

                    }

                    _logger.LogInformation("Executed successfully" + " - DownloadInvoice;Show");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - DownloadInvoice;Show");
                }

                return View(inv);

            }
        }


        public ActionResult Update(IList<DownloadFillInvoice> DInvDa, string TradeRefNo, string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                XLWorkbook wb = new XLWorkbook();
                try
                {
                    string strTrdRef;

                    var DetailsList = inv.ToList();
                    //Boolean ChkFlag = false;
                    int Unsuccesscnt = 0;
                    int cnt = 0; string msg = "";

                    DataTable Details = DetailsList.ToDataTable();
                    if (IsSelect.Length == 0)
                    {

                        msg = "Please Select at least one Record to update.";
                        TempData["alertMessage"] = msg;
                        return RedirectToAction("ShowInvoices");
                    }
                    if (TradeRefNo == "" || TradeRefNo==null)
                    {

                        msg = "Please Enter Trade Ref. No.";
                        TempData["alertMessage"] = msg;
                        return RedirectToAction("ShowInvoices");
                    }
                    DataTable dtFile = Methods.InsertDetails("Insert_FileDescDetails", "Manual" + System.DateTime.Now.ToString("yyyyMMdd"), "IR", IsSelect.Length.ToString(), "2", "", "", "",_logger);
                    for (int i = 0; i < IsSelect.Length; i++)
                    {
                        string InvoiceNumber = IsSelect[i].ToString();

                        DataRow[] dtFilter = Details.Select("[Invoice_Number]='" + InvoiceNumber + "'");
                        //DataRow[] dtFilter = Details.Select().Where("Invoice_Number = '+ IsSelect[i].ToString()");
                        DataTable dtFilterData = dtFilter.CopyToDataTable();
                        strTrdRef = TradeRefNo;
                        if (TradeRefNo.Trim() != "" && TradeRefNo.Trim().Length > 4)
                        {

                            DataSet ds = Methods.getDetails_Web("Get_UploadInvoice", dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim(), "", "", "", "", "", "",_logger);

                            if (ds.Tables[0].Rows.Count > 0)// Get Table 1 rows      
                            {
                                _logger.LogError("Record Found In Invoice Table Invoice -" + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadReceivedInvoiceDataController;UploadDetails");
                                if (ds.Tables[1].Rows.Count > 0)// check table 2 rows
                                {
                                    Unsuccesscnt += 1;
                                    msg = "Duplicate record Found In Invoice Received Table Invoice -" + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim();
                                    _logger.LogError("Duplicate record Found In Invoice Received Table Invoice -" + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadReceivedInvoiceDataController;UploadDetails");
                  //                  ChkFlag = false;
                                    goto Phy_Inv_Not_Rec;
                                }
                                else //'3
                                {
                                    DataTable dsUpdate = Methods.UpdateDetails("Update_InvoiceFCC_Details", dtFile.Rows[0][0].ToString(), UserSession.LoginID, strTrdRef.Trim(), ds.Tables[0].Rows[0]["TradeOp_Selected_Invoice_Date"].ToString().Trim(), "Y", dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim(), "", _logger);

                                    if (ds.Tables[0].Rows[0]["IMEX_DEAL_NUMBER"].ToString() == "NULL")
                                    {
                                        msg = "Value of imex_deal_number not updated for InvoiceNo - " + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim();
                                        _logger.LogError("Value of imex_deal_number not updated for InvoiceNo -" + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadReceivedInvoiceDataController;UploadDetails");
                                        Unsuccesscnt += 1;
                                        goto Phy_Inv_Not_Rec;
                                    }
                                }
                                DataSet dsRec = Methods.getDetails_Web("Get_UploadInvoice", dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim(), "", "", "", "", "", "",_logger);
                                if (dsRec.Tables[0].Rows[0]["IMEX_DEAL_NUMBER"].ToString() == "NULL")// '4
                                {
                                    _logger.LogError("Value of imex_deal_number=" + dsRec.Tables[0].Rows[0]["IMEX_DEAL_NUMBER"].ToString() + "and StepDate =" + dsRec.Tables[0].Rows[0]["stepdate"].ToString() + " not updated for InvoiceNo -" + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadReceivedInvoiceDataController;UploadDetails");
                                }
                                else
                                {
                                    string[] detailsCash = new string[25];
                                    detailsCash[0] = "0";   //@Invoice_Rec_ID
                                    detailsCash[1] = cnt.ToString(); //Sr_No
                                    detailsCash[2] = dsRec.Tables[0].Rows[0]["Invoice_Number"].ToString(); //Invoice_Number
                                    detailsCash[3] = dsRec.Tables[0].Rows[0]["Invoice_Amount"].ToString(); //Invoice_Amount
                                    detailsCash[4] = dsRec.Tables[0].Rows[0]["Currency"].ToString(); //Currency
                                    detailsCash[5] = dsRec.Tables[0].Rows[0]["Vehical_ID"].ToString();   //Vehical_ID
                                    detailsCash[6] = dsRec.Tables[0].Rows[0]["DueDate"].ToString();   //DueDate
                                    detailsCash[7] = dsRec.Tables[0].Rows[0]["Dealer_Name"].ToString();   //Dealer_Name
                                    detailsCash[8] = dsRec.Tables[0].Rows[0]["Dealer_Address1"].ToString();   //Dealer_Address1
                                    detailsCash[9] = dsRec.Tables[0].Rows[0]["Dealer_City"].ToString();   //Dealer_City
                                    detailsCash[10] = dsRec.Tables[0].Rows[0]["Transporter_Name"].ToString();   //Transporter_Name
                                    detailsCash[11] = dsRec.Tables[0].Rows[0]["Transport_Number"].ToString();   //Transport_Number
                                    detailsCash[12] = dsRec.Tables[0].Rows[0]["Transport_Date"].ToString();   //Transport_Date
                                    detailsCash[13] = dsRec.Tables[0].Rows[0]["Dealer_Code"].ToString();   //Dealer_Code
                                    detailsCash[14] = dsRec.Tables[0].Rows[0]["Transporter_Code"].ToString();   //Transporter_Code
                                    detailsCash[15] = dsRec.Tables[0].Rows[0]["Dealer_Address2"].ToString();   //Dealer_Address2
                                    detailsCash[16] = dsRec.Tables[0].Rows[0]["Dealer_Address3"].ToString();   //Dealer_Address3
                                    detailsCash[17] = dsRec.Tables[0].Rows[0]["Dealer_Address4"].ToString();   //Dealer_Address4
                                    detailsCash[18] = dsRec.Tables[0].Rows[0]["IMEX_DEAL_NUMBER"].ToString();   //Trade_RefNo
                                    detailsCash[19] = dsRec.Tables[0].Rows[0]["TradeOp_Selected_Invoice_Date"].ToString();   //Physical_Received_Date
                                    detailsCash[20] = dsRec.Tables[0].Rows[0]["Trade_OPs_Remarks"].ToString();   //Remarks
                                    detailsCash[21] = UserSession.LoginID;   //LoginID
                                    DataTable dsUpdate = Methods.Insert_InvoiceRecievedDetails("Save", detailsCash,_logger);
                                    cnt = cnt + 1;
                                    _logger.LogInformation("FCC file record inserted into InvoiceRecieve for Invoice No " + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadInvoiceController;Update"); ////Enhance by yogesh
                                }
                            }
                            else
                            {
                                msg = "Physical Invoice Data Not Received. Invoice Number : " + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim();
                                _logger.LogError("Physical Invoice Data Not Received. Invoice Number : " + dtFilterData.Rows[0]["Invoice_Number"].ToString().Trim() + " - UploadInvoiceController;Update");

                                //ChkFlag = false;
                                goto Phy_Inv_Not_Rec;
                            }
                        }
                    }
                    _logger.LogInformation("File Convertion Completed  and Total Records :  " + IsSelect.Length + " UnSuccessfull Records : " + (IsSelect.Length - cnt));// + " and Data Succesfully Converted : " + cnt + " - UploadReceivedInvoiceDataController;UploadDetails");


                    Phy_Inv_Not_Rec: string str = UserSession.LoginID;
                    DataTable dtF = Methods.UpdateDetails("Update_FileDesc", dtFile.Rows[0][0].ToString(), "0", "", "", "", "", "", _logger);
                    if (msg != "")
                        TempData["alertMessage"] = msg;  //" and Data Succesfully Converted : " + cnt; ////Enhance by yogesh
                    else
                        TempData["alertMessage"] = "FCC File Uploaded  and Total Records:  " + IsSelect.Length + " UnSuccessfull Records : " + (IsSelect.Length - cnt);  //" and Data Succesfully Converted : " + cnt; ////Enhance by yogesh

                    return RedirectToAction("UpdateInvoices");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);// + " and Data Succesfully Converted : " + cnt + " - UploadReceivedInvoiceDataController;UploadDetails");
                    return RedirectToAction("UpdateInvoices"); }
                finally
                {
                    wb.Dispose();

                }
            }
        }

    }
}

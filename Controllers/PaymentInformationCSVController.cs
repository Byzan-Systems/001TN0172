using HDFCMSILWebMVC.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.Extensions.Logging; //Enhance by yogesh
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Hosting;
using System.Data.OleDb;


namespace HDFCMSILWebMVC.Controllers
{
    public class PaymentInformationCSVController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        public PaymentInformationCSVController(ILogger<PaymentInformationCSVController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public string strFileName = "";
        public string strmessage = "";
        public IActionResult ShowPaymentInformationCSV()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult UploadPayment(IFormFile upload)
        {
            StreamReader objStrmReader = null;
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                   // System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                    _logger.LogError("Garbage Collector1" + "- PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh

                    int TotalRows = 0, TotalColumns = 0;
                    Rectify();
                    TempData["alertMessage"] = "Processing.............";
                    if (HttpContext.Request.Form.Files[0].FileName == "")
                    {
                        TempData["alertMessage"] = "Please Select Input File....";
                        return RedirectToAction("ShowPaymentInformationCSV");
                    }

                    WaitR:
                    DataTable dt = Methods.getDetails("GetReportDetails", "", "", "", "", "", "", "",_logger);
                    if (dt.Rows.Count > 0)
                    {
                        _logger.LogError("Waiting for Server to free.Please wait for some time.  " + " - PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                        TempData["alertMessage"] = "Waiting for Server to free.Please wait for some time.";
                        goto WaitR;
                    }
                    TempData["alertMessage"] = "";
                    strFileName = Path.GetFileName(HttpContext.Request.Form.Files[0].FileName);
                    DataTable dtFile = Methods.InsertDetails("Insert_FileDesc", strFileName, "NEFT", "0", "", "", "", "",_logger);


                    var uploads = Path.Combine(_environment.WebRootPath, "files");
                    string FileName = HttpContext.Request.Form.Files[0].FileName;
                    if (System.IO.File.Exists(Path.Combine(uploads, FileName)))
                    {
                        _logger.LogInformation("File Exist at" + uploads + "\\" + FileName + " - PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                        System.IO.File.Delete(Path.Combine(uploads, FileName));
                        _logger.LogInformation("File Delete at From" + uploads  + " - PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                    }

                   // System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                    _logger.LogInformation("Garbage Collector2" + "- PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                    using (var fileStream = new FileStream(Path.Combine(uploads, FileName), FileMode.Create))
                    {
                        //HttpContext.Request.Form.Files[0].CopyToAsync(fileStream);
                        HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                        fileStream.Dispose();
                        fileStream.Close();
                        _logger.LogError("File Create at" + uploads + " - PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh

                    }

                    //System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                    _logger.LogInformation("Garbage Collector3" + "- PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                    objStrmReader = new StreamReader(Path.Combine(uploads, FileName));
                    _logger.LogInformation("Reading File from " + uploads + "\\" + FileName + "- PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                    string Read_Line = "";
                    int TotalUnSuccess = 0;
                    while (!objStrmReader.EndOfStream)
                    {
                        Read_Line = objStrmReader.ReadLine();
                        string[] Data = Read_Line.Split(",");
                        if (!Data[0].Contains("Transaction ID") && Data[0].ToString() != "")
                        {
                            String Transaction_Id = Data[0].ToString();
                            String Product = Data[1].ToString();
                            String Party_Code = Data[2].ToString();
                            String Party_Name = Data[3].ToString();
                            String RemittingBank = Data[4].ToString();
                            String UTR_No = Data[5].ToString();
                            String Entry_Amount = Data[6].ToString();
                            String IFSC_code = Data[7].ToString();
                            String DealerVirAccNo = Data[8].ToString();
                            TotalRows += 1;
                            TotalColumns = Data.Length;
                            if (TotalRows == 1)
                                _logger.LogInformation("Total Rows count  " + TotalRows + "   - PaymentInformationCSVController;UploadPayment");

                            string[] Details = new string[20];

                            if (DealerVirAccNo != "0")
                            {
                                _logger.LogInformation(DealerVirAccNo + "Assign to variable" + "   - PaymentInformationCSVController;UploadPayment"); ////Enhance by yogesh

                                if (DealerVirAccNo.Length != 23)
                                {
                                    TotalUnSuccess += 1;
                                    Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "DO length is less or greater than 23", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                                    TempData["alertMessage"] = "Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail");
                                    _logger.LogError("Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail " + " - PaymentInformationCSVController;UploadPayment"));  ////Enhance by yogesh
                                }
                                else
                                {
                                    string Fname = DealerVirAccNo.Substring(DealerVirAccNo.Length - 1, 1);
                                    //Condition check for Financier Dealer match with B / C / F / K / V / D
                                    if (Fname == "B" || Fname == "C" || Fname == "F" || Fname == "K" || Fname == "V" || Fname == "D")
                                    {
                                        Details[0] = Transaction_Id; //Transaction_Id
                                        Details[1] = "C"; //Type_of_Entry
                                        Details[2] = "C"; //Dr_CR
                                        Details[3] = Entry_Amount; //Entry_Amount
                                        Details[4] = ""; //Value_date
                                        Details[5] = Product; //Product
                                        Details[6] = Party_Code; //Party_Code
                                        Details[7] = Party_Name; //Party_Name
                                        Details[8] = DealerVirAccNo; //VA_account
                                        Details[9] = ""; //Locations
                                        Details[10] = RemittingBank; //RemittingBank
                                        Details[11] = UTR_No; //UTR_No
                                        Details[12] = IFSC_code; //IFSC_code
                                        Details[13] = ""; //Dealer_Name
                                        Details[14] = ""; //Dealer_Account_No
                                        Details[15] = ""; //Releated_Ref_No
                                        Details[16] = dtFile.Rows[0][0].ToString(); //fileID
                                        Details[17] = UserSession.LoginID.ToString(); //Login ID

                                        string[] detailsCash = new string[25];
                                        detailsCash[0] = "0";   //cashopsID
                                        detailsCash[1] = Details[16];   //FileID
                                        detailsCash[2] = Details[5]; //CashOps_FileType
                                        detailsCash[3] = ""; //NEFT_RTGS_BT_ID
                                        detailsCash[4] = Details[8].Substring(0, 22); //Virtual_Account
                                        detailsCash[5] = Details[11]; //UTR_No
                                        detailsCash[6] = Details[3]; //Transaction_Amount
                                        detailsCash[7] = "C"; //Transaction_status
                                        detailsCash[8] = DBNull.Value.ToString(); //Payment_Status
                                        detailsCash[9] = DBNull.Value.ToString(); //Attempt
                                        detailsCash[10] = System.DateTime.Now.ToString("DD/MMM/YYYY"); //Cash_Ops_Date
                                        detailsCash[11] = System.DateTime.Now.ToString("HH:MM:SS AMPM"); //Cash_Ops_Time
                                        detailsCash[12] = "0"; //GEFO_Flag
                                        detailsCash[13] = DBNull.Value.ToString(); //GEFO_Date
                                        detailsCash[14] = DBNull.Value.ToString(); //DR_Account_No
                                        detailsCash[15] = DBNull.Value.ToString(); //CR_Account_No
                                        detailsCash[16] = UserSession.LoginID; //LoginID
                                        detailsCash[17] = DBNull.Value.ToString(); //EOD_MailFlag
                                        detailsCash[18] = DBNull.Value.ToString(); //DRC_Generation
                                        detailsCash[19] = Details[8]; //FNCR_Virtual_Account
                                        detailsCash[20] = Details[12]; //IFSC_code
                                        detailsCash[21] = Details[10]; //FNCR_code
                                        detailsCash[22] = ""; //FNCR_Name

                                        Methods.CashOps_Payments(Details, detailsCash,_logger);

                                        TempData["alertMessage"] = "File Successfully Uploaded. Total Successful Records : " + TotalRows + "     UnSuccessful Records : " + TotalUnSuccess; ////Enhance by yogesh
                                    }
                                    else
                                    {
                                        TotalUnSuccess += 1;
                                        Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Mismatched Dealer Financier Name", DealerVirAccNo + "-" + "Mismatched Dealer Financier Name", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                                        TempData["alertMessage"] = "Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail");
                                        _logger.LogError("Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail"));  ////Enhance by yogesh
                                    }
                                }
                            }
                        }


                    }

                    objStrmReader.Close();
                    objStrmReader.Dispose();
                    //System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                    
                    if (TotalRows > 0)
                        _logger.LogInformation("File Successfully Uploaded. Total Successful Records : " + TotalRows + " UnSuccessful Records : " + TotalUnSuccess + " - PaymentInformationCSVController;UploadPayment");  ////Enhance by yogesh
                        TempData["alertMessage"] = "File Successfully Uploaded. Total Successful Records : " + TotalRows + "     UnSuccessful Records : " + TotalUnSuccess; ////Enhance by yogesh


                }
                catch (Exception ex)
                {
                    TempData["alertMessage"] = ex.Message;
                    objStrmReader.Close();
                    objStrmReader.Dispose();
                    //System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();

                }
                finally
                {
                    objStrmReader.Close();
                    objStrmReader.Dispose();
                    //System.GC.Collect();
                    //System.GC.WaitForPendingFinalizers();
                }
                return RedirectToAction("ShowPaymentInformationCSV");
            }
        }


        private void Rectify()
        {
            try
            {
                DataTable dt = Methods.getDetails("GetOrder_CashOpsDetails", "", "", "", "", "", "", "",_logger);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataTable dt_Up = Methods.UpdateDetails("Update_OrderInv_Details", dt.Rows[i]["Cash_Ops_ID"].ToString(), dt.Rows[i]["Virtual_Account"].ToString(), dt.Rows[i]["UTR_No"].ToString(), dt.Rows[i]["Order_ID"].ToString(), "", "", "",_logger);
                    }
                }
                // thiss method get details from cashops join with invoice table as per Payment_Status = 'CREDIT MSIL and Invoice_Status<>'Payment received on'
                // and update order_desc and invoice table as per condtions
                DataTable dtCash = Methods.getDetails("GetCashOps_INVDetails", "", "", "", "", "", "", "",_logger);
                if (dtCash.Rows.Count > 0)
                {
                    //clserr.WriteLogToTxtFile("Data Updated Successfully", "Rectify", "Rectify");
                }
            }
            catch (Exception ex)
            {
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Rectify");
            }
        }
    }
}

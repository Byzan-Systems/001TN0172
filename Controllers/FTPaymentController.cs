using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _001TN0172.Controllers
{
    public class FTPaymentController : Controller
    {
        public string strFileName = "";
        public string strmessage = "";
        private readonly ILogger _logger;
        private IHostingEnvironment _environment;
        public FTPaymentController(ILogger<FTPaymentController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public IActionResult ShowFTPayment()
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
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {

                try
                {
                    int TotalRows = 0;
                    Rectify();
                    TempData["alertMessage"] = "Processing.............";
                    if (HttpContext.Request.Form.Files[0].FileName == "")
                    {
                        // MessageBox.Show("Please Select Input File....", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //Txt_FileName.Text = "";
                        //Txt_FileName.Focus();
                    }

                WaitR:
                    DataTable dt = Methods.getDetails("GetReportDetails", "", "", "", "", "", "", "", _logger);
                    if (dt.Rows.Count > 0)
                    {
                        TempData["alertMessage"] = "Waiting for Server to free.Please wait for some time.";
                        goto WaitR;
                    }
                    TempData["alertMessage"] = "";
                    DataTable dtFile = Methods.InsertDetails("Insert_FileDesc", strFileName, "FT", "0", "", "", "", "", _logger);
                    string[] Details = new string[20];

                    StreamReader objStrmReader = new StreamReader(HttpContext.Request.Form.Files[0].FileName);
                    int T_id = 0;
                    while (!objStrmReader.EndOfStream)
                    {
                        string ReadOP_Line = objStrmReader.ReadLine();

                        if (ReadOP_Line.Substring(2, 11) != "")
                        {
                            TotalRows += 1;
                            if (ReadOP_Line.Substring(13, 2) == "DO")
                            {
                                T_id = T_id + 1;
                                string Amount = ReadOP_Line.Substring(104, 21);
                                string ValueDate = ReadOP_Line.Substring(72, 11);
                                string VA_Account = ReadOP_Line.Substring(16, 22);
                                string UTR_No = ReadOP_Line.Substring(41, 31);
                                string Dealer_AccountNo = ReadOP_Line.Substring(56, 15);
                                InsertCashops_Payment(T_id.ToString(), Amount, ValueDate, VA_Account, UTR_No, Dealer_AccountNo, dtFile.Rows[0][0].ToString(), HttpContext.Request.Form.Files[0].FileName);
                                TempData["alertMessage"] = HttpContext.Request.Form.Files[0].FileName + " File Successfully Uploaded.Total Rows: " + TotalRows + " Successfull Records: " + T_id + "";
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    TempData["alertMessage"] = ex.Message;
                    _logger.LogError(ex.Message + " - FTPaymentController;UploadPayment");
                }
                return RedirectToAction("ShowPaymentInformation");
            }
        }

        private void InsertCashops_Payment(string Transaction_Id, string Entry_Amount, string Value_date, string DealerVirAccNo, string UTR_No, string Dealer_Account_No, string fileID, string filename)
        {
            try
            {
                string[] Details = new string[20];
                Details[0] = Transaction_Id; //Transaction_Id
                Details[1] = "T"; //Type_of_Entry
                Details[2] = "C"; //Dr_CR
                Details[3] = Entry_Amount; //Entry_Amount
                Details[4] = Value_date; //Value_date
                Details[5] = "FT"; //Product
                Details[6] = ""; //Party_Code
                Details[7] = ""; //Party_Name
                Details[8] = DealerVirAccNo; //VA_account
                Details[9] = ""; //Locations
                Details[10] = ""; //RemittingBank
                Details[11] = UTR_No; //UTR_No
                Details[12] = ""; //IFSC_code
                Details[13] = ""; //Dealer_Name
                Details[14] = Dealer_Account_No; //Dealer_Account_No
                Details[15] = ""; //Releated_Ref_No
                Details[16] = fileID; //fileID
                Details[17] = UserSession.LoginID; //Login ID

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
                CashOps_Payments(Details, detailsCash, filename);
            }
            catch (Exception ex)
            {
                TempData["alertMessage"] = ex.Message;
                _logger.LogError(ex.Message + " - FTPaymentController;UploadPayment");
                _logger.LogError(ex.Message);
            }
        }
        public void CashOps_Payments(string[] Details, string[] detailsCash, string strFileName)
        {
            try
            {
                //Check Duplicate UTR No. and Amount
                DataTable dt = Methods.getDetails("Get_CashOpsDetails", Details[0], Details[1], "", "", "", "", "",_logger);
                if (dt.Rows.Count > 0)
                {
                    _logger.LogError("Duplicate UTR No.: " + Details[11] + " with same  Amount: " + Details[3], "CashOps_Payments", strFileName);
                    return;
                }

                DataTable dtCash = Methods.Insert_PaymentUploadDetails("Save", Details,_logger);
                if (Details[2].ToUpper() == "C")
                {
                    Fill_CashOpsDetails(dtCash.Rows[0][0].ToString(), Details, detailsCash);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; FTPaymentController;CashOps_Payments");
                //_logger.LogError(ex.Message);
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "CashOps_Payments");
            }
        }
        public  void Fill_CashOpsDetails(string NEFT_RTGS_BT_ID, string[] Details, string[] detailsCash)
        {
            try
            {
                string strIFSCPart = Details[12].ToString().Substring(0, 4);
                DataTable dt_Finance = Methods.getDetails("Get_FinancerDetailsAsPer", Details[8].ToString().Substring(Details[8].Length - 1, 1), strIFSCPart, "", "", "", "", "",_logger);


                //clserr.WriteLogToTxtFile("Insert values into CashOps Upload Table", "Fill_CashOpsDetails", strFileName);
                detailsCash[3] = NEFT_RTGS_BT_ID; //NEFT_RTGS_BT_ID
                                                  //if (Details[1] == "MAN")
                                                  //{

                if (dt_Finance.Rows.Count > 0)
                {
                    detailsCash[21] = dt_Finance.Rows[0]["FCode"].ToString(); detailsCash[22] = dt_Finance.Rows[0]["FName"].ToString();
                }

                DataTable dt = Methods.Insert_CashOpsDetails("Save", detailsCash, _logger);
                //clserr.WriteLogToTxtFile("FNCR Virtual Account and UTR no in CashOps = " + detailsCash[19] + " and " + detailsCash[5], "Fill_CashOpsDetails", strFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; FTPaymentController;Fill_CashOpsDetails");
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Fill_CashOpsDetails");
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
                _logger.LogError(ex.Message + " - FTPaymentController;Rectify");
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Rectify");
            }
        }
    }
}

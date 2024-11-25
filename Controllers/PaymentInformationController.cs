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
//using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Hosting;
using System.Data.OleDb;
//using ExcelDataReader;
using Spire.Xls;
using System.Xml.Linq;
using System.Text;

namespace HDFCMSILWebMVC.Controllers
{
    public class PaymentInformationController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        public PaymentInformationController(ILogger<PaymentInformationController> logger,IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public string strFileName = "";
        public string strmessage = "";
        public IActionResult ShowPaymentInformation()
        {

            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                ViewBag.Count = 0;
                ViewBag.Percentage = 0;

                return View();
            }

        }
        [HttpPost]
        public IActionResult UploadPayment(IFormFile upload, int Count, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                string connString;
                string timestamp;
                int firstRow = 0;
                //Create a Workbook object
                Workbook wb = new Workbook();
                //HSSFWorkbook hssfwb = new HSSFWorkbook(); ISheet sheet;
                try
                {
                    ViewBag.Count = Count + 1;
                    ViewBag.Percentage = 25;

                    int TotalRows = 0, TotalColumns = 0;
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
                        _logger.LogInformation("Waiting for Server to free.Please wait for some time.  " + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                        TempData["alertMessage"] = "Waiting for Server to free.Please wait for some time.";
                        goto WaitR;
                    }
                    TempData["alertMessage"] = "";
                    strFileName = Path.GetFileName(HttpContext.Request.Form.Files[0].FileName);
                    DataTable dtFile = Methods.InsertDetails("Insert_FileDesc", strFileName, "NEFT", "0", "", "", "", "", _logger);

                    var uploads = Path.Combine(_environment.WebRootPath, "files");
                    string FileName = HttpContext.Request.Form.Files[0].FileName;
                    timestamp = DateTime.Now.ToString("ddMMyyyyHHmmss");

                    FileName = Path.GetFileNameWithoutExtension(FileName) + "_" + timestamp + Path.GetExtension(FileName);

                    //if (System.IO.File.Exists(Path.Combine(uploads, FileName)))
                    //{
                    //    System.IO.File.Delete(Path.Combine(uploads, FileName));
                    //}

                    using (var fileStream = new FileStream(Path.Combine(uploads, FileName), FileMode.Create))
                    {
                        HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                        fileStream.Dispose();
                        fileStream.Close();
                    }


                    string[] Details = new string[20];
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    //  strFileName = $"{hostingEnvironment.WebRootPath}\\Files\\{HttpContext.Request.Form.Files[0].FileName}";

                    strFileName = $"{hostingEnvironment.WebRootPath}\\Files\\{FileName}";

                    //Stream stream = System.IO.File.Open(strFileName, FileMode.Open);
                    //wb.LoadFromStream(stream); 

                    //Load an existing Excel file
                    wb.LoadFromFile(strFileName);
                    //wb.LoadFromFile(strFileName,ExcelVersion.Version97to2003);
                    //Get the first worksheet
                    Worksheet sheet = wb.Worksheets[0];

                    DataTable dt1 = new DataTable();
                    dt1 = sheet.ExportDataTable();
                    //dt1 = GetDatatable_Text(strFileName);  CSV reding 

                    //Iterate through the rows

                    RejectPaymentFile(dt1);
                    int issusccess = 0, issuccessGlobal = 1;
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                        if (dt1.Rows[i][0].ToString() != null)
                        {
                            DataFormatter formatter = new DataFormatter();
                            String Transaction_Id = dt1.Rows[i][0].ToString(); //result.Rows[k].ItemArray[0].ToString();  //formatter.FormatCellValue(sheet.GetRow(k).GetCell(0));
                            String Product = dt1.Rows[i][1].ToString(); //result.Rows[k].ItemArray[1].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(1));
                            String Party_Code = dt1.Rows[i][2].ToString(); //result.Rows[k].ItemArray[2].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(2));
                            String Party_Name = dt1.Rows[i][3].ToString(); //result.Rows[k].ItemArray[3].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(3));
                            String RemittingBank = dt1.Rows[i][4].ToString(); //result.Rows[k].ItemArray[4].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(4));
                            String UTR_No = dt1.Rows[i][5].ToString(); //result.Rows[k].ItemArray[5].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(5));
                            String Entry_Amount = dt1.Rows[i][6].ToString(); //result.Rows[k].ItemArray[6].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(6));
                            String IFSC_code = dt1.Rows[i][7].ToString(); //result.Rows[k].ItemArray[7].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(7));
                            String DealerVirAccNo = dt1.Rows[i][8].ToString(); //result.Rows[k].ItemArray[8].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(8));
                            _logger.LogInformation(DealerVirAccNo + "Assign to variable" + "   - PaymentInformationController;UploadPayment"); ////Enhance by yogesh

                            if (Product == "NEFT" || Product == "RTGS" || Product == "FT" || Product == "FUND TRANS")
                            {
                            }
                            else
                            {
                                //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "Invalid Payment Type, Please upload correct file again with Payment Type such as NEFT, RTGS, FT or FUND TRANS", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                                //TempData["alertMessage"] = "Invalid Payment type. Please upload the correct file again.";
                                _logger.LogInformation("Invalid Payment type. Please upload the correct file again." + "" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                                //break;
                            }

                            if (DealerVirAccNo != "0")
                            {
                                if (DealerVirAccNo.Length != 23)
                                {


                                    //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "DO length is less or greater than 23", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));

                                    //TempData["alertMessage"] = "Invalid Virtual Account Number. Please upload the correct file again.";
                                    _logger.LogInformation("Invalid Virtual Account Number. Please upload the correct file again." + "" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                                    //break;
                                }
                                else if (DealerVirAccNo.Length == 23)
                                {
                                    //select distinct FType from FinancerDetails

                                    DataTable dtabl = Methods.getDetails("GetFinancerCodeDetails", DealerVirAccNo.Substring(22, 1).ToString(), "", "", "", "", "", "", _logger);

                                    if (dtabl.Rows.Count == 0)

                                    {
                                        //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "Product Code is not valid", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));

                                        //TempData["alertMessage"] = "Invalid Virtual Account Number. Please upload the correct file again.";
                                        _logger.LogInformation("Invalid Virtual Account Number. Please upload the correct file again." + "" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh  ////Enhance by yogesh
                                                                                                                                                                                                 //break;   
                                    }
                                }
                            }

                            //////For Duplicate UTR NO  Added on 23-02-2024
                            DataTable dtUTRDuplicate = Methods.getDetails("GetUTRDetailsForPaymentUpload", UTR_No, "", "", "", "", "", "", _logger);
                            if (dtUTRDuplicate.Rows.Count > 0)
                            {
                                _logger.LogInformation("UTR No is duplicate." + "" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh  ////Enhance by yogesh
                            }
                            //////For Duplicate UTR NO  Added on 23-02-2024

                            string Fname = DealerVirAccNo.Substring(DealerVirAccNo.Length - 1, 1);
                            string strRemitterBankName = RemittingBank;
                            //string strIFSCPart;

                            //if (Fname == "V")
                            //{
                            //    strIFSCPart = "";
                            //}
                            //else
                            //{
                            //    strIFSCPart = IFSC_code.Substring(0, 4);
                            //}
                            //DataTable dt_Finance = Methods.getDetails("Get_FinancerDetailsAsPerTypeIFSC", Fname, strIFSCPart, "", "", "", "", "");

                            DataTable dt_Finance = Methods.getDetails("Get_FinancerDetailsAsPerTypeIFSC", Fname, "", "", "", "", "", "", _logger);
                            //Condition check for Financier Dealer match with B / C / F / K / V
                            if (dt_Finance.Rows.Count > 0) //  if (Fname == "B" || Fname == "C" || Fname == "F" || Fname == "K" || Fname == "V" || Fname == "D")
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
                                                      //clserr.WriteLogToTxtFile("In Cashops_Payment Function", "Upload_Click", strFileName);
                                issusccess = Methods.CashOps_Payments(Details, detailsCash, _logger);
                                TotalRows += 1;
                                if (issusccess == 0)
                                    issuccessGlobal = issusccess;
                            }
                            else
                            {
                                //clserr.WriteLogToTxtFile("Financier Dealer Name does not match with B/C/F/K/V = " + sheet.GetRow(k).GetCell(8).StringCellValue, "Upload_Click", strFileName);
                                //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Mismatched Dealer Financier Name", DealerVirAccNo + "-" + "Mismatched Dealer Financier Name", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                                Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Either Product code or IFSC Code is invalid", DealerVirAccNo + "-" + "Either Product code or IFSC Code is invalid", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                                //clserr.WriteLogToTxtFile("Mismatched Fin Dealer mail sent to - " + sttg.Payment_Rejection_EMail, "Upload_Click", strFileName);
                                //TempData["alertMessage"] = "Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail");

                                _logger.LogInformation("Either Product code or IFSC Code is invalid mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail"));  ////Enhance by yogesh
                            }

                           
                        }

                        firstRow += 1;
                    }
                    if (issusccess == 0)
                    {
                        TempData["alertMessage"] = HttpContext.Request.Form.Files[0].FileName + " File Uploaded fail. Please ReLogin and Try again."; ////Enhance by yogesh
                        _logger.LogInformation(HttpContext.Request.Form.Files[0].FileName + " File Uploaded fail. Please ReLogin and Try again. " + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh

                    }
                    else
                    {
                        TempData["alertMessage"] = HttpContext.Request.Form.Files[0].FileName + " File Successfully Uploaded. Successfull Records " + TotalRows + ""; ////Enhance by yogesh
                        _logger.LogInformation(HttpContext.Request.Form.Files[0].FileName + " File Successfully Uploaded. Successfull Records  " + TotalRows + "" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    }

                    sheet.Dispose();
                    wb.Dispose();


                }
                catch (Exception ex)
                {
                    TempData["alertMessage"] = ex.Message + " " + ex.StackTrace;
                    //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Upload_Click");

                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //hssfwb1.Close();
                    //sheet = null;


                }
                return RedirectToAction("ShowPaymentInformation");
            }
        }

        private void AddColumnToTable(DataTable pDt, int pCols)
        {

            if (pDt is null)
            {
                pDt = new DataTable("Input");
            }

            if (pDt.Columns.Count < pCols)
            {
                pDt.Columns.Add(new DataColumn("Column_" + pDt.Columns.Count));
                AddColumnToTable(pDt, pCols);
            }

        }

        public DataTable GetDatatable_Text(string StrFilePath)
        {
            string[] strTemp = null;
            string TmpLineStr = string.Empty;
            DataTable DtInput = new DataTable();
            StreamReader strReader = new StreamReader(StrFilePath);
            int linecnt = 0;
            try
            {
                while (strReader.EndOfStream == false)
                {
                    linecnt += 1;
                    TmpLineStr = strReader.ReadLine();
                    strTemp = TmpLineStr.ToString().Split(','); //GetInArrayByComma(TmpLineStr);
                    if (linecnt > 1)
                    {
                        AddColumnToTable(DtInput, strTemp.Length);
                        DtInput.Rows.Add(strTemp);

                    }

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (!(strReader == null))
                {
                    strReader.Close();
                    strReader.Dispose();
                }
                strReader = null;

            }
            return DtInput.Copy();
        }

        private void Rectify()
        {
            try
            {
                DataTable dt = Methods.getDetails("GetOrder_CashOpsDetails", "", "", "", "", "", "", "", _logger);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataTable dt_Up = Methods.UpdateDetails("Update_OrderInv_Details", dt.Rows[i]["Cash_Ops_ID"].ToString(), dt.Rows[i]["Virtual_Account"].ToString(), dt.Rows[i]["UTR_No"].ToString(), dt.Rows[i]["Order_ID"].ToString(), "", "", "", _logger);
                    }
                }
                // thiss method get details from cashops join with invoice table as per Payment_Status = 'CREDIT MSIL and Invoice_Status<>'Payment received on'
                // and update order_desc and invoice table as per condtions
                DataTable dtCash = Methods.getDetails("GetCashOps_INVDetails", "", "", "", "", "", "", "", _logger);
                if (dtCash.Rows.Count > 0)
                {
                    //clserr.WriteLogToTxtFile("Data Updated Successfully", "Rectify", "Rectify");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  PaymentInformationController;Rectify");
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Rectify");
            }
        }


        public Boolean RejectPaymentFile(DataTable dt)
        {
            try
            {
                Boolean rpt = true;
                DateTime currentTime = DateTime.Now;
                string filepath = this._environment.WebRootPath + "\\files\\" + "RejectedPaymentReason" + currentTime.ToString("hhmmss") + ".csv";
                string line = "";
                StringBuilder EmailBodyCtn = new StringBuilder();
                const string quote = "\"";
                using (var w = new StreamWriter(filepath))
                {
                    line = quote + "Product" + quote + "," + quote + "Remitter Name" + quote + "," + quote + "Remitter A/ C No" + quote + "," + quote + "Remitter Bank Name" + quote + "," + quote + "UTR No" + quote + "," + quote + "Amount" + quote + "," + quote + "IFSC Code" + quote + "," + quote + "VirtualAccountNumber" + quote + "," + quote + "RejectedReason" + quote;
                    w.WriteLine(line);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        String Product = dt.Rows[i][1].ToString(); //result.Rows[k].ItemArray[1].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(1));
                        String Party_Code = dt.Rows[i][2].ToString(); //result.Rows[k].ItemArray[2].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(2));
                        String Party_Name = dt.Rows[i][3].ToString(); //result.Rows[k].ItemArray[3].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(3));
                        String RemittingBank = dt.Rows[i][4].ToString(); //result.Rows[k].ItemArray[4].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(4));
                        String UTR_No = dt.Rows[i][5].ToString(); //result.Rows[k].ItemArray[5].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(5));
                        String Entry_Amount = dt.Rows[i][6].ToString(); //result.Rows[k].ItemArray[6].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(6));
                        String IFSC_code = dt.Rows[i][7].ToString(); //result.Rows[k].ItemArray[7].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(7));
                        String DealerVirAccNo = dt.Rows[i][8].ToString();
                        if (Product == "NEFT" || Product == "RTGS" || Product == "FT" || Product == "FUND TRANS")
                        {
                        }
                        else
                        {
                            w.WriteLine(quote + Product + quote + "," + quote + Party_Code + quote + "," + quote + Party_Name + quote + "," + quote + RemittingBank + quote + "," + quote + UTR_No + quote + "," + quote + Entry_Amount + quote + "," + quote + IFSC_code + quote + "," + quote + DealerVirAccNo + quote + "," + quote + "Invalid Payment Type" + quote);


                            EmailBodyCtn = EmailBodyCtn.Append("<br>" + DealerVirAccNo + "," + "Invalid Payment Type");
                            //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "Invalid Payment Type, Please upload correct file again with Payment Type such as NEFT, RTGS, FT or FUND TRANS", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                            rpt = false;
                        }



                        if (DealerVirAccNo != "0")
                        {

                            if (DealerVirAccNo.Length != 23)
                            {


                                w.WriteLine(quote + Product + quote + "," + quote + Party_Code + quote + "," + quote + Party_Name + quote + "," + quote + RemittingBank + quote + "," + quote + UTR_No + quote + "," + quote + Entry_Amount + quote + "," + quote + IFSC_code + quote + "," + quote + DealerVirAccNo + quote + "," + quote + "DO length is less or greater than 23" + quote);
                                EmailBodyCtn = EmailBodyCtn.Append("<br>" + DealerVirAccNo + "," + "DO length is less or greater than 23");
                                //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "DO length is less or greater than 23", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                                rpt = false;
                            }
                            else if (DealerVirAccNo.Length == 23)
                            {

                                //select distinct FType from FinancerDetails

                                DataTable dtabl = Methods.getDetails("GetFinancerCodeDetails", DealerVirAccNo.Substring(22, 1).ToString(), "", "", "", "", "", "", _logger);

                                if (dtabl.Rows.Count == 0)

                                {

                                    w.WriteLine(quote + Product + quote + "," + quote + Party_Code + quote + "," + quote + Party_Name + quote + "," + quote + RemittingBank + quote + "," + quote + UTR_No + quote + "," + quote + Entry_Amount + quote + "," + quote + IFSC_code + quote + "," + quote + DealerVirAccNo + quote + "," + quote + "Product Code is not valid" + quote);
                                    EmailBodyCtn = EmailBodyCtn.Append("<br>" + DealerVirAccNo + "," + "Product Code is not valid");
                                    //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "Product Code is not valid", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                                    rpt = false;

                                }



                            }
                        }


                        //////For Duplicate UTR NO  Added on 23-02-2024
                        DataTable dtUTRDuplicate = Methods.getDetails("GetUTRDetailsForPaymentUpload", UTR_No, "", "", "", "", "", "", _logger);
                        if (dtUTRDuplicate.Rows.Count > 0)
                        {
                            w.WriteLine(quote + Product + quote + "," + quote + Party_Code + quote + "," + quote + Party_Name + quote + "," + quote + RemittingBank + quote + "," + quote + UTR_No + quote + "," + quote + Entry_Amount + quote + "," + quote + IFSC_code + quote + "," + quote + DealerVirAccNo + quote + "," + quote + "UTR NO is Duplicate" + quote);
                            EmailBodyCtn = EmailBodyCtn.Append("<br>" + DealerVirAccNo + "," + "UTR NO is Duplicate");
                            //Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", strFileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "Product Code is not valid", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                            rpt = false;
                        }
                        //////For Duplicate UTR NO  Added on 23-02-2024




                    }
                    w.Close();
                    //w.Flush();
                    if (rpt == false)
                    {
                        Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", filepath, "Rejection of MIS-Payment File", "Please find the attachment.", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                        //Methods.SendEmailstrBld(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", filepath, "Rejection of MIS-Payment File",EmailBodyCtn, HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                    }

                }

            }

            catch (Exception ex)
            {
                TempData["alertMessage"] = ex.Message + " " + ex.StackTrace;
                _logger.LogError(ex.Message + "  " + " - PaymentInformationController;UploadPayment");

            }
            return true;
        }
    }
}

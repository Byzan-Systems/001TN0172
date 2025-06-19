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
    public class PaymentInformationExcelPvdr12Controller : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        public PaymentInformationExcelPvdr12Controller(ILogger<PaymentInformationExcelPvdr12Controller> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public string strFileName = "";
        public string strmessage = "";
        public IActionResult ShowPaymentInformation()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {

                ViewBag.Count = 0;
                ViewBag.Percentage = 0;

                return View();
            }
        }
        [HttpPost]
        public IActionResult UploadPayment(IFormFile upload, int Count)
        {
            string connString;
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                //HSSFWorkbook hssfwb = new HSSFWorkbook(); ISheet sheet;
                try
                {

                    ViewBag.Count = Count + 1;
                    ViewBag.Percentage = 25;

                    //int TotalRows = 0, TotalColumns = 0;
                    Rectify();
                    TempData["alertMessage"] = "Processing.............";
                    if (HttpContext.Request.Form.Files[0].FileName == "")
                    {
                        // MessageBox.Show("Please Select Input File....", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //Txt_FileName.Text = "";
                        //Txt_FileName.Focus();
                    }

                    WaitR:
                    DataTable dt = Methods.getDetails("GetReportDetails", "", "", "", "", "", "", "",_logger);
                    if (dt.Rows.Count > 0)
                    {
                        _logger.LogError("Waiting for Server to free.Please wait for some time.  " + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
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
                        System.IO.File.Delete(Path.Combine(uploads, FileName));
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, FileName), FileMode.Create))
                    {
                        HttpContext.Request.Form.Files[0].CopyToAsync(fileStream);
                        fileStream.Dispose();
                        fileStream.Close();

                    }

                    //using (var fileStream = new FileStream(HttpContext.Request.Form.Files[0].FileName, FileMode.Create))
                    //{
                    //    HttpContext.Request.Form.Files[0].CopyToAsync(fileStream);
                    //    fileStream.Close();
                    //    fileStream.Dispose();
                    
                    //}
                    ////DataTable result;
                    ////connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path.Combine(uploads, FileName) + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    ////OleDbConnection oledbConn = new OleDbConnection(connString);
                    ////DataTable dt1 = new DataTable();
                    ////oledbConn.Open();
                    ////DataTable dt2 = new DataTable();
                    ////dt2 = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    ////string excelSheets = dt2.Rows[0].ItemArray[2].ToString();
                    ////string query = "SELECT * FROM [" + excelSheets + "]";
                    ////using (OleDbCommand cmd = new OleDbCommand(query, oledbConn))
                    ////{
                    ////    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    ////    oleda.SelectCommand = cmd;
                    ////    DataSet ds = new DataSet();
                    ////    oleda.Fill(ds);

                    ////    dt1 = ds.Tables[0];
                    ////    result = dt1;
                    ////}
                    ////oledbConn.Close();

                    ////_logger.LogInformation("Reading completed into table" + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    ////TotalRows = result.Rows.Count;
                    ////TotalColumns = result.Columns.Count;
                    ////_logger.LogInformation("Total Rows count  " + TotalRows + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh

                    //////IRow row = sheet.GetRow(0);

                    //////TotalRows = sheet.LastRowNum;
                    //////TotalColumns = row.LastCellNum;
                    ////string[] Details = new string[20];
                    ////for (int k = 0; k <= TotalRows - 1; k++)
                    ////{
                    ////    if (result.Rows[k] != null)
                    ////    {
                    ////        DataFormatter formatter = new DataFormatter();
                    ////        String Transaction_Id = result.Rows[k].ItemArray[0].ToString();  //formatter.FormatCellValue(sheet.GetRow(k).GetCell(0));
                    ////        String Product = result.Rows[k].ItemArray[1].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(1));
                    ////        String Party_Code = result.Rows[k].ItemArray[2].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(2));
                    ////        String Party_Name = result.Rows[k].ItemArray[3].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(3));
                    ////        String RemittingBank = result.Rows[k].ItemArray[4].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(4));
                    ////        String UTR_No = result.Rows[k].ItemArray[5].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(5));
                    ////        String Entry_Amount = result.Rows[k].ItemArray[6].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(6));
                    ////        String IFSC_code = result.Rows[k].ItemArray[7].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(7));
                    ////        String DealerVirAccNo = result.Rows[k].ItemArray[8].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(8));
                    ////        _logger.LogInformation(DealerVirAccNo + "Assign to variable" + "   - PaymentInformationController;UploadPayment"); ////Enhance by yogesh

                    ////        if (DealerVirAccNo != "0")
                    ////        {

                    ////            if (DealerVirAccNo.Length != 23)
                    ////            {
                    ////                //clserr.WriteLogToTxtFile("Length of Order number = " + sheet.GetRow(k).GetCell(8).StringCellValue.ToUpper() + " is less or more than 23 digits ", "Upload_Click", strFileName);

                    ////                Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "DO length is less or greater than 23", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                    ////                //clserr.WriteLogToTxtFile("Rejection of MIS mail sent to = " + sttg.Payment_Rejection_EMail, "Upload_Click", strFileName);
                    ////                TempData["alertMessage"] = "Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail");
                    ////                _logger.LogError("Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail " + " - PaymentInformationController;UploadPayment"));  ////Enhance by yogesh
                    ////            }
                    ////            else
                    ////            {
                    ////                //clserr.WriteLogToTxtFile("Length of Order number = " + sheet.GetRow(k).GetCell(8).StringCellValue + " is 23 digit", "Upload_Click", strFileName);
                    ////                string Fname = DealerVirAccNo.Substring(DealerVirAccNo.Length - 1, 1);
                    ////                //Condition check for Financier Dealer match with B / C / F / K / V
                    ////                if (Fname == "B" || Fname == "C" || Fname == "F" || Fname == "K" || Fname == "V" || Fname == "D")
                    ////                {
                    ////                    Details[0] = Transaction_Id; //Transaction_Id
                    ////                    Details[1] = "C"; //Type_of_Entry
                    ////                    Details[2] = "C"; //Dr_CR
                    ////                    Details[3] = Entry_Amount; //Entry_Amount
                    ////                    Details[4] = ""; //Value_date
                    ////                    Details[5] = Product; //Product
                    ////                    Details[6] = Party_Code; //Party_Code
                    ////                    Details[7] = Party_Name; //Party_Name
                    ////                    Details[8] = DealerVirAccNo; //VA_account
                    ////                    Details[9] = ""; //Locations
                    ////                    Details[10] = RemittingBank; //RemittingBank
                    ////                    Details[11] = UTR_No; //UTR_No
                    ////                    Details[12] = IFSC_code; //IFSC_code
                    ////                    Details[13] = ""; //Dealer_Name
                    ////                    Details[14] = ""; //Dealer_Account_No
                    ////                    Details[15] = ""; //Releated_Ref_No
                    ////                    Details[16] = dtFile.Rows[0][0].ToString(); //fileID
                    ////                    Details[17] = UserSession.LoginID; //Login ID

                    ////                    string[] detailsCash = new string[25];
                    ////                    detailsCash[0] = "0";   //cashopsID
                    ////                    detailsCash[1] = Details[16];   //FileID
                    ////                    detailsCash[2] = Details[5]; //CashOps_FileType
                    ////                    detailsCash[3] = ""; //NEFT_RTGS_BT_ID
                    ////                    detailsCash[4] = Details[8].Substring(0, 22); //Virtual_Account
                    ////                    detailsCash[5] = Details[11]; //UTR_No
                    ////                    detailsCash[6] = Details[3]; //Transaction_Amount
                    ////                    detailsCash[7] = "C"; //Transaction_status
                    ////                    detailsCash[8] = DBNull.Value.ToString(); //Payment_Status
                    ////                    detailsCash[9] = DBNull.Value.ToString(); //Attempt
                    ////                    detailsCash[10] = System.DateTime.Now.ToString("DD/MMM/YYYY"); //Cash_Ops_Date
                    ////                    detailsCash[11] = System.DateTime.Now.ToString("HH:MM:SS AMPM"); //Cash_Ops_Time
                    ////                    detailsCash[12] = "0"; //GEFO_Flag
                    ////                    detailsCash[13] = DBNull.Value.ToString(); //GEFO_Date
                    ////                    detailsCash[14] = DBNull.Value.ToString(); //DR_Account_No
                    ////                    detailsCash[15] = DBNull.Value.ToString(); //CR_Account_No
                    ////                    detailsCash[16] = UserSession.LoginID; //LoginID
                    ////                    detailsCash[17] = DBNull.Value.ToString(); //EOD_MailFlag
                    ////                    detailsCash[18] = DBNull.Value.ToString(); //DRC_Generation
                    ////                    detailsCash[19] = Details[8]; //FNCR_Virtual_Account
                    ////                    detailsCash[20] = Details[12]; //IFSC_code
                    ////                    detailsCash[21] = Details[10]; //FNCR_code
                    ////                    detailsCash[22] = ""; //FNCR_Name

                    ////                    //clserr.WriteLogToTxtFile("In Cashops_Payment Function", "Upload_Click", strFileName);
                    ////                    Methods.CashOps_Payments(Details, detailsCash,_logger);
                    ////                    //clserr.WriteLogToTxtFile("End -Cashops_Payment Function", "Upload_Click", strFileName);
                    ////                    TempData["alertMessage"] = "File Successfully Uploaded. Total Data " + TotalRows + " Successfully upload"; ////Enhance by yogesh
                    ////                    _logger.LogInformation("File Successfully Uploaded. Record count " + TotalRows + " Successfully upload" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    ////                }
                    ////                else
                    ////                {
                    ////                    //clserr.WriteLogToTxtFile("Financier Dealer Name does not match with B/C/F/K/V = " + sheet.GetRow(k).GetCell(8).StringCellValue, "Upload_Click", strFileName);
                    ////                    Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Mismatched Dealer Financier Name", DealerVirAccNo + "-" + "Mismatched Dealer Financier Name", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"), _logger);
                    ////                    //clserr.WriteLogToTxtFile("Mismatched Fin Dealer mail sent to - " + sttg.Payment_Rejection_EMail, "Upload_Click", strFileName);
                    ////                    TempData["alertMessage"] = "Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail");

                    ////                    _logger.LogError("Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail"));  ////Enhance by yogesh
                    ////                }

                    ////            }
                    ////        }
                    ////    }

                    ////    //if (sheet.GetRow(k) != null) //null is when the row only contains empty cells 
                    ////    //{


                    ////    //}
                    ////}

                    //using (FileStream file = new FileStream(HttpContext.Request.Form.Files[0].FileName, FileMode.Open, FileAccess.Read))
                    //{
                    //    string connString ="";
                    //    string path1 = Path.GetFileName(HttpContext.Request.Form.Files[0].FileName); //HttpContext.Request.Form.Files[0].FileName;

                    //    //string fileLocation = @"F:\MSIL NEW APPLICATION\SERVER APPLICAIOTN\LIVE\BackupFile";//Microsoft.AspNetCore.Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName; //this.Environment.ContentRootPath;
                    //    string fileLocation = Path.GetFullPath(HttpContext.Request.Form.Files[0].FileName); //this.Environment.ContentRootPath;
                    //    DataTable result;
                    //    //hssfwb = new HSSFWorkbook(file);

                    //    // adding by Dipak mali 23-05-2023  new code for read xls or xlsx  file
                    //    var fileExt = Path.GetExtension(Path.GetFileName(HttpContext.Request.Form.Files[0].FileName));
                    //    //if (fileExt.ToString().ToUpper() == ".xls".ToString().ToUpper())
                    //    //{
                    //    //    HSSFWorkbook hssfwb1 = new HSSFWorkbook(file); //HSSWorkBook object will read the Excel 97-2000 formats  
                    //    //                                                   // excelSheet = hssfwb1.GetSheetAt(0); //get first Excel sheet from workbook  
                    //    //    sheet = hssfwb1.GetSheet(hssfwb1.GetSheetName(0));
                    //    //}
                    //    //else
                    //    //{
                    //    //    XSSFWorkbook hssfwb1 = new XSSFWorkbook(file); //XSSFWorkBook will read 2007 Excel format  
                    //    //                                                   //excelSheet = hssfwb1.GetSheetAt(0); //get first Excel sheet from workbook   
                    //    //    sheet = hssfwb1.GetSheet(hssfwb1.GetSheetName(0));
                    //    //}
                    //    //------

                    //    //connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + "; Extended Properties = Excel 12.0";

                    //    _logger.LogInformation(connString + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    //    //if (fileExt.ToString().ToUpper() == ".xls".ToString().ToUpper())
                    //    //{
                    //    //    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + "\\" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    //    //}
                    //    //else if (fileExt.ToString().ToUpper() == ".xlsx".ToString().ToUpper())
                    //    //{
                    //    //    connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + "\\" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //    //}

                    //    //result = Methods.ImportExceltoDatabase(path1, connString);

                    //    OleDbConnection oledbConn = new OleDbConnection(connString);
                    //    DataTable dt1 = new DataTable();
                    //    if (oledbConn.State == ConnectionState.Closed)
                    //    {
                    //        oledbConn.Open();
                    //        _logger.LogInformation("Connection open successfully" + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    //    }
                    //    else
                    //    {
                    //        oledbConn.Close();
                    //        oledbConn.Open();
                    //        _logger.LogInformation("Connection open successfully" + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    //    }
                    //    //_logger.LogInformation("Connection open Successfully" + "   - Methods");
                    //    using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn))
                    //    {
                    //        OleDbDataAdapter oleda = new OleDbDataAdapter();
                    //        oleda.SelectCommand = cmd;
                    //        DataSet ds = new DataSet();
                    //        oleda.Fill(ds);

                    //        result = ds.Tables[0];

                    //    }



                    //    //result = dt1;



                    //    _logger.LogInformation("Reading completed into table" + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    //    TotalRows = result.Rows.Count;
                    //    TotalColumns = result.Columns.Count;
                    //    _logger.LogInformation("Total Rows count  " + TotalRows + "   - PaymentInformationController;UploadPayment");  ////Enhance by yogesh

                    //    //IRow row = sheet.GetRow(0);

                    //    //TotalRows = sheet.LastRowNum;
                    //    //TotalColumns = row.LastCellNum;
                    //    string[] Details = new string[20];
                    //    for (int k = 0; k <= TotalRows - 1; k++)
                    //    {
                    //        if (result.Rows[k] != null)
                    //        {
                    //            DataFormatter formatter = new DataFormatter();
                    //            String Transaction_Id = result.Rows[k].ItemArray[0].ToString();  //formatter.FormatCellValue(sheet.GetRow(k).GetCell(0));
                    //            String Product = result.Rows[k].ItemArray[1].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(1));
                    //            String Party_Code = result.Rows[k].ItemArray[2].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(2));
                    //            String Party_Name = result.Rows[k].ItemArray[3].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(3));
                    //            String RemittingBank = result.Rows[k].ItemArray[4].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(4));
                    //            String UTR_No = result.Rows[k].ItemArray[5].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(5));
                    //            String Entry_Amount = result.Rows[k].ItemArray[6].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(6));
                    //            String IFSC_code = result.Rows[k].ItemArray[7].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(7));
                    //            String DealerVirAccNo = result.Rows[k].ItemArray[8].ToString(); //formatter.FormatCellValue(sheet.GetRow(k).GetCell(8));


                    //            if (DealerVirAccNo != "0")
                    //            {

                    //                if (DealerVirAccNo.Length != 23)
                    //                {
                    //                    //clserr.WriteLogToTxtFile("Length of Order number = " + sheet.GetRow(k).GetCell(8).StringCellValue.ToUpper() + " is less or more than 23 digits ", "Upload_Click", strFileName);

                    //                    Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Rejection of MIS-Payment File", DealerVirAccNo + "-" + "DO length is less or greater than 23", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                    //                    //clserr.WriteLogToTxtFile("Rejection of MIS mail sent to = " + sttg.Payment_Rejection_EMail, "Upload_Click", strFileName);
                    //                    TempData["alertMessage"] = "Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail");
                    //                    _logger.LogError("Rejection of MIS mail sent to = " + HttpContext.Session.GetString("Payment_Rejection_EMail " + " - PaymentInformationController;UploadPayment"));  ////Enhance by yogesh
                    //                }
                    //                else
                    //                {
                    //                    //clserr.WriteLogToTxtFile("Length of Order number = " + sheet.GetRow(k).GetCell(8).StringCellValue + " is 23 digit", "Upload_Click", strFileName);
                    //                    string Fname = DealerVirAccNo.Substring(DealerVirAccNo.Length - 1, 1);
                    //                    //Condition check for Financier Dealer match with B / C / F / K / V
                    //                    if (Fname == "B" || Fname == "C" || Fname == "F" || Fname == "K" || Fname == "V")
                    //                    {
                    //                        Details[0] = Transaction_Id; //Transaction_Id
                    //                        Details[1] = "C"; //Type_of_Entry
                    //                        Details[2] = "C"; //Dr_CR
                    //                        Details[3] = Entry_Amount; //Entry_Amount
                    //                        Details[4] = ""; //Value_date
                    //                        Details[5] = Product; //Product
                    //                        Details[6] = Party_Code; //Party_Code
                    //                        Details[7] = Party_Name; //Party_Name
                    //                        Details[8] = DealerVirAccNo; //VA_account
                    //                        Details[9] = ""; //Locations
                    //                        Details[10] = RemittingBank; //RemittingBank
                    //                        Details[11] = UTR_No; //UTR_No
                    //                        Details[12] = IFSC_code; //IFSC_code
                    //                        Details[13] = ""; //Dealer_Name
                    //                        Details[14] = ""; //Dealer_Account_No
                    //                        Details[15] = ""; //Releated_Ref_No
                    //                        Details[16] = dtFile.Rows[0][0].ToString(); //fileID
                    //                        Details[17] =  UserSession.LoginID; //Login ID

                    //                        string[] detailsCash = new string[25];
                    //                        detailsCash[0] = "0";   //cashopsID
                    //                        detailsCash[1] = Details[16];   //FileID
                    //                        detailsCash[2] = Details[5]; //CashOps_FileType
                    //                        detailsCash[3] = ""; //NEFT_RTGS_BT_ID
                    //                        detailsCash[4] = Details[8].Substring(0, 22); //Virtual_Account
                    //                        detailsCash[5] = Details[11]; //UTR_No
                    //                        detailsCash[6] = Details[3]; //Transaction_Amount
                    //                        detailsCash[7] = "C"; //Transaction_status
                    //                        detailsCash[8] = DBNull.Value.ToString(); //Payment_Status
                    //                        detailsCash[9] = DBNull.Value.ToString(); //Attempt
                    //                        detailsCash[10] = System.DateTime.Now.ToString("DD/MMM/YYYY"); //Cash_Ops_Date
                    //                        detailsCash[11] = System.DateTime.Now.ToString("HH:MM:SS AMPM"); //Cash_Ops_Time
                    //                        detailsCash[12] = "0"; //GEFO_Flag
                    //                        detailsCash[13] = DBNull.Value.ToString(); //GEFO_Date
                    //                        detailsCash[14] = DBNull.Value.ToString(); //DR_Account_No
                    //                        detailsCash[15] = DBNull.Value.ToString(); //CR_Account_No
                    //                        detailsCash[16] =  UserSession.LoginID; //LoginID
                    //                        detailsCash[17] = DBNull.Value.ToString(); //EOD_MailFlag
                    //                        detailsCash[18] = DBNull.Value.ToString(); //DRC_Generation
                    //                        detailsCash[19] = Details[8]; //FNCR_Virtual_Account
                    //                        detailsCash[20] = Details[12]; //IFSC_code
                    //                        detailsCash[21] = Details[10]; //FNCR_code
                    //                        detailsCash[22] = ""; //FNCR_Name

                    //                        //clserr.WriteLogToTxtFile("In Cashops_Payment Function", "Upload_Click", strFileName);
                    //                        Methods.CashOps_Payments(Details, detailsCash);
                    //                        //clserr.WriteLogToTxtFile("End -Cashops_Payment Function", "Upload_Click", strFileName);
                    //                        TempData["alertMessage"] = "File Successfully Uploaded. Total Data " + TotalRows + " Successfully upload"; ////Enhance by yogesh
                    //                        _logger.LogInformation("File Successfully Uploaded. Record count " + TotalRows + " Successfully upload" + " - PaymentInformationController;UploadPayment");  ////Enhance by yogesh
                    //                    }
                    //                    else
                    //                    {
                    //                        //clserr.WriteLogToTxtFile("Financier Dealer Name does not match with B/C/F/K/V = " + sheet.GetRow(k).GetCell(8).StringCellValue, "Upload_Click", strFileName);
                    //                        Methods.SendEmail(HttpContext.Session.GetString("Payment_Rejection_EMail"), "", "", HttpContext.Request.Form.Files[0].FileName, "Mismatched Dealer Financier Name", DealerVirAccNo + "-" + "Mismatched Dealer Financier Name", HttpContext.Session.GetString("Email_FromID").ToString(), HttpContext.Session.GetString("PWD").ToString(), HttpContext.Session.GetString("SMTP_HOST"), HttpContext.Session.GetString("Port"));
                    //                        //clserr.WriteLogToTxtFile("Mismatched Fin Dealer mail sent to - " + sttg.Payment_Rejection_EMail, "Upload_Click", strFileName);
                    //                        TempData["alertMessage"] = "Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail");

                    //                        _logger.LogError("Mismatched Fin Dealer mail sent to - " + HttpContext.Session.GetString("Payment_Rejection_EMail"));  ////Enhance by yogesh
                    //                    }

                    //                }
                    //            }
                    //        }

                    //        //if (sheet.GetRow(k) != null) //null is when the row only contains empty cells 
                    //        //{


                    //        //}
                    //    }


                    //    //Txt_FileName.Text = "";
                    //    //Txt_FileName.Focus();

                    //    //hssfwb1.Close();
                    //    //hssfwb1.Dispose();
                    //    sheet = null;
                    //    file.Close();
                    //    file.Dispose();
                    //    System.IO.File.Delete(HttpContext.Request.Form.Files[0].FileName);
                    //}
                }
                catch (Exception ex)
                {
                    TempData["alertMessage"] = ex.Message;
                    //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Upload_Click");

                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //hssfwb1.Close();
                    //sheet = null;


                }
                return RedirectToAction("ShowPaymentInformation");
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
                        DataTable dt_Up = Methods.UpdateDetails("Update_OrderInv_Details", dt.Rows[i]["Cash_Ops_ID"].ToString(), dt.Rows[i]["Virtual_Account"].ToString(), dt.Rows[i]["UTR_No"].ToString(), dt.Rows[i]["Order_ID"].ToString(), "", "", "", _logger);
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
                _logger.LogError( ex.Message + "  PaymentInformationExcelPvdr12Controller;Rectify");

                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Rectify");
            }
        }
    }
}

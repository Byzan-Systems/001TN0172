using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using HDFCMSILWebMVC.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace HDFCMSILWebMVC.Controllers
{
    public class UploadReceivedInvoiceDataController : Controller
    {
        private static Dictionary<string, int> _uploadProgress = new Dictionary<string, int>();

        [Obsolete]
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UploadReceivedInvoiceDataController> _logger;
        private readonly IExcelService _excelService;
        private readonly DataService _dataService;
        private readonly IHubContext<UploadProgressHub> _hubContext;

        [Obsolete]
        public UploadReceivedInvoiceDataController(DataService dataService, IWebHostEnvironment hostingEnvironment, ILogger<UploadReceivedInvoiceDataController> logger, IExcelService excelService, IHubContext<UploadProgressHub> hubContext)
        {

            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _excelService = excelService;
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _hubContext = hubContext;
        }
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> UploadDetailsUpdated(IFormFile txtFile)
        {

            var sessionId = HttpContext.Session.Id;
            TempData["alertMessage"] = "";
            TempData["successMessage"] = "";
            TempData["alertMessage"] = "";

            var validationErrors = new List<string>();
            //var processingMessages = new List<string>(); // List to store processing messages
            if (txtFile == null || txtFile.Length == 0)
            {
                TempData["alertMessage"] = "No file selected for upload.";
                return RedirectToAction("ViewUploadReceivedInvoiceData");
            }

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Files", txtFile.FileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var file = HttpContext.Request.Form.Files[0];
                    await file.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
                // Initial processing progress
                int progress = 10;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");


                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                //DataTable dataTable = await _excelService.ReadExcelToDataTableAsync(filePath);

                IWorkbook workbook;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (Path.GetExtension(filePath).ToLower() == ".xls")
                    {
                        workbook = new HSSFWorkbook(fileStream); // for .xls files
                    }
                    else if (Path.GetExtension(filePath).ToLower() == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fileStream); // for .xlsx files
                    }
                    else
                    {
                        throw new Exception("Invalid file extension");
                    }
                }

                ISheet sheet = workbook.GetSheetAt(0); // Get the first sheet

                if (sheet == null)
                {
                    throw new Exception("No sheets found in the Excel file.");
                }

                // Loop through the first row and add columns to DataTable
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                //for (int i = 0; i < cellCount; i++)
                //{
                //    dataTable.Columns.Add(headerRow.GetCell(i).ToString());
                //}

                progress = 20;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");
                int fileId = await _excelService.InsertFileDescAsync(txtFile.FileName, "IR", DateTime.Now.ToString("dd/MM/yyyy"), sheet.LastRowNum, "2");
                _logger.LogError($"File ID " + fileId + " created ");

                // File reading progress

                // File description insertion progress
                progress = 30;
                int FailureCount = 0;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");
                // Populate DataTable with data from Excel rows
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    var formatter = new DataFormatter();
                    IRow row = sheet.GetRow(i);
                    string SrNo = GetSafeCellValue(row, 0);
                    string InvoiceNumber = GetSafeCellValue(row, 1);
                    string InvoiceAmount = GetSafeCellValue(row, 2);
                    string Currency = GetSafeCellValue(row, 3);
                    string VehicleId = GetSafeCellValue(row, 4);
                    string DueDate = GetSafeCellValue(row, 5);
                    string DealerName = GetSafeCellValue(row, 6);
                    string DealerAddress1 = GetSafeCellValue(row, 7);
                    string DealerCity = GetSafeCellValue(row, 8);
                    string TransporterName = GetSafeCellValue(row, 9);
                    string TransportNumber = GetSafeCellValue(row, 10);
                    string TransportDate = GetSafeCellValue(row, 11);
                    string DealerCode = GetSafeCellValue(row, 12);
                    string TransporterCode = GetSafeCellValue(row, 13);
                    string DealerAddress2 = GetSafeCellValue(row, 14);
                    string DealerAddress3 = GetSafeCellValue(row, 15);
                    string DealerAddress4 = GetSafeCellValue(row, 16);
                    string TradeRefNo = GetSafeCellValue(row, 17);
                    string PhysicalReceivedDate = GetSafeCellValue(row, 18);
                    string Remarks = GetSafeCellValue(row, 19);

                    bool isValid = true; 
                    // Validate TradeRefNo
                    if (!string.IsNullOrWhiteSpace(TradeRefNo))
                    {
                        var tradeRefNo = TradeRefNo.Trim();

                        if (tradeRefNo.Length > 4)
                        {
                            if (tradeRefNo.Length != 16)
                            {
                                validationErrors.Add($"Trade Reference Number length should be 16 for Invoice {InvoiceNumber}.");
                                _logger.LogError($"Trade Reference Number length should be sixteen for Invoice {InvoiceNumber}.");
                                isValid = false;
                            }

                            if (!tradeRefNo.StartsWith("027BC53", StringComparison.OrdinalIgnoreCase))
                            {
                                validationErrors.Add($"Trade Reference Number is not valid for Invoice {InvoiceNumber}.");
                                _logger.LogError($"Trade Reference Number is not valid for Invoice {InvoiceNumber}.");
                                isValid = false;
                            }

                            var query = "Select * from Invoice where Invoice_Number='" + InvoiceNumber.ToString().Trim() + "' and TradeOp_Selected_Invoice_Flag = 1";
                            DataTable dtInvoice = _dataService.GetDataTable(query);

                            if (dtInvoice.Rows.Count > 0)
                            {
                                var query1 = "select * from Invoice_Received where { fn ucase(Invoice_Number)}='" + InvoiceNumber.ToString().Trim() + "'";
                                DataTable dtInvoiceRec = _dataService.GetDataTable(query1);

                                if (dtInvoiceRec.Rows.Count > 0)
                                {
                                    validationErrors.Add($"Duplicate Invoice Number {InvoiceNumber} found in Invoice Receive table- UploadReceivedInvoiceDataController;UploadDetails");
                                    _logger.LogError($"Duplicate Invoice Number {InvoiceNumber} found in Invoice Receive table- UploadReceivedInvoiceDataController;UploadDetails");
                                    isValid = false;
                                }
                                else
                                {
                                    DateTime phy_rec_Date = DateTime.ParseExact(PhysicalReceivedDate, "dd/MM/yyyy", null);

                                    string query2 = @"UPDATE Invoice SET TradeopsFileID = @Search1,F7_MIS = 0,LoginID_TradeOps = @Search2,Invoice_Status = 'PHYSICAL INV REC',IMEX_DEAL_NUMBER = @Search3,StepDate = @Search4,Trade_OPs_Remarks = @Search5 WHERE Invoice_Number = @Search6";
                                    if (HttpContext.Session.GetString("LoginID") != null)
                                    { UserSession.LoginID = HttpContext.Session.GetString("LoginID"); }
                                    var parameters = new[]
                                {
                                        new Microsoft.Data.SqlClient.SqlParameter("@Search1", fileId), new Microsoft.Data.SqlClient.SqlParameter("@Search2", UserSession.LoginID.ToString()),new Microsoft.Data.SqlClient.SqlParameter("@Search3", tradeRefNo),new Microsoft.Data.SqlClient.SqlParameter("@Search4", phy_rec_Date.ToString("yyyy-MM-dd").Trim()),new Microsoft.Data.SqlClient.SqlParameter("@Search5", Remarks),new Microsoft.Data.SqlClient.SqlParameter("@Search6", InvoiceNumber)
                                    };
                                    int rowsAffected = _dataService.ExecuteUpdateQuery(query2, parameters);
                                    if (rowsAffected > 0)
                                    {
                                        //  validationErrors.Add($"Successfully updated InvoiceFCC_Details for Invoice Number {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        _logger.LogInformation($"Successfully updated InvoiceFCC_Details for Invoice Number {InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                    }
                                    else
                                    {
                                        validationErrors.Add($"Error InvoiceFCC_Details update for Invoice Number {InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        _logger.LogError($"Error InvoiceFCC_Details update for Invoice Number {InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        isValid = false;
                                    }
                                    //if (string.IsNullOrEmpty(dtInvoice.Rows[0]["IMEX_DEAL_NUMBER"].ToString()))
                                    //{
                                    //    _logger.LogError($"Value of imex_deal_number not updated for InvoiceNo  {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");

                                    //}
                                }
                                //
                                DataTable dtInvoice1 = _dataService.GetDataTable(query);
                                if (string.IsNullOrEmpty(dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString()))
                                {
                                    validationErrors.Add($"Value of imex_deal_number=" + dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString() + "and StepDate =" + dtInvoice1.Rows[0]["stepdate"].ToString() + " not updated for InvoiceNo -" + InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");
                                    _logger.LogError("Value of imex_deal_number=" + dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString() + "and StepDate =" + dtInvoice1.Rows[0]["stepdate"].ToString() + " not updated for InvoiceNo -" + InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");
                                    isValid = false;
                                }
                            }
                            else
                            {
                                validationErrors.Add($"Physical Invoice Data Not Received. Invoice number not found in Invoice table. Invoice Number: {InvoiceNumber} - UploadReceivedInvoiceDataController; UploadDetails");
                                _logger.LogError("Physical Invoice Data Not Received. Invoice number not found in Invoice table. Invoice Number : " + InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");

                                isValid = false;
                            }
                        }
                        else
                        {
                            validationErrors.Add($"Trade Reference Number length is more than 4 characters for Invoice {InvoiceNumber}.");
                            _logger.LogError($"Trade Reference Number length is more than 4 characters for Invoice {InvoiceNumber}.");
                            isValid = false;
                        }
                    }
                    else
                    {
                        validationErrors.Add($"Trade Reference Number is required for Invoice Number {InvoiceNumber}.");
                        _logger.LogError($"Trade Reference Number is required for Invoice Number {InvoiceNumber}.");
                        isValid = false;

                    }
                    //
                    if (isValid == true)
                    {

                        string[] detailsCash = new string[25];
                        detailsCash[0] = "0";   //@Invoice_Rec_ID
                        detailsCash[1] = SrNo; //Sr_No
                        detailsCash[2] = InvoiceNumber; //Invoice_Number
                        detailsCash[3] = InvoiceAmount; //Invoice_Amount
                        detailsCash[4] = Currency; //Currency
                        detailsCash[5] = VehicleId;   //Vehical_ID
                        detailsCash[6] = DueDate;   //DueDate
                        detailsCash[7] = DealerName;   //Dealer_Name
                        detailsCash[8] = DealerAddress1;   //Dealer_Address1
                        detailsCash[9] = DealerCity;   //Dealer_City
                        detailsCash[10] = TransporterName;   //Transporter_Name
                        detailsCash[11] = TransportNumber;   //Transport_Number
                        detailsCash[12] = TransportDate;   //Transport_Date
                        detailsCash[13] = DealerCode;   //Dealer_Code
                        detailsCash[14] = TransporterCode;   //Transporter_Code
                        detailsCash[15] = DealerAddress2;   //Dealer_Address2
                        detailsCash[16] = DealerAddress3;   //Dealer_Address3
                        detailsCash[17] = DealerAddress4;   //Dealer_Address4
                        detailsCash[18] = TradeRefNo;   //Trade_RefNo
                        detailsCash[19] = PhysicalReceivedDate;   //Physical_Received_Date
                        detailsCash[20] = Remarks;   //Remarks
                        detailsCash[21] = UserSession.LoginID;   //LoginID
                        DataTable dsUpdate = Methods.Insert_InvoiceRecievedDetails("Save", detailsCash, _logger);
                    }
                    else
                    {
                        FailureCount = FailureCount + 1;
                    }

                    progress = 40 + (int)(((double)i / sheet.LastRowNum) * 40);
                    _uploadProgress[sessionId] = progress;

                    // Send progress update to SignalR hub
                    await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, $" {progress}% ");
                }
                // Processing completed
                progress = 80;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");

                _logger.LogInformation("FCC File Uploaded Successfully and Total Records :  " + sheet.LastRowNum);// + " and Data Succesfully Converted : " + cnt + " - UploadReceivedInvoiceDataController;UploadDetails");
                TempData["alertMessage"] = "FCC File Uploaded Successfully and Total Records :  " + sheet.LastRowNum;

                string str = UserSession.LoginID;
                string query23 = @"UPDATE File_Desc SET FCC_Mail=@Search2 where FileID=@Search1";

                var parameters1 = new[]
                {
                     new Microsoft.Data.SqlClient.SqlParameter("@Search2", "0"), new Microsoft.Data.SqlClient.SqlParameter("@Search1", fileId)
                    };
                int rowsAffected1 = _dataService.ExecuteUpdateQuery(query23, parameters1);
                if (rowsAffected1 > 0)
                    _logger.LogInformation("File_Desc Table updated successfully");
                else
                    _logger.LogInformation("There is some issue when updating File_Desc table. Query : " + query23);
                if (FailureCount > 0)
                {
                    _logger.LogInformation("FCC File uploading Failed Due to some Discrepancies : Failure Record : -" + FailureCount);
                    //  TempData["alertMessage"] = JsonConvert.SerializeObject(validationErrors);
                    //TempData["alertMessage"] = null;
                    TempData["alertMessage"] = "FCC File uploading Failed Due to some Discrepancies, Please check audit log: Failure Record : -" + FailureCount;

                }
                // Final completion progress
                progress = 100;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "Upload process completed");
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Error occurred while uploading invoice details.");
                _logger.LogError(ex, "Error occurred while uploading invoice details.");
                TempData["alertMessage"] = "An error occurred while processing the file.";
            }
            finally
            {
                _uploadProgress.Remove(sessionId);
            }
            return RedirectToAction("ViewUploadReceivedInvoiceData");

        }
        public static string GetSafeCellValue(IRow row, int columnIndex)
        {
            var cell = row.GetCell(columnIndex);
            if (cell == null)
                return string.Empty;

            DataFormatter formatter = new DataFormatter();
            return formatter.FormatCellValue(cell);
        }
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> UploadDetails(IFormFile txtFile)
        {

            var sessionId = HttpContext.Session.Id;
            TempData["alertMessage"] = "";
            TempData["successMessage"] = "";
            TempData["alertMessage"] = "";

            var validationErrors = new List<string>();
            //var processingMessages = new List<string>(); // List to store processing messages
            if (txtFile == null || txtFile.Length == 0)
            {
                TempData["alertMessage"] = "No file selected for upload.";
                return RedirectToAction("ViewUploadReceivedInvoiceData");
            }

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Files", txtFile.FileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var file = HttpContext.Request.Form.Files[0];
                    await file.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
                // Initial processing progress
                int progress = 10;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");


                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                DataTable dataTable = await _excelService.ReadExcelToDataTableAsync(filePath);

                // File reading progress
                progress = 20;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");



                string fileName = txtFile.FileName;
                string fileType = "IR";
                string fileDate = DateTime.Now.ToString("dd/MM/yyyy");
                int srNo = dataTable.Rows.Count;
                string fccMail = "2"; // Example value
                int fileId = await _excelService.InsertFileDescAsync(fileName, fileType, fileDate, srNo, fccMail);
                _logger.LogError($"File ID " + fileId + " created ");
                // File description insertion progress
                progress = 30;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");

                DataTable successRecords = dataTable.Clone();
                DataTable failureRecords = dataTable.Clone();

                //var successRecords = new List<InvoiceData>();
                //var failureRecords = new List<InvoiceData>();

                List<InvoiceData> invoices = dataTable.AsEnumerable()
             .Select(row => new InvoiceData
             {
                 SrNo = row.Field<string>(0),
                 InvoiceNumber = row.Field<string>(1),
                 InvoiceAmount = row.Field<string>(2),
                 Currency = row.Field<string>(3),
                 VehicleId = row.Field<string>(4),
                 DueDate = row.Field<string>(5),
                 DealerName = row.Field<string>(6),
                 DealerAddress1 = row.Field<string>(7),
                 DealerCity = row.Field<string>(8),
                 TransporterName = row.Field<string>(9),
                 TransportNumber = row.Field<string>(10),
                 TransportDate = row.Field<string>(11),
                 DealerCode = row.Field<string>(12),
                 TransporterCode = row.Field<string>(13),
                 DealerAddress2 = row.Field<string>(14),
                 DealerAddress3 = row.Field<string>(15),
                 DealerAddress4 = row.Field<string>(16),
                 TradeRefNo = row.Field<string>(17),
                 PhysicalReceivedDate = row.Field<string>(18),
                 Remarks = row.Field<string>(19)
             })
             .ToList();
                _logger.LogError($"Reading FCC File Completed.");
                // Initial processing stage completed
                progress = 40;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");


                // Processing each row from the DataTable
                int recordCount = 1;
                foreach (var record in invoices)
                {
                    bool isValid = true;


                    // Validate TradeRefNo
                    if (!string.IsNullOrWhiteSpace(record.TradeRefNo))
                    {
                        var tradeRefNo = record.TradeRefNo.Trim();

                        if (tradeRefNo.Length > 4)
                        {
                            if (tradeRefNo.Length != 16)
                            {
                                 validationErrors.Add($"Trade Reference Number length should be 16 for Invoice {record.InvoiceNumber}.");
                                _logger.LogError($"Trade Reference Number length should be sixteen for Invoice {record.InvoiceNumber}.");
                                isValid = false;
                            }

                            if (!tradeRefNo.StartsWith("027BC53", StringComparison.OrdinalIgnoreCase))
                            {
                                validationErrors.Add($"Trade Reference Number is not valid for Invoice {record.InvoiceNumber}.");
                                _logger.LogError($"Trade Reference Number is not valid for Invoice {record.InvoiceNumber}.");
                                isValid = false;
                            }

                            var query = "Select IMEX_DEAL_NUMBER,stepdate from Invoice where Invoice_Number='" + record.InvoiceNumber.ToString().Trim() + "' and TradeOp_Selected_Invoice_Flag = 1";
                            DataTable dtInvoice = _dataService.GetDataTable(query);

                            if (dtInvoice.Rows.Count > 0)
                            {
                                var query1 = "select Invoice_Number from Invoice_Received where { fn ucase(Invoice_Number)}='" + record.InvoiceNumber.ToString().Trim() + "'";
                                DataTable dtInvoiceRec = _dataService.GetDataTable(query1);

                                if (dtInvoiceRec.Rows.Count > 0)
                                {
                                    validationErrors.Add($"Duplicate Invoice Number {record.InvoiceNumber} found in Invoice Receive table- UploadReceivedInvoiceDataController;UploadDetails");
                                    _logger.LogError($"Duplicate Invoice Number {record.InvoiceNumber} found in Invoice Receive table- UploadReceivedInvoiceDataController;UploadDetails");
                                    isValid = false;
                                }
                                else
                                {
                                    DateTime phy_rec_Date = DateTime.ParseExact(record.PhysicalReceivedDate, "dd/MM/yyyy", null);

                                    string query2 = @"UPDATE Invoice SET TradeopsFileID = @Search1,F7_MIS = 0,LoginID_TradeOps = @Search2,Invoice_Status = 'PHYSICAL INV REC',IMEX_DEAL_NUMBER = @Search3,StepDate = @Search4,Trade_OPs_Remarks = @Search5 WHERE Invoice_Number = @Search6";
                                    if (HttpContext.Session.GetString("LoginID") != null)
                                    { UserSession.LoginID = HttpContext.Session.GetString("LoginID"); }
                                    var parameters = new[]
                                {
                                        new Microsoft.Data.SqlClient.SqlParameter("@Search1", fileId), new Microsoft.Data.SqlClient.SqlParameter("@Search2", UserSession.LoginID.ToString()),new Microsoft.Data.SqlClient.SqlParameter("@Search3", tradeRefNo),new Microsoft.Data.SqlClient.SqlParameter("@Search4", phy_rec_Date.ToString("yyyy-MM-dd").Trim()),new Microsoft.Data.SqlClient.SqlParameter("@Search5", record.Remarks),new Microsoft.Data.SqlClient.SqlParameter("@Search6", record.InvoiceNumber)
                                    };
                                    int rowsAffected = _dataService.ExecuteUpdateQuery(query2, parameters);
                                    if (rowsAffected > 0)
                                    {
                                        //  validationErrors.Add($"Successfully updated InvoiceFCC_Details for Invoice Number {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        _logger.LogInformation($"Successfully updated InvoiceFCC_Details for Invoice Number {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                    }
                                    else
                                    {
                                        validationErrors.Add($"Error InvoiceFCC_Details update for Invoice Number {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        _logger.LogError($"Error InvoiceFCC_Details update for Invoice Number {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");
                                        isValid = false;
                                    }
                                    //if (string.IsNullOrEmpty(dtInvoice.Rows[0]["IMEX_DEAL_NUMBER"].ToString()))
                                    //{
                                    //    _logger.LogError($"Value of imex_deal_number not updated for InvoiceNo  {record.InvoiceNumber} - UploadReceivedInvoiceDataController;UploadDetails");

                                    //}
                                }
                                //
                                DataTable dtInvoice1 = _dataService.GetDataTable(query);
                                if (string.IsNullOrEmpty(dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString()))
                                {
                                    validationErrors.Add($"Value of imex_deal_number=" + dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString() + "and StepDate =" + dtInvoice1.Rows[0]["stepdate"].ToString() + " not updated for InvoiceNo -" + record.InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");
                                    _logger.LogError("Value of imex_deal_number=" + dtInvoice1.Rows[0]["IMEX_DEAL_NUMBER"].ToString() + "and StepDate =" + dtInvoice1.Rows[0]["stepdate"].ToString() + " not updated for InvoiceNo -" + record.InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");
                                    isValid = false;
                                }
                            }
                            else
                            {
                                validationErrors.Add($"Physical Invoice Data Not Received. Invoice number not found in Invoice table. Invoice Number: {record.InvoiceNumber} - UploadReceivedInvoiceDataController; UploadDetails");
                                _logger.LogError("Physical Invoice Data Not Received. Invoice number not found in Invoice table. Invoice Number : " + record.InvoiceNumber + " - UploadReceivedInvoiceDataController;UploadDetails");

                                isValid = false;
                            }
                        }
                        else
                        {
                            validationErrors.Add($"Trade Reference Number length is more than 4 characters for Invoice {record.InvoiceNumber}.");
                            _logger.LogError($"Trade Reference Number length is more than 4 characters for Invoice {record.InvoiceNumber}.");
                            isValid = false;
                        }
                    }
                    else
                    {
                        validationErrors.Add($"Trade Reference Number is required for Invoice Number {record.InvoiceNumber}.");
                        _logger.LogError($"Trade Reference Number is required for Invoice Number {record.InvoiceNumber}.");
                        isValid = false;

                    }
                    //
                    if (isValid == true)
                    {
                        successRecords.Rows.Add(record.SrNo, record.InvoiceNumber, record.InvoiceAmount, record.Currency, record.VehicleId, record.DueDate, record.DealerName, record.DealerAddress1, record.DealerCity, record.TransporterName, record.TransportNumber, record.TransportDate, record.DealerCode, record.TransporterCode, record.DealerAddress2, record.DealerAddress3, record.DealerAddress4, record.TradeRefNo, record.PhysicalReceivedDate, record.Remarks);
                    }
                    else
                    {
                        failureRecords.Rows.Add(record.SrNo, record.InvoiceNumber, record.InvoiceAmount, record.Currency, record.VehicleId, record.DueDate, record.DealerName, record.DealerAddress1, record.DealerCity, record.TransporterName, record.TransportNumber, record.TransportDate, record.DealerCode, record.TransporterCode, record.DealerAddress2, record.DealerAddress3, record.DealerAddress4, record.TradeRefNo, record.PhysicalReceivedDate, record.Remarks);
                    }

                    progress = 40 + (int)(((double)recordCount / invoices.Count) * 40);
                    _uploadProgress[sessionId] = progress;

                    // Send progress update to SignalR hub
                    await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, $" {progress}% ");
                    recordCount++;
                }
                // Processing completed
                progress = 80;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "");

                if (successRecords.Rows.Count > 0)
                {
                    bool flag = await _dataService.BulkInsertAsync_Invoice_Received(successRecords, HttpContext, UserSession.LoginID);
                    if (flag == true)
                    {
                        _logger.LogInformation("FCC File Uploaded Successfully and Total Records :  " + successRecords.Rows.Count);// + " and Data Succesfully Converted : " + cnt + " - UploadReceivedInvoiceDataController;UploadDetails");
                        TempData["alertMessage"] = "FCC File Uploaded Successfully and Total Records :  " + successRecords.Rows.Count;
                    }
                }
                string str = UserSession.LoginID;
                string query23 = @"UPDATE File_Desc SET FCC_Mail=@Search2 where FileID=@Search1";

                var parameters1 = new[]
                {
                     new Microsoft.Data.SqlClient.SqlParameter("@Search2", "0"), new Microsoft.Data.SqlClient.SqlParameter("@Search1", fileId)
                    };
                int rowsAffected1 = _dataService.ExecuteUpdateQuery(query23, parameters1);
                if (rowsAffected1 > 0)
                    _logger.LogInformation("File_Desc Table updated successfully");
                else
                    _logger.LogInformation("There is some issue when updating File_Desc table. Query : " + query23);
                if (failureRecords.Rows.Count > 0)
                {
                    _logger.LogInformation("FCC File uploading Failed Due to some Discrepancies : Failure Record : -" + failureRecords.Rows.Count);
                    //  TempData["alertMessage"] = JsonConvert.SerializeObject(validationErrors);
                    //TempData["alertMessage"] = null;
                    TempData["alertMessage"] = "FCC File uploading Failed Due to some Discrepancies, Please check audit log: Failure Record : -" + failureRecords.Rows.Count;

                }
                // Final completion progress
                progress = 100;
                _uploadProgress[sessionId] = progress;
                await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveProgressUpdate", progress, "Upload process completed");

            }
            catch (Exception ex)
            {
                validationErrors.Add($"Error occurred while uploading invoice details.");
                _logger.LogError(ex, "Error occurred while uploading invoice details.");
                TempData["alertMessage"] = "An error occurred while processing the file.";
            }
            finally
            {
                _uploadProgress.Remove(sessionId);
            }
            return RedirectToAction("ViewUploadReceivedInvoiceData");

        }
        [HttpGet]
        public IActionResult GetProgress()
        {
            var sessionId = HttpContext.Session.Id;
            if (_uploadProgress.TryGetValue(sessionId, out int progress))
            {
                return Json(new { progress });
            }
            return Json(new { progress = 0 });
        }
        public IActionResult ViewUploadReceivedInvoiceData()
        {

            // ViewBag.ProcessingMessages = messages; // Pass the messages to the view
            return View();
        }

        public class InvoiceData
        {

            public string InvoiceRecId { get; set; }
            public string SrNo { get; set; }
            public string InvoiceNumber { get; set; }
            public string InvoiceAmount { get; set; }
            public string Currency { get; set; }
            public string VehicleId { get; set; }
            public string DueDate { get; set; }
            public string DealerName { get; set; }
            public string DealerAddress1 { get; set; }
            public string DealerCity { get; set; }
            public string TransporterName { get; set; }
            public string TransportNumber { get; set; }
            public string TransportDate { get; set; }
            public string DealerCode { get; set; }
            public string TransporterCode { get; set; }
            public string DealerAddress2 { get; set; }
            public string DealerAddress3 { get; set; }
            public string DealerAddress4 { get; set; }
            public string TradeRefNo { get; set; }
            public string PhysicalReceivedDate { get; set; }
            public string Remarks { get; set; }

        }
    }
}

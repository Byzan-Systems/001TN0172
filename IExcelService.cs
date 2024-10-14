//using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace HDFCMSILWebMVC
{
    public interface IExcelService
    {
        Task<DataTable> ReadExcelToDataTableAsync(string filePath);

        Task<int> InsertFileDescAsync(string fileName, string fileType, string fileDate, int srNo, string fccMail);
    }
    public class ExcelService : IExcelService
    {
        private readonly string _connectionString;
        private readonly ILogger<ExcelService> _logger;
        public ExcelService(IConfiguration configuration, ILogger<ExcelService> logger)
        {
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            _logger = logger;
        }
        public async Task<DataTable> ReadExcelToDataTableAsync(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
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

                for (int i = 0; i < cellCount; i++)
                {
                    dataTable.Columns.Add(headerRow.GetCell(i).ToString());
                }

                // Populate DataTable with data from Excel rows
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = dataTable.NewRow();

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while reading Excel file: {ex.Message}");
                throw;
            }

            return dataTable;
        }
        //public async Task<DataTable> ReadExcelToDataTableAsync(string filePath)
        //{
        //    DataTable dataTable = new DataTable();

        //    try
        //    {
        //        using (var package = new ExcelPackage(new FileInfo(filePath)))
        //        {
        //            if (package.Workbook.Worksheets.Count == 0)
        //            {
        //                throw new Exception("No worksheets found in the Excel file.");
        //            }

        //            ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Accessing the first worksheet

        //            if (worksheet == null)
        //            {
        //                throw new Exception("Worksheet is null or not found.");
        //            }

        //            // Loop through the first row and add columns to DataTable
        //            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
        //            {
        //                dataTable.Columns.Add(firstRowCell.Text);
        //            }

        //            // Populate DataTable with data from Excel rows
        //            for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
        //            {
        //                var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
        //                DataRow newRow = dataTable.Rows.Add();

        //                foreach (var cell in row)
        //                {
        //                    newRow[cell.Start.Column - 1] = cell.Text;
        //                }
        //            }
        //        }
        //    }
        //    catch (IndexOutOfRangeException ex)
        //    {
        //        // Handle specific exception related to worksheet position or index here
        //        // Log the exception for debugging
        //        _logger.LogError($"Index out of range exception: {ex.Message}");
        //        throw; // Re-throw the exception to propagate it further
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle general exceptions
        //        _logger.LogError($"Error occurred while reading Excel file: {ex.Message}");
        //        throw; // Re-throw the exception to propagate it further
        //    }

        //    return dataTable;
        //}

        //public async Task<DataTable> ReadExcelToDataTableAsyncXls(string filePath)
        //{
        //    DataTable dataTable = new DataTable();

        //    try
        //    {
        //        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            HSSFWorkbook workbook = new HSSFWorkbook(file);
        //            ISheet sheet = workbook.GetSheetAt(0); // Accessing the first worksheet

        //            // Loop through the first row and add columns to DataTable
        //            IRow headerRow = sheet.GetRow(0);
        //            foreach (var cell in headerRow.Cells)
        //            {
        //                dataTable.Columns.Add(cell.ToString());
        //            }

        //            // Populate DataTable with data from Excel rows
        //            for (int rowNumber = 1; rowNumber <= sheet.LastRowNum; rowNumber++)
        //            {
        //                IRow row = sheet.GetRow(rowNumber);
        //                DataRow newRow = dataTable.Rows.Add();

        //                foreach (var cell in row.Cells)
        //                {
        //                    newRow[cell.ColumnIndex] = cell.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions
        //        _logger.LogError($"Error occurred while reading Excel file: {ex.Message}");
        //        throw;
        //    }

        //    return dataTable;
        //}

        public async Task<int> InsertFileDescAsync(string fileName, string fileType, string fileDate, int srNo, string fccMail)
        {
            int newFileId;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string selectMaxIdQuery = "SELECT ISNULL(MAX(CAST(FileID AS int)), 0) + 1 AS File_ID FROM File_Desc";
                using (SqlCommand selectCommand = new SqlCommand(selectMaxIdQuery, connection))
                {
                    newFileId = (int)await selectCommand.ExecuteScalarAsync();
                }

                string insertQuery = "INSERT INTO File_Desc (FileID, FileName, FileType, File_Date, SrNo, FCC_Mail) VALUES (@FileID, @FileName, @FileType, @FileDate, @SrNo, @FCC_Mail)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@FileID", newFileId);
                    insertCommand.Parameters.AddWithValue("@FileName", fileName);
                    insertCommand.Parameters.AddWithValue("@FileType", fileType);
                    insertCommand.Parameters.AddWithValue("@FileDate", fileDate);
                    insertCommand.Parameters.AddWithValue("@SrNo", srNo);
                    insertCommand.Parameters.AddWithValue("@FCC_Mail", fccMail);

                    await insertCommand.ExecuteNonQueryAsync();
                }
                connection.CloseAsync();
            }

            return newFileId;
        }
    }
}

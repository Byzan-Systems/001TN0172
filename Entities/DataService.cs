using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace HDFCMSILWebMVC.Entities
{
    public class DataService
    {
        private readonly string _connectionString;

        public DataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable GetDataTable(string _query)
        {
            var dataTable = new DataTable();
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
                {
                    var query = _query;
                    var command = new System.Data.SqlClient.SqlCommand(query, connection);
                    var dataAdapter = new System.Data.SqlClient.SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {

            }
            return dataTable;
        }
        public DataTable ExecuteStoredProcedure(string storedProcedureName, Microsoft.Data.SqlClient.SqlParameter[] parameters)
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    connection.Open();

                    var dataTable = new DataTable();
                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                    connection.Close();
                    return dataTable;
                }
            }
        }
        public int ExecuteUpdateQuery(string query, Microsoft.Data.SqlClient.SqlParameter[] parameters)
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddRange(parameters);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    return rowsAffected;
                }
            }
        }
        public string ConvertDateString(string inputDate)
        {
            // Define the input format and the desired output format
            string inputFormat = "dd/MM/yyyy";
            string outputFormat = "yyyy-MM-dd HH:mm:ss.fff";

            // Parse the input date string
            if (DateTime.TryParseExact(inputDate, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                // Format the parsed date to the desired output format
                return parsedDate.ToString(outputFormat);
            }
            else
            {
                // Handle the case where the date string could not be parsed
                throw new FormatException("The input date string is not in the expected format.");
            }
        }
            public async Task<bool> BulkInsertAsync_Invoice_Received(DataTable records, HttpContext httpContext,string userID)
        {
            int maxRetries = 3;
            int delay = 2000; // 2 seconds
            int attempt = 0;

            while (attempt < maxRetries)
            {
                attempt++;

                using var sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                await sqlConnection.OpenAsync();

                using var transaction = sqlConnection.BeginTransaction();

                try
                {
                    // Add new columns to the DataTable if they don't exist
                    if (!records.Columns.Contains("LoginID"))
                    {
                        records.Columns.Add("LoginID", typeof(string));
                    }

                    if (!records.Columns.Contains("Invoice_Rec_ID"))
                    {
                        records.Columns.Add("Invoice_Rec_ID", typeof(int));
                    }
                    // Ensure the DataTable has the correct data type for the date column
                    records.Columns["PHYSICAL RECIEVED DATE"].DataType = typeof(string);

                    // Set the Invoice_Rec_ID dynamically within the transaction
                    string query = "SELECT ISNULL(MAX(CONVERT(INT, Invoice_Rec_ID)), 0) + 1 FROM Invoice_Received";
                    using var command = new Microsoft.Data.SqlClient.SqlCommand(query, sqlConnection, transaction);
                    int invoiceRecId = (int)await command.ExecuteScalarAsync();

                    // Set the default value for LoginID and Invoice_Rec_ID
                    foreach (DataRow row in records.Rows)
                    {
                        row["LoginID"] = userID; // Replace with the actual default value you want to set
                        row["Invoice_Rec_ID"] = invoiceRecId++;
                        // Parse the Physical_Received_date column if it exists
                        string dateString = ConvertDateString(row["PHYSICAL RECIEVED DATE"].ToString());
                        
                            row["PHYSICAL RECIEVED DATE"] = dateString;
                        
                    }

                    using var sqlBulkCopy = new Microsoft.Data.SqlClient.SqlBulkCopy(sqlConnection, Microsoft.Data.SqlClient.SqlBulkCopyOptions.Default, transaction)
                    {
                        DestinationTableName = "Invoice_Received"
                    };

                    sqlBulkCopy.ColumnMappings.Add("Invoice_Rec_ID", "Invoice_Rec_ID");
                    sqlBulkCopy.ColumnMappings.Add("Sr.No", "Sr_No");

                    sqlBulkCopy.ColumnMappings.Add("INVOICE NUMBER", "Invoice_Number");
                    sqlBulkCopy.ColumnMappings.Add("INVOICE Amount", "Invoice_Amount");
                    sqlBulkCopy.ColumnMappings.Add("CURRENCY", "Currency");
                    sqlBulkCopy.ColumnMappings.Add("DESCRIPTION OF GOODS", "Vehical_ID");
                    sqlBulkCopy.ColumnMappings.Add("DUE DATE", "DueDate");
                    sqlBulkCopy.ColumnMappings.Add("BUYER NAME", "Dealer_Name");
                    sqlBulkCopy.ColumnMappings.Add("ADDRESS", "Dealer_Address1");
                    sqlBulkCopy.ColumnMappings.Add("CITY", "Dealer_City");
                    sqlBulkCopy.ColumnMappings.Add("TRANSPORTER NAME", "Transporter_Name");
                    sqlBulkCopy.ColumnMappings.Add("TRANSPORT NUMBER(L/R or D/C or GRN or MRIR)", "Transport_Number");
                    sqlBulkCopy.ColumnMappings.Add("TRANSPORT DATE", "Transport_Date");
                    sqlBulkCopy.ColumnMappings.Add("DEALER CODE", "Dealer_Code");
                    sqlBulkCopy.ColumnMappings.Add("TRANSPORTERCODE", "Transporter_Code");
                    sqlBulkCopy.ColumnMappings.Add("DEALER ADDRESS LINE 2", "Dealer_Address2");
                    sqlBulkCopy.ColumnMappings.Add("DEALER ADDRESS LINE 3", "Dealer_Address3");
                    sqlBulkCopy.ColumnMappings.Add("DEALER ADDRESS LINE 4", "Dealer_Address4");
                    sqlBulkCopy.ColumnMappings.Add("TRADE REFERENCE NO", "Trade_RefNo");
                    sqlBulkCopy.ColumnMappings.Add("PHYSICAL RECIEVED DATE", "Physical_Received_date");
                    sqlBulkCopy.ColumnMappings.Add("REMARK", "Remarks");


                    // Add the new columns to the column mappings
                    sqlBulkCopy.ColumnMappings.Add("LoginID", "LoginID");
                   

                    await sqlBulkCopy.WriteToServerAsync(records);

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true; // Return true if the operation succeeds
                }
                catch (Exception ex)
                {
                    // Rollback the transaction on error
                    await transaction.RollbackAsync();

                    if (ex is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == 1205) // SQL Server error code for deadlock
                    {
                        // Log the error (consider using a logging framework)
                        Console.WriteLine($"Attempt {attempt} - Transaction was deadlocked. Retrying in {delay / 1000} seconds...");
                        await Task.Delay(delay); // Wait before retrying
                    }
                    else
                    {
                        // Log other exceptions
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        return false; // Return false if a non-deadlock exception occurs
                    }
                }
            }

            return false; // Return false if all retry attempts fail
        }
        public bool TryParseDate(string dateString, out DateTime parsedDate)
        {
            return DateTime.TryParseExact(
                dateString,
                "yyyy-MM-dd HH:mm:ss fff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate
            );
        }


    }
}

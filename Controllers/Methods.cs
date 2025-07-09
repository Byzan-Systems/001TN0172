using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace HDFCMSILWebMVC.Controllers
{
    public class Methods
    {
        //private readonly ILogger<Methods> _logger;
        //public static readonly ILogger _logger;// = ApplicationLogging.CreateLogger("YourClass");
        //public Methods(ILogger<Methods> logger)
        //{
        //    _logger = logger;
        //}

        public static int CashOps_Payments(string[] Details, string[] detailsCash, ILogger logger)
        {
            try
            {
                //Check Duplicate UTR No. and Amount
                DataTable dt = Methods.getDetails("Get_CashOpsDetails", Details[0], Details[1], "", "", "", "", "", logger);
                if (dt.Rows.Count > 0)
                {
                    //clserr.WriteLogToTxtFile("Duplicate UTR No.: " + Details[11] + " with same  Amount: " + Details[3], "CashOps_Payments", strFileName);
                    return -1;
                }
                //clserr.WriteLogToTxtFile("Insert values in Payment Upload table", "CashOps_Payments", strFileName);
                //clserr.WriteLogToTxtFile("VA_account =" + Details[8].Substring(0, 22) + " and UTR Number =" + Details[11], "CashOps_Payments", strFileName);
                DataTable dtCash = Methods.Insert_PaymentUploadDetails("Save", Details, logger);
                if (dtCash != null)
                {
                    if (Details[2].ToUpper() == "C")
                    {
                        Fill_CashOpsDetails(dtCash.Rows[0][0].ToString(), Details, detailsCash, logger);
                    }
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(ex.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; CashOps_Payments");
                return 0;
            }
        }
        //public void WriteErrorToTxtFile(string ErrorDesc, string ModuleName, string ProcName)
        //{
        //    string strfilename = string.Empty;
        //    string strErrorString = string.Empty;
        //    try
        //    {
        //        string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        //        strErrorString = "[" + DateTime.Now.ToString("dd MM yyyy") + "] [ " + ErrorDesc + "] [ " + ModuleName + "] [ " + ProcName + "]";
        //        strfilename = sttg.ErrorLog + "\\" + ModuleName + ".log";

        //        FileStream fsObj;
        //        StreamWriter SwOpenFile;

        //        if (File.Exists(strfilename))
        //        {
        //            fsObj = new FileStream(strfilename, FileMode.Append, FileAccess.Write, FileShare.Read);
        //        }
        //        else
        //        {
        //            fsObj = new FileStream(strfilename, FileMode.Create, FileAccess.Write, FileShare.Read);
        //        }

        //        SwOpenFile = new StreamWriter(fsObj);
        //        SwOpenFile.WriteLine(strErrorString);

        //        fsObj.Flush();
        //        SwOpenFile.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static void Fill_CashOpsDetails(string NEFT_RTGS_BT_ID, string[] Details, string[] detailsCash, ILogger logger)
        {
            try
            {
                string strIFSCPart = Details[12].ToString().Substring(0, 4);
                DataTable dt_Finance = Methods.getDetails("Get_FinancerDetailsAsPer", Details[8].ToString().Substring(Details[8].Length - 1, 1), strIFSCPart, "", "", "", "", "", logger);


                //clserr.WriteLogToTxtFile("Insert values into CashOps Upload Table", "Fill_CashOpsDetails", strFileName);
                detailsCash[3] = NEFT_RTGS_BT_ID; //NEFT_RTGS_BT_ID
                                                  //if (Details[1] == "MAN")
                                                  //{
                if (dt_Finance.Rows.Count > 0)
                {
                    detailsCash[21] = dt_Finance.Rows[0]["FCode"].ToString(); detailsCash[22] = dt_Finance.Rows[0]["FName"].ToString();
                }
                DataTable dt = Methods.Insert_CashOpsDetails("Save", detailsCash, logger);
                //clserr.WriteLogToTxtFile("FNCR Virtual Account and UTR no in CashOps = " + detailsCash[19] + " and " + detailsCash[5], "Fill_CashOpsDetails", strFileName);
            }
            catch (Exception ex)
            { 
                //log enhance by chaitrali 3/7/2024
                logger.LogError(ex.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; Fill_CashOpsDetails");
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Fill_CashOpsDetails");
            }
            //if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "B")
            //{
            //    detailsCash[21] = "MMFS"; detailsCash[22] = "Mahindra & Mahindra Financial Services Ltd";
            //}
            //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "C")
            //{
            //    detailsCash[21] = "TCFS"; detailsCash[22] = "Tata Capital Financial Services Ltd";
            //}
            //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "K")
            //{
            //    detailsCash[21] = "KOTM"; detailsCash[22] = "KOTAK MAHINDRA PRIMUS LTD";
            //}
            //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "V")
            //{
            //    detailsCash[21] = ""; detailsCash[22] = "Vehicle Non inventory";
            //}
            //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "D")  //Added by yogesh on mail 22-08-2023
            //{
            //    switch (Details[12].Substring(0, 4))
            //    {
            //        case "CIFL":
            //            detailsCash[21] = "CIFL"; detailsCash[22] = "Cholamandalam Investment and Finance Company Limited"; //Added by yogesh on mail 22-08-2023
            //            break;
            //    }
            //}
            //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "F")
            //{
            //    switch (Details[12].Substring(0, 4))
            //    {
            //        case "RATN":
            //            detailsCash[21] = "RTNK"; detailsCash[22] = "RATNAKAR BANK";
            //            break;
            //        case "YESB":
            //            detailsCash[21] = "YESB"; detailsCash[22] = "YES BANK";
            //            break;
            //        case "SBIN":
            //            detailsCash[21] = "SBIN"; detailsCash[22] = "STATE BANK OF INDIA";
            //            break;
            //        case "ICIC":
            //            detailsCash[21] = "ICIC"; detailsCash[22] = "ICICI BANK LTD";
            //            break;
            //        case "HDFC":
            //            detailsCash[21] = "HDFB"; detailsCash[22] = "HDFC Bank";
            //            break;
            //        case "BARB":
            //            detailsCash[21] = "BOBR"; detailsCash[22] = "BANK OF BARODA";
            //            break;
            //        case "UTIB":
            //            detailsCash[21] = "UTIB"; detailsCash[22] = "AXIS BANK LTD";
            //            break;
            //        case "BKID":
            //            detailsCash[21] = "BOIB"; detailsCash[22] = "BANK OF INDIA";
            //            break;
            //        case "INDB":
            //            detailsCash[21] = "INDB"; detailsCash[22] = "INDUSIND BANK";
            //            break;
            //        case "FDRL":
            //            detailsCash[21] = "FDRL"; detailsCash[22] = "THE FEDERAL BANK LIMITED";
            //            break;
            //        case "MAHB":
            //            detailsCash[21] = "BOMH"; detailsCash[22] = "BANK OF MAHARASHTRA";
            //            break;
            //        case "SIBL":
            //            detailsCash[21] = "SINB"; detailsCash[22] = "South Indian Bank"; //Added by yogesh on mail 22-08-2023
            //            break;
            //    }
            //}
            //}

        }
        public static void CashOps_Payments_Manual(string[] Details, string[] detailsCash, ILogger logger)
        {
            try
            {
                //Check Duplicate UTR No. and Amount
                DataTable dt = Methods.getDetails("Get_CashOpsDetails", Details[0], Details[1], "", "", "", "", "", logger);
                if (dt.Rows.Count > 0)
                {
                    //clserr.WriteLogToTxtFile("Duplicate UTR No.: " + Details[11] + " with same  Amount: " + Details[3], "CashOps_Payments", strFileName);
                    return;
                }
                //clserr.WriteLogToTxtFile("Insert values in Payment Upload table", "CashOps_Payments", strFileName);
                //clserr.WriteLogToTxtFile("VA_account =" + Details[8].Substring(0, 22) + " and UTR Number =" + Details[11], "CashOps_Payments", strFileName);
                DataTable dtCash = Methods.Insert_PaymentUploadDetails("Save", Details, logger);
                if (Details[2].ToUpper() == "C")
                {
                    Fill_CashOpsDetails_Manual(dtCash.Rows[0][0].ToString(), Details, detailsCash, logger);
                }
            }
            catch (Exception ex)
            {
                //log enhance by chaitrali 3/7/2024
                string sanitizedAccountNo = detailsCash[4]?.Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", "");
                string sanitizedUTRNo = detailsCash[5]?.Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", "");
                logger.LogError(ex.Message + " For Virtual Account No: " + sanitizedAccountNo + " and UTR No: " + sanitizedUTRNo + " ; CashOps_Payments_Manual");
            }
        }
        public static void Fill_CashOpsDetails_Manual(string NEFT_RTGS_BT_ID, string[] Details, string[] detailsCash, ILogger logger)
        {
            try
            {
                //clserr.WriteLogToTxtFile("Insert values into CashOps Upload Table", "Fill_CashOpsDetails", strFileName);
                detailsCash[3] = NEFT_RTGS_BT_ID; //NEFT_RTGS_BT_ID
                if (Details[1] == "MAN")
                {
                    string strIFSCPart = Details[12].ToString().Substring(0, 4);
                    DataTable dt_Finance = Methods.getDetails("Get_FinancerDetailsAsPer", Details[8].ToString().Substring(Details[8].Length - 1, 1), strIFSCPart, "", "", "", "", "", logger);


                    if (dt_Finance.Rows.Count > 0)
                    {
                        detailsCash[21] = dt_Finance.Rows[0]["FCode"].ToString(); detailsCash[22] = dt_Finance.Rows[0]["FName"].ToString();
                    }
                    //if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "B")
                    //{
                    //    detailsCash[21] = "MMFS"; detailsCash[22] = "Mahindra & Mahindra Financial Services Ltd";
                    //}
                    //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "C")
                    //{
                    //    detailsCash[21] = "TCFS"; detailsCash[22] = "Tata Capital Financial Services Ltd";
                    //}
                    //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "K")
                    //{
                    //    detailsCash[21] = "KOTM"; detailsCash[22] = "KOTAK MAHINDRA PRIMUS LTD";
                    //}
                    //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "V")
                    //{
                    //    detailsCash[21] = ""; detailsCash[22] = "Vehicle Non inventory";
                    //}
                    //else if (Details[8].ToString().Substring(Details[8].Length - 1, 1) == "F")
                    //{
                    //    switch (Details[12].Substring(0, 4))
                    //    {
                    //        case "RATN":
                    //            detailsCash[21] = "RTNK"; detailsCash[22] = "RATNAKAR BANK";
                    //            break;
                    //        case "YESB":
                    //            detailsCash[21] = "YESB"; detailsCash[22] = "YES BANK";
                    //            break;
                    //        case "SBIN":
                    //            detailsCash[21] = "SBIN"; detailsCash[22] = "STATE BANK OF INDIA";
                    //            break;
                    //        case "ICIC":
                    //            detailsCash[21] = "ICIC"; detailsCash[22] = "ICICI BANK LTD";
                    //            break;
                    //        case "HDFC":
                    //            detailsCash[21] = "HDFB"; detailsCash[22] = "HDFC Bank";
                    //            break;
                    //        case "BARB":
                    //            detailsCash[21] = "BOBR"; detailsCash[22] = "BANK OF BARODA";
                    //            break;
                    //        case "UTIB":
                    //            detailsCash[21] = "UTIB"; detailsCash[22] = "AXIS BANK LTD";
                    //            break;
                    //        case "BKID":
                    //            detailsCash[21] = "BOIB"; detailsCash[22] = "BANK OF INDIA";
                    //            break;
                    //        case "INDB":
                    //            detailsCash[21] = "INDB"; detailsCash[22] = "INDUSIND BANK";
                    //            break;
                    //        case "FDRL":
                    //            detailsCash[21] = "FDRL"; detailsCash[22] = "THE FEDERAL BANK LIMITED";
                    //            break;
                    //        case "MAHB":
                    //            detailsCash[21] = "BOMH"; detailsCash[22] = "BANK OF MAHARASHTRA";
                    //            break;
                    //    }
                    //}
                }
                DataTable dt = Methods.Insert_CashOpsDetails("Save", detailsCash, logger);
                //clserr.WriteLogToTxtFile("FNCR Virtual Account and UTR no in CashOps = " + detailsCash[19] + " and " + detailsCash[5], "Fill_CashOpsDetails", strFileName);
            }
            catch (Exception ex)
            {
                //log enhance by chaitrali 3/7/2024
              
                string accountno= detailsCash[4].ToString().Length <= 4 ? "****" : new string('*', detailsCash[4].ToString().Length - 4) + detailsCash[4].ToString()[^4..];
                logger.LogError(ex, "Fill_CashOpsDetails_Manual failed for VirtualAccount ending in {VirtualAccountMasked} and UTR Present: {UTRPresent}", accountno, !string.IsNullOrWhiteSpace(detailsCash[5]));

               // logger.LogError(ex.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; Fill_CashOpsDetails_Manual");
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmPaymentInformation", "Fill_CashOpsDetails");
            }
        }
        public static DataTable Insert_CashOpsDetails(string Task, string[] detailsCash, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {
                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_Insert_CashOpsDetails";
                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Cash_Ops_ID", SqlDbType.VarChar) { Value = detailsCash[0] });
                    cmd.Parameters.Add(new SqlParameter("@FileID", SqlDbType.VarChar) { Value = detailsCash[1] });
                    cmd.Parameters.Add(new SqlParameter("@CashOps_FileType", SqlDbType.VarChar) { Value = detailsCash[2] });
                    cmd.Parameters.Add(new SqlParameter("@NEFT_RTGS_BT_ID", SqlDbType.VarChar) { Value = detailsCash[3] });
                    cmd.Parameters.Add(new SqlParameter("@Virtual_Account", SqlDbType.VarChar) { Value = detailsCash[4] });
                    cmd.Parameters.Add(new SqlParameter("@UTR_No", SqlDbType.VarChar) { Value = detailsCash[5] });
                    cmd.Parameters.Add(new SqlParameter("@Transaction_Amount", SqlDbType.VarChar) { Value = detailsCash[6] });
                    cmd.Parameters.Add(new SqlParameter("@Transaction_status", SqlDbType.VarChar) { Value = detailsCash[7] });
                    cmd.Parameters.Add(new SqlParameter("@Payment_Status", SqlDbType.VarChar) { Value = detailsCash[8] });
                    cmd.Parameters.Add(new SqlParameter("@Attempt", SqlDbType.VarChar) { Value = detailsCash[9] });
                    cmd.Parameters.Add(new SqlParameter("@Cash_Ops_Date", SqlDbType.VarChar) { Value = detailsCash[10] });
                    cmd.Parameters.Add(new SqlParameter("@Cash_Ops_Time", SqlDbType.VarChar) { Value = detailsCash[11] });
                    cmd.Parameters.Add(new SqlParameter("@GEFO_Flag", SqlDbType.VarChar) { Value = detailsCash[12] });
                    cmd.Parameters.Add(new SqlParameter("@GEFO_Date", SqlDbType.VarChar) { Value = detailsCash[13] });
                    cmd.Parameters.Add(new SqlParameter("@DR_Account_No", SqlDbType.VarChar) { Value = detailsCash[14] });
                    cmd.Parameters.Add(new SqlParameter("@CR_Account_No", SqlDbType.VarChar) { Value = detailsCash[15] });
                    cmd.Parameters.Add(new SqlParameter("@LoginID", SqlDbType.VarChar) { Value = UserSession.LoginID.ToString() });
                    cmd.Parameters.Add(new SqlParameter("@EOD_MailFlag", SqlDbType.VarChar) { Value = detailsCash[17] });
                    cmd.Parameters.Add(new SqlParameter("@DRC_Generation", SqlDbType.VarChar) { Value = detailsCash[18] });
                    cmd.Parameters.Add(new SqlParameter("@FNCR_Virtual_Account", SqlDbType.VarChar) { Value = detailsCash[19] });
                    cmd.Parameters.Add(new SqlParameter("@IFSC_code", SqlDbType.VarChar) { Value = detailsCash[20] });
                    cmd.Parameters.Add(new SqlParameter("@FNCR_code", SqlDbType.VarChar) { Value = detailsCash[21] });
                    cmd.Parameters.Add(new SqlParameter("@FNCR_Name", SqlDbType.VarChar) { Value = detailsCash[22] });

                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                string maskeddata = "";
                if (string.IsNullOrWhiteSpace(detailsCash[4].ToString())) maskeddata = "Unknown";
                maskeddata = detailsCash[4].ToString().Length <= 5 ? "****" : new string('*', detailsCash[4].ToString().Length - 5) + detailsCash[4].ToString()[^5..];
                logger.LogError(EX, "Insert_CashOpsDetails failed for Virtual Account ending in {MaskedAccount} and UTR Present: {UtrPresent}", maskeddata, !string.IsNullOrWhiteSpace(detailsCash[5]));
                //  logger.LogError(EX.Message + " For Virtual Account No: " + detailsCash[4] + "" + " and UTR No: " + detailsCash[5] + " ; Insert_CashOpsDetails");
                return null;
            }
        }

        public static DataTable Insert_PaymentUploadDetails(string Task, string[] details, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {
                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_Insert_PaymentUpload";
                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Record_ID", SqlDbType.VarChar) { Value = details[0] });
                    cmd.Parameters.Add(new SqlParameter("@Transaction_Id", SqlDbType.VarChar) { Value = details[0] });
                    //cmd.Parameters.Add(new SqlParameter("@Transaction_Id", SqlDbType.Decimal) { Value =  details[0] });
                    cmd.Parameters.Add(new SqlParameter("@Type_of_Entry", SqlDbType.VarChar) { Value = details[1] });
                    cmd.Parameters.Add(new SqlParameter("@Dr_CR", SqlDbType.VarChar) { Value = details[2] });
                    cmd.Parameters.Add(new SqlParameter("@Entry_Amount", SqlDbType.VarChar) { Value = details[3] });
                    //cmd.Parameters.Add(new SqlParameter("@Entry_Amount", SqlDbType.Decimal) { Value = details[3] });
                    cmd.Parameters.Add(new SqlParameter("@Value_date", SqlDbType.VarChar) { Value = details[4] });
                    cmd.Parameters.Add(new SqlParameter("@Product", SqlDbType.VarChar) { Value = details[5] });
                    cmd.Parameters.Add(new SqlParameter("@Party_Code", SqlDbType.VarChar) { Value = details[6] });
                    cmd.Parameters.Add(new SqlParameter("@Party_Name", SqlDbType.VarChar) { Value = details[7] });
                    cmd.Parameters.Add(new SqlParameter("@VA_account", SqlDbType.VarChar) { Value = details[8].Substring(0, 22) });
                    cmd.Parameters.Add(new SqlParameter("@Locations", SqlDbType.VarChar) { Value = details[9] });
                    cmd.Parameters.Add(new SqlParameter("@RemittingBank", SqlDbType.VarChar) { Value = details[10] });
                    cmd.Parameters.Add(new SqlParameter("@UTR_No", SqlDbType.VarChar) { Value = details[11] });
                    cmd.Parameters.Add(new SqlParameter("@IFSC_Code", SqlDbType.VarChar) { Value = details[12] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Name", SqlDbType.VarChar) { Value = details[13] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Account_No", SqlDbType.VarChar) { Value = details[14] });
                    cmd.Parameters.Add(new SqlParameter("@Releated_Ref_No", SqlDbType.VarChar) { Value = details[15] });
                    cmd.Parameters.Add(new SqlParameter("@FileId", SqlDbType.VarChar) { Value = details[16] });
                    cmd.Parameters.Add(new SqlParameter("@UploadDateTime", SqlDbType.VarChar) { Value = System.DateTime.Now.Date.ToString("dd/MMM/yyyy HH:MM:SS AMPM") });
                    cmd.Parameters.Add(new SqlParameter("@LoginID", SqlDbType.VarChar) { Value = UserSession.LoginID.ToString() });

                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Virtual Account No: " + details[8].Substring(0, 22) + "" + " and UTR No: " + details[11] + " ; Insert_PaymentUploadDetails");
                return null;
            }
        }
        public static DataSet getDetails_Web(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataSet ds = new DataSet();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {
                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_GetDetails_Web";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                    }

                    cmd.Connection.Close();
                    return ds;
                }
            }
            catch (Exception EX)
            { //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Task: " + Task + "" + " and invoiceNo or Do Number or OrderID: " + Search1 + " ; getDetails_Web");
                return null;
            }
        }
        public static string EncryptDecryptData(string Task, string PlainText, string @InputEncryptedData, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "EncryptDecryptData";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@PlainText", SqlDbType.VarChar) { Value = PlainText });
                    cmd.Parameters.Add(new SqlParameter("@InputEncryptedData", SqlDbType.VarChar) { Value = InputEncryptedData });
                    //cmd.ExecuteReader();  
                    var result = cmd.ExecuteScalar();
                    cmd.Connection.Close();
                    return result.ToString();
                    //using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    //{
                    //    adapter.SelectCommand = cmd;
                    //    adapter.Fill(dataTable);
                    //}

                    
                    //return dataTable;
                }
            }
            catch (Exception EX)
            {
                if (string.IsNullOrEmpty(PlainText)) return "N/A";
                string abc= PlainText.Length <= 4 ? "****" : new string('*', PlainText.Length - 4) + PlainText[^4..];
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX, "EncryptDecryptData failed. Task: {Task}, AccountNo: {MaskedAccountNo}, EncryptedInputPresent: {HasEncryptedInput}",Task, abc, !string.IsNullOrWhiteSpace(InputEncryptedData));
                return null;
            }
        }
     
        public static DataTable getDetailsAudit(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_GetDetails_Audit";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2025
                logger.LogError(EX.Message , "getDetailsAudit"); return null;
            }
        }

        public static DataTable getDetails(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_GetDetails";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Task: " + Task + "" + " and invoiceNo or Do Number or OrderID: " + Search1 + " ; getDetails"); return null;
            }
        }
        public static DataTable InsertDetails(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_InsertDetails";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message, "InsertDetails failed. Task: {Task}, FileName: {FileName}", Task, Search1);
                return null;
            }
        }

        public static DataTable InsertDetailsAudit(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                //using var cmd = db.LoginMSTs.FromSqlRaw($"SP_FTPayment");
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_InsertDetails_Audit";

                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }

                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Task: " + Task + "" + " and FileName: " + Search1 + " ; InsertDetails");
                return null;
            }
        }
        public static DataTable UpdateDetails(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_UpdateDetails";

                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }


                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            { //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Task: " + Task + "" + " and FileName or Cashops ID: " + Search1 + " ; UpdateDetails");
                return null;
            }
        }

        public static DataTable UpdateDetailsAudit(string Task, string Search1, string Search2, string Search3, string Search4, string Search5, string Search6, string Search7,ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                using (var db = new Entities.DatabaseContext())
                {

                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_UpdateDetailsAudit";

                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Search1", SqlDbType.VarChar) { Value = Search1 });
                    cmd.Parameters.Add(new SqlParameter("@Search2", SqlDbType.VarChar) { Value = Search2 });
                    cmd.Parameters.Add(new SqlParameter("@Search3", SqlDbType.VarChar) { Value = Search3 });
                    cmd.Parameters.Add(new SqlParameter("@Search4", SqlDbType.VarChar) { Value = Search4 });
                    cmd.Parameters.Add(new SqlParameter("@Search5", SqlDbType.VarChar) { Value = Search5 });
                    cmd.Parameters.Add(new SqlParameter("@Search6", SqlDbType.VarChar) { Value = Search6 });
                    cmd.Parameters.Add(new SqlParameter("@Search7", SqlDbType.VarChar) { Value = Search7 });
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }


                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            { //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Task: " + Task + "" + " and FileName or Cashops ID: " + Search1 + " ; UpdateDetails");
                return null;
            }
        }
        public static DataTable Insert_InvoiceRecievedDetails(string Task, string[] details, ILogger logger)
        {
            try
            {
                DataTable dataTable = new DataTable();
                using (var db = new Entities.DatabaseContext())
                {
                    DbConnection connection = db.Database.GetDbConnection();
                    using var cmd = db.Database.GetDbConnection().CreateCommand();
                    DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
                    cmd.CommandText = "SP_Insert_InvoiceReceivedDetails";
                    //common
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                    cmd.Parameters.Add(new SqlParameter("@Task", SqlDbType.VarChar) { Value = Task });
                    cmd.Parameters.Add(new SqlParameter("@Invoice_Rec_ID", SqlDbType.VarChar) { Value = details[0] });
                    cmd.Parameters.Add(new SqlParameter("@Sr_No", SqlDbType.VarChar) { Value = details[1] });
                    cmd.Parameters.Add(new SqlParameter("@Invoice_Number", SqlDbType.VarChar) { Value = details[2] });
                    cmd.Parameters.Add(new SqlParameter("@Invoice_Amount", SqlDbType.VarChar) { Value = details[3] });
                    cmd.Parameters.Add(new SqlParameter("@Currency", SqlDbType.VarChar) { Value = details[4] });
                    cmd.Parameters.Add(new SqlParameter("@Vehical_ID", SqlDbType.VarChar) { Value = details[5] });
                    cmd.Parameters.Add(new SqlParameter("@DueDate", SqlDbType.VarChar) { Value = details[6] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Name", SqlDbType.VarChar) { Value = details[7] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Address1", SqlDbType.VarChar) { Value = details[8] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_City", SqlDbType.VarChar) { Value = details[9] });
                    cmd.Parameters.Add(new SqlParameter("@Transporter_Name", SqlDbType.VarChar) { Value = details[10] });
                    cmd.Parameters.Add(new SqlParameter("@Transport_Number", SqlDbType.VarChar) { Value = details[11] });
                    cmd.Parameters.Add(new SqlParameter("@Transport_Date", SqlDbType.VarChar) { Value = details[12] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Code", SqlDbType.VarChar) { Value = details[13] });
                    cmd.Parameters.Add(new SqlParameter("@Transporter_Code", SqlDbType.VarChar) { Value = details[14] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Address2", SqlDbType.VarChar) { Value = details[15] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Address3", SqlDbType.VarChar) { Value = details[16] });
                    cmd.Parameters.Add(new SqlParameter("@Dealer_Address4", SqlDbType.VarChar) { Value = details[17] });
                    cmd.Parameters.Add(new SqlParameter("@Trade_RefNo", SqlDbType.VarChar) { Value = details[18] });
                    cmd.Parameters.Add(new SqlParameter("@Physical_Received_Date", SqlDbType.VarChar) { Value = details[19] });
                    cmd.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.VarChar) { Value = details[20] });
                    cmd.Parameters.Add(new SqlParameter("@LoginID", SqlDbType.VarChar) { Value = UserSession.LoginID.ToString() });
                    //cmd.ExecuteReader();  
                    using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dataTable);
                    }
                    cmd.Connection.Close();
                    return dataTable;
                }
            }
            catch (Exception EX)
            {
                //log enhance by chaitrali 3/7/2024
                logger.LogError(EX.Message + " For Invoice Number : " + details[2] + " ; Insert_InvoiceRecievedDetails");
                return null;
            }
        }
        public static void SendEmail(string SendTo, string SendCC, string SendBCC, string FileAttach, string subject, string Message, string Email_FromID, string PWD, string SMTP_HOST, string Port,ILogger logger)
        {
            string strmessage = "";
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTP_HOST, Convert.ToInt32(Port));
                mail.From = new MailAddress(Email_FromID);
                Attachment attachment;

                string[] ArrSendTo;
                ArrSendTo = SendTo.ToString().Trim().Split(";");
                for (int i = 0; i <= ArrSendTo.Length - 1; i++)
                    mail.To.Add(new MailAddress(ArrSendTo[i]));


                mail.Subject = subject;
                // string strMailBody;
                mail.IsBodyHtml = true;
                attachment = new Attachment(FileAttach);
                mail.Attachments.Add(attachment);


                mail.Body = Message;

                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Message, null, "text/html");
                //mail.AlternateViews.Add(htmlView);

                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Email_FromID, PWD);
                SmtpServer.Send(mail);
                mail.Dispose();
                SmtpServer.Dispose();

                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                //clserr.WriteLogToTxtFile("Attached file = " + Path.GetFileName(FileAttach), "SendMail", "SendEmail");
                //clserr.WriteLogToTxtFile("Email Sent Successfully", "Upload_Click", "SendEmail");

                //strmessage = strmessage + Environment.NewLine + "SupplierID = " + ToSupplierID + "  " + "Email sent successfully";
            }
            catch (Exception ex)
            {
                // clserr.WriteLogToTxtFile("Attached file = " + Path.GetFileName(FileAttach), "SendMail", "SendEmail");
                strmessage = strmessage + Environment.NewLine + "Receipent = " + SendTo + "  " + "Email sending failed check error and audit log";
                //clserr.WriteLogToTxtFile("SupplierID = " + SendTo + "   " + SendTo + " Email Sending failed", "Upload_Click", "SendEmail");
                //clserr.Handle_Error(ex, "FrmPaymentInformation", "SendEmail");

                //log enhance by chaitrali 3/7/2024
                logger.LogError(ex.Message + " For Message: " + strmessage +  " ; UpdateDetails");

            }
        }
        public static void SendEmailstrBld(string SendTo, string SendCC, string SendBCC, string FileAttach, string subject, StringBuilder Message, string Email_FromID, string PWD, string SMTP_HOST, string Port,ILogger logger)
        {
            string strmessage = "";
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTP_HOST, Convert.ToInt32(Port));
                mail.From = new MailAddress(Email_FromID);
                Attachment attachment;

                string[] ArrSendTo;
                ArrSendTo = SendTo.ToString().Trim().Split(";");
                for (int i = 0; i <= ArrSendTo.Length - 1; i++)
                    mail.To.Add(new MailAddress(ArrSendTo[i]));


                mail.Subject = subject;
                // string strMailBody;
                mail.IsBodyHtml = true;
                attachment = new Attachment(FileAttach);
                mail.Attachments.Add(attachment);


                mail.Body = Message.ToString();

                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Message, null, "text/html");
                //mail.AlternateViews.Add(htmlView);

                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Email_FromID, PWD);
                SmtpServer.Send(mail);
                mail.Dispose();
                SmtpServer.Dispose();

               // System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                //clserr.WriteLogToTxtFile("Attached file = " + Path.GetFileName(FileAttach), "SendMail", "SendEmail");
                //clserr.WriteLogToTxtFile("Email Sent Successfully", "Upload_Click", "SendEmail");

                //strmessage = strmessage + Environment.NewLine + "SupplierID = " + ToSupplierID + "  " + "Email sent successfully";
            }
            catch (Exception ex)
            {
                // clserr.WriteLogToTxtFile("Attached file = " + Path.GetFileName(FileAttach), "SendMail", "SendEmail");
                strmessage = strmessage + Environment.NewLine + "Receipent = " + SendTo + "  " + "Email sending failed check error and audit log";
                //log enhance by chaitrali 3/7/2024
                logger.LogError(ex.Message + " For Message: " + strmessage + " ; UpdateDetails");
                //clserr.WriteLogToTxtFile("SupplierID = " + SendTo + "   " + SendTo + " Email Sending failed", "Upload_Click", "SendEmail");
                //clserr.Handle_Error(ex, "FrmPaymentInformation", "SendEmail");

            }
        }
       

    }
}

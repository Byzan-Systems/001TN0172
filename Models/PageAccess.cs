using HDFCMSILWebMVC.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC
{
  
    public interface AccessMenu
    {
        List<AccessItem> GetMenuItems(string user);
      
    }
    public class PageAccess : AccessMenu
    {
        public List<AccessItem> AccessList { get; set; }
        private readonly ILogger _logger;
        public PageAccess(ILogger<PageAccess> logger)
        {
            _logger = logger;
            AccessList = new List<AccessItem>();
            var userMaster = new AccessItem { Id = 0, PageName = "userMaster", IsAccessible = true };
            AccessList.Add(userMaster);
            var FinancerMaster = new AccessItem { Id = 1, PageName = "FinancerMaster", IsAccessible = false };
            AccessList.Add(FinancerMaster);
            var AccountDetails = new AccessItem { Id = 2, PageName = "AccountDetails", IsAccessible = false };
            AccessList.Add(AccountDetails);
            var ChangeTradeRefNo = new AccessItem { Id = 3, PageName = "ChangeTradeRefNo", IsAccessible = false };
            AccessList.Add(ChangeTradeRefNo);
            var EODProcess = new AccessItem { Id = 4, PageName = "EODProcess", IsAccessible = false };
            AccessList.Add(EODProcess);
            var PhysicalReceived = new AccessItem { Id = 5, PageName = "PhysicalReceived", IsAccessible = false };
            AccessList.Add(PhysicalReceived);
            var DownloadInvoice = new AccessItem { Id = 6, PageName = "DownloadInvoice", IsAccessible = false };
            AccessList.Add(DownloadInvoice);
            var UploadReceivedInvoiceData = new AccessItem { Id = 7, PageName = "UploadReceivedInvoiceData", IsAccessible = false };
            AccessList.Add(UploadReceivedInvoiceData);
            var TallyBookingFCC = new AccessItem { Id = 8, PageName = "TallyBookingFCC", IsAccessible = false };
            AccessList.Add(TallyBookingFCC);
            var AdditionalMISControlPoint = new AccessItem { Id = 9, PageName = "AdditionalMISControlPoint", IsAccessible = false };
            AccessList.Add(AdditionalMISControlPoint);
            var MISLiquidation = new AccessItem { Id = 10, PageName = "MISLiquidation", IsAccessible = false };
            AccessList.Add(MISLiquidation);
            var MISLiquidationDetailInvoice = new AccessItem { Id = 11, PageName = "MISLiquidationDetailInvoice", IsAccessible = false };
            AccessList.Add(MISLiquidationDetailInvoice);
            var CancelationInvoiceAndDO = new AccessItem { Id = 12, PageName = "CancelationInvoiceAndDO", IsAccessible = false };
            AccessList.Add(CancelationInvoiceAndDO);
            var Authorisation = new AccessItem { Id = 13, PageName = "Authorisation", IsAccessible = false };
            AccessList.Add(Authorisation);
            var PaymentInformationCSVNew = new AccessItem { Id = 14, PageName = "PaymentInformationCSVNew", IsAccessible = false };
            AccessList.Add(PaymentInformationCSVNew);
            var DOLiquidation = new AccessItem { Id = 15, PageName = "DOLiquidation", IsAccessible = false };
            AccessList.Add(DOLiquidation);
            var FTPayment = new AccessItem { Id = 16, PageName = "FTPayment", IsAccessible = false };
            AccessList.Add(FTPayment);
            var PaymentReport = new AccessItem { Id = 17, PageName = "PaymentReport", IsAccessible = false };
            AccessList.Add(PaymentReport);
            var DocumentReleasedCnf = new AccessItem { Id = 18, PageName = "DocumentReleasedCnf", IsAccessible = false };
            AccessList.Add(DocumentReleasedCnf);
            var PendingReport = new AccessItem { Id = 19, PageName = "PendingReport", IsAccessible = false };
            AccessList.Add(PendingReport);
            var Gefu = new AccessItem { Id = 20, PageName = "Gefu", IsAccessible = false };
            AccessList.Add(Gefu);
            var DoInformation = new AccessItem { Id = 21, PageName = "DoInformation", IsAccessible = false };
            AccessList.Add(DoInformation);
        }
        
        public List<AccessItem> GetMenuItems(string UserID)
        {
            //int newFileId;
            List<AccessItem> AccessList = new List<AccessItem>();
            DataSet dt = Methods.getDetails_Web("Get_SubMenuAccessAsper", UserSession.LoginID, "", "", "", "", "", "", _logger);
        

            return AccessList;
        }
    }
    public class AccessItem
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public bool IsAccessible { get; set; }
    }
}
     //public Boolean DownloadInvoice { get; set; }
     //public Boolean Setting { get; set; }
     //public Boolean Login { get; set; }
     //public Boolean Financer { get; set; }
     //public Boolean AccountDetails { get; set; }
     //public Boolean ChangeTradeRefNo { get; set; }
     //public Boolean EODProcess { get; set; }
     //public Boolean PhysicalReceived { get; set; }
     //public Boolean TradeOPS { get; set; }
     //public Boolean UploadReceivedInvoiceData { get; set; }
     //public Boolean TallyBookingFCC { get; set; }
     //public Boolean AdditionalMISControlPoint { get; set; }
     //public Boolean MISLiquidation { get; set; }
     //public Boolean MISLiquidationDetailInvoice { get; set; }
     //public Boolean CancelationInvoiceAndDO { get; set; }
     //public Boolean Authorisation { get; set; }
     //public Boolean CashOPS { get; set; }
     //public Boolean PaymentInformationCSVNew { get; set; }
     //public Boolean DOLiquidation { get; set; }
     //public Boolean FTPayment { get; set; }
     //public Boolean PaymentReport { get; set; }
     //public Boolean DocumentReleasedCnf { get; set; }
     //public Boolean PendingReport { get; set; }
     //public Boolean Gefu { get; set; }
     //public Boolean DoInformation { get; set; }
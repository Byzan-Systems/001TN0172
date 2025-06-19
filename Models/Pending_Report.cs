using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class ShowPending_Report
    {
        public bool chkSelectDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ReportType { get; set; }
        public string MISReportType { get; set; }

        public bool Status { get; set; }
    }

    public class PendingRet_InvoiceLevel
    {
        public string InvoiceNo { get; set; }
        public string InvoiceAmount { get; set; }
        public string? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string DeliveryOrderNo { get; set; }
        public string Payment_status { get; set; }
        public string PaymentReceiptMode { get; set; } 
        public string? PaymentReceiptDate { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }
        
    }

    public class PendingRet_PendingOrder
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? StepDate { get; set; }
        public string IMEX_DEAL_NUMBER { get; set; }

        public string Dealer_Code { get; set; }
        public string Dealer_Name { get; set; }

        public string Dealer_Address4 { get; set; }
        
    }
    public class PendingRet_PendingPayment
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? StepDate { get; set; }
        public string IMEX_DEAL_NUMBER { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string order_number { get; set; }
       
        public string Dealer_Code { get; set; }
        public string Dealer_Name { get; set; }

        public string Dealer_Address4 { get; set; }

    }

    
        public class PendingRet_FinancerName
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string DeliveryOrderNo { get; set; }
       
        public string Payment_status { get; set; }
        public string PaymentReceiptMode { get; set; }

        public string PaymentReceiptDate { get; set; }

        public string PaymentReferenceNo { get; set; }
        public string Financed_NotFinanced { get; set; }
        public string FinancierCode { get; set; }
        public string FinancerName { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }
    }

    public class PendingRet_NONFinancerName
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string DeliveryOrderNo { get; set; }

        public string Payment_status { get; set; }
        public string PaymentReceiptMode { get; set; }

        public string PaymentReceiptDate { get; set; }

        public string PaymentReferenceNo { get; set; }
        public string Financed_NotFinanced { get; set; }
        public string FinancierCode { get; set; }
        public string FinancerName { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }
    }

    public class PendingRet_BothFinancerName
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public string? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string DeliveryOrderNo { get; set; }

        public string Payment_status { get; set; }
        public string PaymentReceiptMode { get; set; }

        public string PaymentReceiptDate { get; set; }

        public string PaymentReferenceNo { get; set; }
        public string Financed_NotFinanced { get; set; }
        public string FinancierCode { get; set; }
        public string FinancerName { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }
        //public List<PendingRet_BothFinancerName> LstPendingRet_BothFinancerName { get; set; }
    }

    public class PendingRet_DO_CancellationAndRetainInvoiceReport
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        public string OrderUpdationStatus { get; set; }
        public string DeliveryOrderNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }

    }

    public class PendingRet_DOAndInvoiceCancellationReport
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }
        
        public string DeliveryOrderNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }

    }

    public class PendingRet_InvoiceCancellationReport
    {
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? PhysicallyReceiptDate { get; set; }
        public string TradeReferenceNo { get; set; }       
        public string DeliveryOrderNo { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string Location { get; set; }

    }

}

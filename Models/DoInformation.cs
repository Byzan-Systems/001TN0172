using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class DOInformationModels
    {
        public string DONumber { get; set; }
        public string InvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public Boolean IsSelect { get; set; }
    }

    public class InvoiceDetails
    {
        [Display(Name = "Invoice Number")]
        public string Invoice_Number { get; set; }
        [Display(Name = "Amount")]
        public string Invoice_Amount { get; set; }
        [Display(Name = "Invoice Received Date")]
        public string Invoice_data_Received_Date { get; set; }
        [Display(Name = "Trade Ref. Number")]
        public string IMEX_DEAL_NUMBER { get; set; }
        [Display(Name = "Order Received Date")]
        public string Order_Data_Received_Date { get; set; }
        [Display(Name = "Payment Recieved Date")]
        public string Payment_Received_Date { get; set; }
        [Display(Name = "UTR Number")]
        public string Utr_Number { get; set; }
        [Display(Name = "Status")]
        public string Invoice_Status { get; set; }
        [Display(Name = "Order INV ID")]
        public string Ord_Inv_ID { get; set; }
        [Display(Name = "Cash Ops ID")]
        public string Cash_Ops_ID { get; set; }
        [Display(Name = "F1 MIS")]
        public int F1_MIS { get; set; }
        [Display(Name = "F2 MIS")]
        public int F2_MIS { get; set; }
        [Display(Name = "F3 MIS")]
        public int F3_MIS { get; set; }
    }

    public class OrderDetails
    {
        [Display(Name = "Order Date")]
        public string Order_Date { get; set; }
        [Display(Name = "DO Date")]
        public string Do_Date { get; set; }
        [Display(Name = "Financer Code")]
        public string Financier_Code { get; set; }
        [Display(Name = "Financer Name")]
        public string Financier_Name { get; set; }
        [Display(Name = "Amount")]
        public string Order_Amount { get; set; }
        [Display(Name = "Status")]
        public string Order_Status { get; set; }
        [Display(Name = "Order Received Date")]
        public string Order_Data_Received_On { get; set; }
        [Display(Name = "Cash Ops ID")]
        public string Cash_Ops_ID { get; set; }
        [Display(Name = "Rej. Reason")]
        public string Ord_Rej_Reason { get; set; }
        
    }
    public class CashOPSDetails
    {
        [Display(Name = "Cash Ops ID")]
        public string Cash_Ops_ID { get; set; }
        [Display(Name = "File Type")]
        public string CashOps_FileType { get; set; }
        [Display(Name = "UTR Number")]
        public string UTR_No { get; set; }
        [Display(Name = "Amount")]
        public string Transaction_Amount { get; set; }
        [Display(Name = "Status")]
        public string Payment_Status { get; set; }
        [Display(Name = "CashOps Date")]
        public string Cash_Ops_Date { get; set; }
        [Display(Name = "DRC Generation")]
        public string DRC_Generation { get; set; }
        [Display(Name = "FNCR Virtual Account")]
        public string FNCR_Virtual_Account { get; set; }
        [Display(Name = "IFSC Code")]
        public string IFSC_code { get; set; }
        [Display(Name = "FNCR Code")]
        public string FNCR_code { get; set; }
        [Display(Name = "FNCR Name")]
        public string FNCR_Name { get; set; }
       
    }
    public class PaymentDetails
    {
        [Display(Name = "Amount")]
        public string Entry_Amount { get; set; }
        [Display(Name = "Product")]
        public string Product { get; set; }
        [Display(Name = "Party Code")]
        public string Party_Code { get; set; }
        [Display(Name = "Party Name")]
        public string Party_Name { get; set; }
        [Display(Name = "Remitting Bank")]
        public string RemittingBank { get; set; }
        [Display(Name = "UTR Number")]
        public string UTR_No { get; set; }
        [Display(Name = "IFSC Code")]
        public string IFSC_Code { get; set; }
        [Display(Name = "Dealer Name")]
        public string Dealer_Name { get; set; }
        [Display(Name = "Upload Date Time")]
        public string UploadDateTime { get; set; }
      
    }
    public class DOInformationViewModel
    {
        public List<InvoiceDetails> Invoicelist { get; set; }
        public List<OrderDetails> orderlist { get; set; }
        public List<CashOPSDetails> CashopsList { get; set; }
        public List<PaymentDetails> PaymentList { get; set; }
    }
}

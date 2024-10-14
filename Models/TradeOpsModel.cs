using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
   
    public class DownloadFillInvoice
    {
        [Display(Name = "Select")]
        public bool IsSelect { get; set; }
        [Display(Name = "Invoice Number")]
        public string Invoice_Number { get; set; }
        [Display(Name = "Invoice Amount")]
        public string Invoice_Amount { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [Display(Name = "Vehical ID")]
        public string Vehical_ID { get; set; }
        [Display(Name = "Due Date")]
        public string DueDate { get; set; }
        [Display(Name = "Dealer Name")]
        public string Dealer_Name { get; set; }
        [Display(Name = "Dealer Address1")]
        public string Dealer_Address1 { get; set; }
        [Display(Name = "Dealer City")]
        public string Dealer_City { get; set; }
        [Display(Name = "Transporter Name")]
        public string Transporter_Name { get; set; }
        [Display(Name = "Transport Number")]
        public string Transport_Number { get; set; }
        [Display(Name = "Transport Date")]
        public string Transport_Date { get; set; }
        [Display(Name = "Dealer Code")]
        public string Dealer_Code { get; set; }
        [Display(Name = "Transporter Code")]
        public string Transporter_Code { get; set; }
        [Display(Name = "Dealer Address2")]
        public string Dealer_Address2 { get; set; }
        [Display(Name = "Dealer Address3")]
        public string Dealer_Address3 { get; set; }
        [Display(Name = "Dealer Address4")]
        public string Dealer_Address4 { get; set; }
        [Display(Name = "IMEX Number")]
        public string? IMEX_DEAL_NUMBER { get; set; }
        [Display(Name = "Invoice Date")]
        public string TradeOp_Selected_Invoice_Date { get; set; }
        [Display(Name = "Trade OPs Remark")]
        public string Trade_OPs_Remarks { get; set; } 
        
    }

    

    public class DownloadinvFrmDeta
    {
        public bool ChkInvoiceNo { get; set; }
        public bool ChkDate { get; set; }
        public bool ChkReporttype { get; set; }
        public string Invoice_Number { get; set; }
        public string  DateFrom { get; set; }
        public string  DateTo { get; set; }
        public string RerportType { get; set; }


        
    }
    public class TallyBookingFCC
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }
    public class TallyBookingFCCData
    {
        [Display(Name = "Invoice Count")]
        public string INV_Count { get; set; }
        [Display(Name = "Dealer Name")]
        public string Dealer_Name { get; set; }
        [Display(Name = "Dealer Code")]
        public string Dealer_Code { get; set; }
        [Display(Name = "IMEX Deal Number")]
        public string IMEX_DEAL_NUMBER { get; set; }
        [Display(Name = "Step Date")]
        public string StepDate { get; set; }
        [Display(Name = "Invoice Amount")]
        public double Inv_Amt { get; set; }
        

    }

}

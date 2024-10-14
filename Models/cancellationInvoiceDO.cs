using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
   public class cancellationInvDORetainInv
    {
       
        public bool IsSelect { get; set; }
        public string DO_number { get; set; }
      
        public string Do_Date { get; set; }
       
        public string Dealer_Code { get; set; }
      

        public string Dealer_Destination_Code { get; set; }
      

        public string Dealer_Outlet_Code { get; set; }
      
        public string Order_Amount { get; set; }
        

     
        public string Order_ID { get; set; }
        
    }

    public class cancellationoderNoInv
    {
        public bool IsSelect { get; set; }
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public string DO_Number { get; set; }

       
    }

    public class cancellationInvoicesonly
    {
        public bool IsSelect { get; set; }
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
    }
    public class ShowCancelationInvoiceAndDO
    {
        public bool selectdate { get; set; }
        public bool chkReportType { get; set; }
        //public DateTime? DateFrom { get; set; }
        //public DateTime? DateTo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string RerportType { get; set; }

    }

    [Table("Order_Desc")]
    public class Order_Des
    {
        [Key]
        public string Order_ID { get; set; }
        public DateTime? Order_Date { get; set; }
        public string Record_Identifier { get; set; }
        public string DO_Number { get; set; }
        public string Do_Date { get; set; }
        public string Dealer_Code { get; set; }
        public string Dealer_Destination_Code { get; set; }
        public string Dealer_Outlet_Code { get; set; }
        public string Financier_Code { get; set; }
        public string Financier_Name { get; set; }
        public string Email_IDs { get; set; }
        public string Order_Amount { get; set; }
        public string Order_Status { get; set; }
        public string Order_Data_Received_On { get; set; }
        public string Cash_Ops_ID { get; set; }
        public int? Ord_Rej_Flag { get; set; }
        public string Ord_Rej_Reason { get; set; }


    }

    [Table("invoice_cancel_desc")]
    public class invoice_cancel_desc
    {
        public string Invoice_ID { get; set; }
        public string Sr_No { get; set; }
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
        public DateTime? Requested_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string DO_number { get; set; }
        public string Order_ID { get; set; }
        public int? Cancelled_Flag { get; set; }
        public string Reason { get; set; }
        public int? F2_MIS { get; set; }
        public int? DO_IN_Flag { get; set; }
        public int? Authorize_Flag { get; set; }
        public string Deleted_By { get; set; }

        public DateTime? Deleted_On { get; set; }
        public string Authorized_By { get; set; }
        public DateTime? Authorized_On { get; set; }

    }

    [Table("invoice")]
    public class invoice
    {
        public string Invoice_ID { get; set; }    
        public string Invoice_Number { get; set; }
        public string Invoice_Amount { get; set; }
   
    

    }
    public class Order_Invoice_Order_Des
    {
        public string Ord_Inv_ID { get; set; }
        public string Order_ID { get; set; }
        public string Record_Identifier { get; set; }
        public string Order_Inv_Number { get; set; }
        public string Order_Inv_Amount { get; set; }
        public string Order_Inv_Status { get; set; }
        //public string Order_Date { get; set; }
        public DateTime? Order_Date { get; set; }
        public string DO_Number { get; set; }
        public string Do_Date { get; set; }
        public string Dealer_Code { get; set; }
        public string Dealer_Destination_Code { get; set; }
        public string Dealer_Outlet_Code { get; set; }
        public string Financier_Code { get; set; }
        public string Financier_Name { get; set; }
        public string Email_IDs { get; set; }
        public string Order_Amount { get; set; }
        public string Order_Status { get; set; }
        public string Order_Data_Received_On { get; set; }
        public string Cash_Ops_ID { get; set; }
        //public string Ord_Rej_Flag { get; set; }
        public int Ord_Rej_Flag { get; set; }
        public string Ord_Rej_Reason { get; set; }

    }

    [Keyless]
    public class Order_Invoice_Order_DesForCanfrm
    {
        //public string Ord_Inv_ID { get; set; }
        //public string Order_ID { get; set; }
        //public string Record_Identifier { get; set; }
        public string Order_Inv_Number { get; set; }
        //public string Order_Inv_Amount { get; set; }
        //public string Order_Inv_Status { get; set; }
        //public string Order_IDnew { get; set; }
        //public string Order_Date { get; set; }
        //public string Record_Identifiernew { get; set; }
        public string DO_Number { get; set; }
        //public string Do_Date { get; set; }
        //public string Dealer_Code { get; set; }
        //public string Dealer_Destination_Code { get; set; }
        //public string Dealer_Outlet_Code { get; set; }
        //public string Financier_Code { get; set; }
        //public string Financier_Name { get; set; }
        //public string Email_IDs { get; set; }
        //public string Order_Amount { get; set; }
        //public string Order_Status { get; set; }
        //public string Order_Data_Received_On { get; set; }
        //public string Cash_Ops_ID { get; set; }
        //public string Ord_Rej_Flag { get; set; }
        //public string Ord_Rej_Reason { get; set; }

    }



    public class AuthorizInvDORetainInv
    {
        [Display(Name = "Select")]
        public bool IsSelect { get; set; }
        [Display(Name = "DO Number")]
        public string DO_number { get; set; }
        [Display(Name = "Do Date")]
        public string Do_Date { get; set; }
        [Display(Name = "Dealer Code")]
        public string Dealer_Code { get; set; }
        [Display(Name = "Dealer Destination Code")]
        public string Dealer_Destination_Code { get; set; }
        [Display(Name = "Dealer Outlet Code")]
        public string Dealer_Outlet_Code { get; set; }
        [Display(Name = "Order Amount")]
        public string Order_Amount { get; set; }
        [Display(Name = "DO Number")]
        public string DONumber { get; set; }
    }

    public class AuthorizoderNoInv
    {
        [Display(Name = "Select")]
        public bool IsSelect { get; set; }
        [Display(Name = "DO Number")]
        public string DONumber { get; set; }
        [Display(Name = "Invoice Number")]
        public string Invoice_Number { get; set; }
        [Display(Name = "Invoice Amount")]
        public string Invoice_Amount { get; set; }
        [Display(Name = "DO Number")]
        public string DO_Number { get; set; }
        [Display(Name = "DO Number")]
        public string Invoice_ID { get; set; }


    }

    public class AuthorizInvoicesonly
    {
        [Display(Name = "Select")]
        public bool IsSelect { get; set; }
        [Display(Name = "Invoice Number")]
        public string Invoice_Number { get; set; }
        [Display(Name = "Invoice Amount")]
        public string Invoice_Amount { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Invoice ID")]
        public string Invoice_ID { get; set; }
    }

    public class ShowAuthoriz
    {
     
        public string Reporttype { get; set; }
    }
   
    public class AuthOrder_Des
    {
   
        public string Order_ID { get; set; }
        public DateTime? Order_Date { get; set; }
        public string Record_Identifier { get; set; }
        public string DO_Number { get; set; }
        public string Do_Date { get; set; }
        public string Dealer_Code { get; set; }
        public string Dealer_Destination_Code { get; set; }
        public string Dealer_Outlet_Code { get; set; }
        public string Financier_Code { get; set; }
        public string Financier_Name { get; set; }
        public string Email_IDs { get; set; }
        public string Order_Amount { get; set; }
        public string Order_Status { get; set; }
        public string Order_Data_Received_On { get; set; }
        public string Cash_Ops_ID { get; set; }
        public int? Ord_Rej_Flag { get; set; }
        public string Ord_Rej_Reason { get; set; }


    }



}

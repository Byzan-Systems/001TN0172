using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class AddMISControlPoint_INVDate
    {
        public string Invoice_data_Received_Date { get; set; }
        public string Total_Invoice { get; set; }
    }

    public class AddMISControlPoint_PHyInvRec
    {
        public string PHyInvRec { get; set; }
        public string StepDate { get; set; }
    }

    public class AddMISControlPoint_Order_Rec
    {
        public string Order_Rec { get; set; }

    }
    public class AddMISControlPoint_Payment_Recev
    {
        public string Payment_Recev { get; set; }

    }

    public class showAddMISControlPoint
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class showAddMISControlPointDT
    {
        [Display(Name = "Invoice Received Date")]
        public string Invoice_data_Received_Date { get; set; }
        [Display(Name = "Total Invoices")]
        public string TOTAL_Invoice { get; set; }
        [Display(Name = "Step Date")]
        public string stepdate { get; set; }
        [Display(Name = "Physical Invoice Received")]
        public string PHyInvRec { get; set; }
        [Display(Name = "Order Record")]
        public string Order_Rec { get; set; }
        [Display(Name = "Payment Received")]
        public string Payment_Recev { get; set; }

    }

}

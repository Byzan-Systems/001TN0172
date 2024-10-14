using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class MISLiquidationDetailInvoice
    {
        [Display(Name = "IMEX Number")]
        public string imex_deal_number { get; set; }
        [Display(Name = "payment Received Date")]
        public string  payment_received_date { get; set; }
        [Display(Name = "invoice Number")]
        public string invoice_number { get; set; }
        [Display(Name = "Consolidated Amount")]
        public double Consolidated_Amount { get; set; }
        [Display(Name = "payment Time")]
        public string payment_Time { get; set; }
    }

    public class ShowMISLiquidationDetailInvoice
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndtDate { get; set; }
     
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class PaymentReport
    {
    }
    public class ShowPaymentReportSelect
    {
        public bool chkpayrecDate { get; set; }
        public bool Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string  ReportType { get; set; }
    }

    public class ShowCashOps_Upload
    {
        public string Cash_Ops_ID { get; set; }
        public string UTR_No { get; set; }
        public string Virtual_Account { get; set; }
        public string Transaction_Amount { get; set; }
        public DateTime Cash_Ops_Date { get; set; }
        public string Payment_Status1 { get; set; }
        public string PaymentStatus2 { get; set; }
    }

}

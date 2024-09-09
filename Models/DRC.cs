using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _001TN0172.Models
{
    public class DRC
    {
        public string DRCUTRNo { get; set; }
        public string DRCVirtualAcc { get; set; }
        public string DRCUTRCm { get; set; }
        public bool IsSelect { get; set; }
    }


    public class DocumentReleasedCnfNew
    {

        
        public string Virtual_Account { get; set; }
        public string UTR_No { get; set; }
        

    }

    //public class DocumentReleasedCnfNew
    //{ 
        
    //    public string Cash_Ops_ID { get; set; }
    //    public string FileID { get; set; }
    //    public string CashOps_FileType { get; set; }
    //    public string NEFT_RTGS_BT_ID { get; set; }
    //    public string Virtual_Account { get; set; }
    //    public string UTR_No { get; set; }
    //    public string Transaction_Amount { get; set; }
    //    public string Transaction_status { get; set; }
    //    public string Payment_Status { get; set; }
    //    public int? Attempt { get; set; }
    //    public DateTime? Cash_Ops_Date { get; set; }
    //    public DateTime ? Cash_Ops_Time { get; set; }
    //    public float? GEFO_Flag { get; set; }
    //    public DateTime ? GEFO_Date { get; set; }
    //    public string DR_Account_No { get; set; }
    //    public string CR_Account_No { get; set; }
    //    public string LoginID { get; set; }
    //    public int? EOD_MailFlag { get; set; }
    //    public string DRC_Generation { get; set; }
    //    public string FNCR_Virtual_Account { get; set; }
    //    public string IFSC_code { get; set; }
    //    public string FNCR_code { get; set; }
    //    public string FNCR_Name { get; set; }

    //}
}

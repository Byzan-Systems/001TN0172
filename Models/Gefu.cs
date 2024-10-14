using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    
    public class ShowGefuSelect
    {
        public DateTime SelectDate { get; set; }
    }

    public class Gefu
    {
        //public string Cash_Ops_ID { get; set; }
        //public string Account_No { get; set; }
        //public string Account_ini { get; set; }
        //public string DeCR_Status { get; set; }
        //public string UTR_No { get; set; }
        //public string Virtual_Account { get; set; }
        ////public DateTime? SelectDate { get; set; }
        //public string SelectDate { get; set; }


        public string Cash_Ops_ID { get; set; }
        public string Dr_Acc { get; set; }
        public string CR_Acc { get; set; }
        public string Dr_Acc1 { get; set; }
        public string Cr_Acc1 { get; set; }
        public string Debitstatus { get; set; }
        public string Creditstatus { get; set; }
        public string Transaction_Amount { get; set; }
        public string UTR_No { get; set; }
        public string Virtual_Account { get; set; }

    }


    //public class GefuList
    //{
    //    public string Cash_Ops_ID { get; set; }
    //    public string Dr_Acc { get; set; }
    //    public string CR_Acc { get; set; }
    //    public string Dr_Acc1 { get; set; }
    //    public string Cr_Acc1 { get; set; }
    //    public string Debitstatus { get; set; }
    //    public string Creditstatus { get; set; }
    //    public string Transaction_Amount { get; set; }
    //    public string UTR_No { get; set; }
    //    public string Virtual_Account { get; set; }
       

    //}



}

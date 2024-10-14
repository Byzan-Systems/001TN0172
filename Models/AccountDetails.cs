using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{

    //[Table("AccountDetails")]
    public class AccountDetails
    {      
        //[Key]
        public string Payment_Type { get; set; }
        public string DR_Account_No { get; set; }
        public string CR_Account_No { get; set; }

       // public List<AccountDetails> paylist { get; set; }
        //public IEnumerable<SelectListItem> paytypeListItems
        //{
        //    get { return new SelectList(paylist, "0", "BT"),new SelectList(paylist, "1", "NEFT"),new SelectList(paylist, "2", "RTGS"); }
        //}
    }

    
    public class DBAccountDetails
    {
     
        public string Payment_Type { get; set; }
        public string DR_Account_No { get; set; }
        public string CR_Account_No { get; set; }
    }

}

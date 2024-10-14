using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class ControlDOLiquidation
    {
        public string DoNumber { get; set; }

        public string DoNumberNew { get; set; }
        public string DoAmount { get; set; }
        public string TotalNumberofinvoice { get; set; }
        public string DealerName { get; set; }
        public string FinancerDetails { get; set; }
        public string PaymentType { get; set; }
        public string PaymentRefno { get; set; }
        public string Remarks { get; set; }
        public List<Itemlist> PaymentTypeList { get; set; }
        public ControlDOLiquidation()
        {
            PaymentTypeList = new List<Itemlist>() {
        new Itemlist { Type = "RTGS", Values = 1 },
        new Itemlist { Type = "NEFT", Values = 2 },
        new Itemlist { Type = "FT", Values = 3 },
        new Itemlist { Type = "Others", Values = 4 }
        };
        }
    }  
    public class Itemlist
    {
        public string Type { get; set; }
        public int Values { get; set; }
    }
 

}

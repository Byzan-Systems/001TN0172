using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class TradeRefNo
    {
        public string TradeRefNum { get; set; }
        public string InvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public Boolean IsSelect { get; set; }
    }

    public class ShowTradeRefNo
    {
        public string TradeRefNum { get; set; }
    }

    public class updateNewTradeRefNo
    {
        public string Idname { get; set; }
    }

}

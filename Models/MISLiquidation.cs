using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
   public class MISLiquidation
    {
        [Display(Name = "IMEX Number")]
        public string  IMEX_DEAL_NUMBER { get; set; }
        [Display(Name = "Step Date")]
        public string stepdate { get; set; }
        [Display(Name = "Consolidated Amount")]
        public double? Consolidated_Amount { get; set; }
        [Display(Name = "Payments Before")]
        public double? Payments_before { get; set; }
        [Display(Name = "Payments Ondate")]
        public double? Payments_ondate { get; set; }

    }
    public class ShowMISLiquidation
    {
        public DateTime? TradeStartDate { get; set; }
        public DateTime? TradeEndtDate { get; set; }
        public DateTime? DateAsOn { get; set; }
      

    }

    //public class ViewModel
    //{
    //    public IEnumerable<MISLiquidation> MISLiquidations { get; set; }
    //    public ShowMISLiquidation ShowMISLiquidations { get; set; }
    //}

}

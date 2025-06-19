using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    [Table("Stockyard_Master")]
    public class StockyardModel
    {
        public int? ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Display(Name = "DO_Number")]
        public string DO_Number { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "IsActive")]
        public string IsActive { get; set; }

    }
    public class StockyardMaster
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DO_Number { get; set; }
        public string Email { get; set; }        
        public string CreatedBy { get; set; }
        public DateTime Created_Date { get; set; }
    }

    public class ShowStockyardMaster
    {
        public int? ID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Display(Name = "DO_Number")]
        public string DO_Number { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "IsActive")]
        public string IsActive { get; set; }

    }




}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    [Table("FinancerDetails")]
    public class FinancerModel
    {
        public int? ID { get; set; }

        [Display(Name = "Financer Type")]
        public string FType { get; set; }
        [Display(Name = "Financer Name")]
        public string FName { get; set; }
        [Display(Name = "IFSC Part")]
        public string IFSCPart { get; set; }
        [Display(Name = "Financer Code")]
        public string FCode { get; set; }
        
    }
    public class FinancerMaster
    {
        public int? ID { get; set; }
        public string FType { get; set; }
        public string FName { get; set; }
        public string IFSCPart { get; set; }
        public string FCode { get; set; }        
        public string CreatedBy { get; set; }
        public DateTime Created_Date { get; set; }
    }

    public class ShowFinancerMaster
    {
        public int? ID { get; set; }
        [Display(Name = "Financer Type")]
        public string FType { get; set; }
        [Display(Name = "Financer Name")]
        public string FName { get; set; }
        [Display(Name = "IFSC Part")]
        public string IFSCPart { get; set; }
        [Display(Name = "Financer Code")]
        public string FCode { get; set; }
    
    }

    


}

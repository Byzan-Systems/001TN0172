using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace _001TN0172.Models
{
    [Table("Login")]
    public class LoginMST
    {
        [Key]
        public int? LoginID { get; set; }
        public string LoginName { get; set; }
        public string LoginType { get; set; }
        public string Password { get; set; }
        public int? Record_Chk_Flag { get; set; }
        public int? Login_Enable { get; set; }

    }
 

    public class RegisterNew
    {
        public string LoginName { get; set; }
        public string LoginType { get; set; }
        public string Password { get; set; }
        public string ReTypePassword { get; set; }
        public int Record_Chk_Flag { get; set; }
        public int? Login_Enable { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime Created_Date { get; set; }
    }


    public class UserMaster
    {
        public int? LoginID { get; set; }
        public string LoginName { get; set; }
        public string LoginType { get; set; }
        public string Password { get; set; }
        public string ReTypePassword { get; set; }
        public int? Record_Chk_Flag { get; set; }
        public int? Login_Enable { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Created_Date { get; set; }
    }

    public class ShowUserMaster
    {
        public int? LoginID { get; set; }
        public string LoginName { get; set; }
        public string LoginType { get; set; }       
        public int? Login_Enable { get; set; }
    }

    public class DetailsUserMaster
    {
        public int? LoginID { get; set; }
        public string LoginName { get; set; }
        public string LoginType { get; set; }
        public int? Login_Enable { get; set; }
    }

    [Table("UAM_LoginLogout")]
    public class UAM_LoginLogout
    {
        [Key]
        public string LogID { get; set; }
        public string User_id { get; set; }
        public string LoginDate_Time { get; set; }
        public string LogoutDate_Time { get; set; }


    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
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
    [Table("user_mst_temp")]
    public class user_mst_temp
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string Password { get; set; }
        public int? Role { get; set; }
        public string IsActive { get; set; }
        public string Maker_Checker { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string LastLogin { get; set; }
        public int? Attempts { get; set; }
        public string LogoutTime { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string UserType { get; set; }
        public string RoleName { get; set; }
        public string EmpType { get; set; }
        public string Remark { get; set; }
        public DateTime? MSIL_LogoutTime { get; set; }
       

        
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
    }
    public class BrowserCloseModel
    {
        public string isclick { get; set; }
    }
    public class UAM_LoginLogoutExist
    {
        public string MSIL_LogoutDatetime { get; set; }
        public string LogID { get; set; }
}
}

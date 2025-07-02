using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Controllers
{
    public static class UserSession
    {
        public  static  string LoginID { get; set; }
    }
  
    public static class IsLogoutSession
    {
        // Static property
        public static int IsLogout { get; set; }

        // Static constructor for initialization
        static IsLogoutSession()
        {
            // Initialize the static property
            IsLogout = -1; // String values should be in quotes
        }
    }

}

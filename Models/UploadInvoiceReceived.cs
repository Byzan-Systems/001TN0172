using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Models
{
    public class UploadInvoiceReceived
    {
        public IFormFile txtFile { get; set; }
    }
    public class UploadInvoiceReceivedData
    {
        public string File { get; set; }
    }
}

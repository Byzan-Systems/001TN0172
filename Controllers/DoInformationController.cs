using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HDFCMSILWebMVC.Controllers
{
    public class DoInformationController : Controller
    {
        public static IList<DOInformationModels> tsradeRefNo = new List<DOInformationModels>();
        private readonly ILogger _logger;
        public DoInformationController(ILogger<ChangeTradeRefNoController> logger)
        {
            _logger = logger;
        }

        public IActionResult DoInformationDetails()
        {
            //if (HttpContext.Session.GetString("LoginID") == null)
            //{ return RedirectToAction("LoginPage", "Login"); }
            //else
            //{
            return View("DoDetails");
            //}

        }

        [HttpPost]
        public IActionResult Show(DOInformationModels req)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (req.DONumber == "" || req.DONumber == null)
                {
                    TempData["alertMessage"] = "Please Enter DO  Number";
                    // return View("ViewGenerateDRC", DS);
                    return View("DoDetails");
                }
                else if ((req.DONumber != "") && (CheckForSpecial(req.DONumber) == false))
                {
                    TempData["alertMessage"] = "Trade Reffrence Number Should be AlphaNumeric only"; return View("DOInformation");
                }

                return View(GetDoDetails(req.DONumber));
            }
        }

        public DOInformationViewModel GetDoDetails(string do_number)
        {
            var viewModel = new DOInformationViewModel
            {
                Invoicelist = new List<InvoiceDetails>(),
                orderlist = new List<OrderDetails>(),
                CashopsList = new List<CashOPSDetails>(),
                PaymentList = new List<PaymentDetails>()
            };

            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    // Define SQL commands with parameter placeholders
                    var invoiceQuery = "EXEC SP_GetDetails_Web @Task = 'Get_DoDetailsAsInvoice', @Search1 = '"+do_number+"', @Search2 = '', @Search3 = '', @Search4 =  '', @Search5 =  '', @Search6 =  '', @Search7 =  ''";
                    var paymentQuery = "EXEC SP_GetDetails_Web @Task = 'Get_DoDetailsAsPayment', @Search1 = '" + do_number + "', @Search2 =  '', @Search3 =  '', @Search4 =  '', @Search5 =  '', @Search6 =  '', @Search7 =  ''";

                    viewModel.Invoicelist = db.Set<InvoiceDetails>().FromSqlRaw(invoiceQuery).ToList();
                    var orderQuery = "EXEC SP_GetDetails_Web @Task = 'Get_DoDetailsAsOrder', @Search1 = '" + do_number + "', @Search2 =  '', @Search3 =  '', @Search4 =  '', @Search5 =  '', @Search6 =  '', @Search7 =  ''";

                    viewModel.orderlist = db.Set<OrderDetails>().FromSqlRaw(orderQuery).ToList();
                    var cashOpsQuery = "EXEC SP_GetDetails_Web @Task = 'Get_DoDetailsAsCashOps', @Search1 = '" + do_number + "', @Search2 =  '', @Search3 =  '', @Search4 =  '', @Search5 =  '', @Search6 =  '', @Search7 =  ''";
 //viewModel.CashopsList = db.Set<CashOPSDetails>().FromSqlRaw(cashOpsQuery).ToList();
                    viewModel.CashopsList = db.Set<CashOPSDetails>().FromSqlInterpolated($@"EXEC SP_GetDetails_Web  @Task = {"Get_DoDetailsAsCashOps"},  @Search1 = {do_number ?? ""},  @Search2 = {""},   @Search3 = {""},  @Search4 = {""},   @Search5 = {""},   @Search6 = {""},   @Search7 = {""}").ToList();
                   
                    viewModel.PaymentList = db.Set<PaymentDetails>().FromSqlRaw(paymentQuery).ToList();

                }
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "DoInformationController; GetDoDetails");
            }

            return viewModel;
        }

        public Boolean CheckForSpecial(string donumber)
{
    for (int i = 0; i < donumber.Length; i++)
    {
        if (!(char.IsLetter(donumber[i])) && (!(char.IsNumber(donumber[i]))))
            return false;
    }
    return true;
}

    }
}

using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Data;
using MoreLinq;

namespace HDFCMSILWebMVC.Controllers
{
    public class ChangeTradeRefNoController : Controller
    {
       public static IList<TradeRefNo> tsradeRefNo = new List<TradeRefNo>();
        private readonly ILogger _logger;
        public ChangeTradeRefNoController(ILogger<ChangeTradeRefNoController> logger)
        {
            _logger = logger;
        }

        public IActionResult ChangeTradeRefNo()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                return View();
            }
        
        }

        
        public ActionResult UpdateTradeRefNo(updateNewTradeRefNo TradeRefNo, string[] IsSelect)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                try
                {
                    if (IsSelect.Length == 0)
                    {

                        TempData["alertMessage"] = "Please Select at least one Record to update.";
                        return RedirectToAction("Show");
                    }
                    for (int i = 0; i < IsSelect.Length; i++)
                    {
                        string InvoiceNumber = IsSelect[i].ToString();
                        using (var db = new Entities.DatabaseContext())
                        {
                            var DetailsList = tsradeRefNo.ToList();
                            DataTable Details = DetailsList.ToDataTable();
                            DataRow[] dtFilter = Details.Select("[InvoiceNo]='" + InvoiceNumber + "'");
                            DataTable dtFilterData = dtFilter.CopyToDataTable();

                            db.Database.ExecuteSqlRaw("update Invoice set IMEX_DEAL_NUMBER='" + TradeRefNo.Idname.ToString() + "'  where Invoice_Status='PHYSICAL INV REC' and  IMEX_DEAL_NUMBER='" + dtFilterData.Rows[0]["TradeRefNum"].ToString().Trim() + "' and Invoice_ID='" + dtFilterData.Rows[0]["InvoiceID"].ToString().Trim() + "' and  Invoice_Number='" + dtFilterData.Rows[0]["InvoiceNo"].ToString().Trim() + "'");
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - ChangeTradeRefNoController;ChangeTradeRefNo");
                }
                TempData["alertMessage"] = "TradeRefNo has been updated successfully.";
                return RedirectToAction("ChangeTradeRefNo");
            }

        }

        [HttpPost]
        public IActionResult Show(TradeRefNo req)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("Logout", "Login"); }
            else
            {
                if (req.TradeRefNum == "" || req.TradeRefNum == null)
                {
                    TempData["alertMessage"] = "Please Enter TradeRef  Number";
                    // return View("ViewGenerateDRC", DS);
                    return View("ChangeTradeRefNo");
                }
                else if ((req.TradeRefNum != "") && (CheckForSpecial(req.TradeRefNum) == false))
                {
                    TempData["alertMessage"] = "Trade Reffrence Number Should be AlphaNumeric only"; return View("ChangeTradeRefNo");
                }

                return View(GetTradeRefNoList(req.TradeRefNum));
            }
        }

        public IList<TradeRefNo> GetTradeRefNoList(string traderefno)
        {
            //List<TradeRefNo> tsradeRefNo = new List<TradeRefNo>();

            try
            {
                using (var db = new Entities.DatabaseContext())
                {
                    tsradeRefNo = db.Set<TradeRefNo>().FromSqlRaw("EXEC uspChangeTradeRefNo @TradeRefNo ='" + traderefno.ToString() + "',@Flag=1  ").ToList();
                }

                _logger.LogInformation("Executed successfully" + " - ChangeTradeRefNoController; GetTradeRefNoList");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString() + " - ChangeTradeRefNoController;GetTradeRefNoList");
            }

            return tsradeRefNo;
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

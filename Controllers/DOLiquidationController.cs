using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Controllers
{
    public class DOLiquidationController : Controller
    {
        ControlDOLiquidation model = new ControlDOLiquidation();
        public Boolean ChkFlag = false;
        private readonly ILogger _logger;
        private IHostingEnvironment _environment;
        public DOLiquidationController(ILogger<DOLiquidationController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        public ActionResult ShowDOLiquidation()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View(model);
            }
           
        }
        [HttpPost]
        public ActionResult ShowPayInfo(ControlDOLiquidation model)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                    if (model.DoNumberNew == "" || model.DoNumberNew == null)
                    { TempData["alertMessage"] = "Please Enter DO Number"; return View("ShowDOLiquidation", model); }
                    else if (model.DoNumberNew != "" && model.DoNumberNew.Length > 22)
                    {
                        TempData["alertMessage"] = "Length of DO Number should be 22"; return View("ShowDOLiquidation", model);
                    }
                    else if ((model.DoNumberNew != "") && (CheckForSpecial(model.DoNumberNew) == false))
                    {
                        TempData["alertMessage"] = "DO Number Should be AlphaNumeric only"; return View("ShowDOLiquidation", model);
                    }
                    else
                    {
                        DataSet DS = Methods.getDetails_Web("Get_DoLiquidationDataAsPer", model.DoNumberNew.ToString(), "", "", "", "", "", "", _logger);
                        if (DS.Tables[0].Rows.Count > 0)
                        {
                            if (DS.Tables[0].Rows[0]["order_status"].ToString().Trim().ToUpper() == "Payment Received".Trim().ToUpper())
                            {
                                TempData["alertMessage"] = "Order Already Paid";
                                return View("ShowDOLiquidation", model);
                            }
                            else
                            {
                                ChkFlag = true;
                                model.DoNumber = DS.Tables[0].Rows[0]["DO_Number"].ToString();
                                model.DoAmount = DS.Tables[0].Rows[0]["Order_Amount"].ToString();
                                DataSet DSO = Methods.getDetails_Web("Get_DoLiquidationDataAsPerOI", DS.Tables[0].Rows[0]["Order_ID"].ToString(), "", "", "", "", "", "", _logger);
                                model.TotalNumberofinvoice = DSO.Tables[0].Rows.Count.ToString();
                                if (DS.Tables[1].Rows.Count > 0)
                                    model.DealerName = DS.Tables[1].Rows[0]["Dealer_Code"].ToString() + "-" + DS.Tables[1].Rows[0]["Dealer_Name"].ToString();
                                model.FinancerDetails = DS.Tables[0].Rows[0]["Financier_Name"].ToString();
                                return View("ShowDOLiquidationData", model);
                            }
                        }

                        else
                        {
                            TempData["alertMessage"] = "Order No not found in database";
                            return View("ShowDOLiquidation", model);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + "DOLiquidationController;ShowPayInfo");
                }
                return View("ShowDOLiquidation", model);
            }

        }
        [HttpPost]
        public  ActionResult UpdatePayInfo(ControlDOLiquidation model, string Type)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                    if (model.DoNumber == "" || model.DoNumber == null)
                    { TempData["alertMessage"] = "Please Enter DO Number"; return View("ShowDOLiquidationData", model); }
                    if (model.PaymentRefno == "" || model.PaymentRefno == null)
                    { TempData["alertMessage"] = "Please Enter Payment Reference No"; return View("ShowDOLiquidationData", model); }
                    if (model.PaymentType == "" || model.PaymentType == null)
                    { TempData["alertMessage"] = "Please Select Payment Type"; return View("ShowDOLiquidationData", model); }
                    if (ChkFlag == true)
                    {
                        TempData["alertMessage"] = "Invalid Order Information.";
                    }
                chkServer:
                    DataTable DT = Methods.getDetails("GetReportDetails", "", "", "", "", "", "", "", _logger);
                    if (DT.Rows.Count > 0)
                    {
                        TempData["alertMessage"] = "Waiting for Server to free.Please wait for some time.";
                        goto chkServer;
                    }
                    TempData["alertMessage"] = "";
                    var Data = model.PaymentTypeList.Where(x => x.Values.ToString() == model.PaymentType.ToString()).ToList();
                    DataTable DTFile = Methods.InsertDetails("Insert_FileDesc", "MANUAL PAYMENT-" + System.DateTime.Now.ToString("ddmmyyyyHHMM") + "-" + UserSession.LoginID.ToString(), "MANUAL PAYMENT-" + Data.ElementAt(0).Type, "0", "", "", "", "", _logger);


                    string[] Details = new string[20];
                    Details[0] = "1"; //Transaction_Id
                    Details[1] = "MAN"; //Type_of_Entry
                    Details[2] = "C"; //Dr_CR
                    Details[3] = model.DoAmount; //Entry_Amount
                    Details[4] = ""; //Value_date
                    Details[5] = Data.ElementAt(0).Type; //Product
                    Details[6] = ""; //Party_Code
                    Details[7] = ""; //Party_Name
                    Details[8] = model.DoNumber; //VA_account
                    Details[9] = ""; //Locations
                    Details[10] = ""; //RemittingBank
                    Details[11] = model.PaymentRefno; //UTR_No
                    Details[12] = ""; //IFSC_code
                    Details[13] = ""; //Dealer_Name
                    Details[14] = ""; //Dealer_Account_No
                    Details[15] = ""; //Releated_Ref_No
                    Details[16] = DTFile.Rows[0][0].ToString(); //fileID
                    Details[17] = UserSession.LoginID; //Login ID


                    string[] detailsCash = new string[25];
                    detailsCash[0] = "0";   //cashopsID
                    detailsCash[1] = Details[16];   //FileID
                    detailsCash[2] = Details[5]; //CashOps_FileType
                    detailsCash[3] = ""; //NEFT_RTGS_BT_ID
                    detailsCash[4] = Details[8].Substring(0, 22); //Virtual_Account
                    detailsCash[5] = Details[11]; //UTR_No
                    detailsCash[6] = Details[3]; //Transaction_Amount
                    detailsCash[7] = "C"; //Transaction_status
                    detailsCash[8] = DBNull.Value.ToString(); //Payment_Status
                    detailsCash[9] = DBNull.Value.ToString(); //Attempt
                    detailsCash[10] = System.DateTime.Now.ToString("DD/MMM/YYYY"); //Cash_Ops_Date
                    detailsCash[11] = System.DateTime.Now.ToString("HH:MM:SS AMPM"); //Cash_Ops_Time
                    detailsCash[12] = "0"; //GEFO_Flag
                    detailsCash[13] = DBNull.Value.ToString(); //GEFO_Date
                    detailsCash[14] = DBNull.Value.ToString(); //DR_Account_No
                    detailsCash[15] = DBNull.Value.ToString(); //CR_Account_No
                    detailsCash[16] = UserSession.LoginID; //LoginID
                    detailsCash[17] = DBNull.Value.ToString(); //EOD_MailFlag
                    detailsCash[18] = DBNull.Value.ToString(); //DRC_Generation
                    detailsCash[19] = DBNull.Value.ToString(); //FNCR_Virtual_Account
                    detailsCash[20] = DBNull.Value.ToString(); //IFSC_code
                    detailsCash[21] = DBNull.Value.ToString(); //FNCR_code
                    detailsCash[22] = DBNull.Value.ToString(); //FNCR_Name

                    Methods.CashOps_Payments_Manual(Details, detailsCash, _logger);

                    TempData["alertMessage"] = "Update CashOps Details Successfully";
                    return View("ShowDOLiquidation", model);
                }
                catch (Exception ex) { _logger.LogError(ex.ToString() + "DOLiquidationController;UpdatePayInfo"); }
                return View("ShowDOLiquidation", model);
            }
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

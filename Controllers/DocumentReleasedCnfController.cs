using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Text;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.FileProviders;
using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace HDFCMSILWebMVC.Controllers
{
    public class DocumentReleasedCnf : Controller
    {

        string Read_Line = "";
        string Header_Line_Temp = "";
        StringBuilder Header_Line = new StringBuilder();
        string Body_Line_Temp = "";
        StringBuilder   Body_Line = new StringBuilder();
        string Footer_Line_Temp = "";
        StringBuilder Footer_Line = new StringBuilder();

        string Dealer_Code;
        string Transporter_Name;
        string Payment_Amt;

        string[] Spl_Transporter_Name1, Split_String, ArrBodyLine, EmailArr, ArrFooterLine;
        Boolean Flag_Transporter_Name1;
        int Cnt_Transporter_Name1, LInt, CnEmail = 0;
        string Transporter_Name1 = "", STp = "", STp_Full = "";

        string vehicleNo = "", FinancerName = "", OrderNo = "", Invoice_date = "", InvoiceNo = "", AmtRecvd = "", AmtRecvddate = "", Goodlorry = "";
        string  AllEmail = "", ToEmail = "", BCCEmails = "";
        Double Inv_Amt;

        public static IList<DocumentReleasedCnf> inv = new List<DocumentReleasedCnf>();
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public DocumentReleasedCnf(ILogger<DocumentReleasedCnf> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
        }

        public IActionResult ViewGenerateDRC()
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                return View();
            }
            
        }

        public IActionResult GenerateDRC(DRC DS)
        {
            if (HttpContext.Session.GetString("LoginID") == null)
            { return RedirectToAction("LoginPage", "Login"); }
            else
            {
                try
                {
                    using (var db = new Entities.DatabaseContext())
                    {
                        // List<DocumentReleasedCnf> DocumentReleasedCnfs = new List<DocumentReleasedCnf>();
                        if (DS == null)
                        {
                            return NotFound();
                        }

                        if (DS.DRCUTRNo == "" || DS.DRCUTRNo == null)
                        {
                            TempData["alertMessage"] = "Please Enter UTR Number";
                            // return View("ViewGenerateDRC", DS);
                            return View("ViewGenerateDRC");
                        }
                        else if (DS.DRCVirtualAcc == "" || DS.DRCVirtualAcc == null)
                        {
                            TempData["alertMessage"] = "Please Enter Virtual Account Number";
                            //return View("ViewGenerateDRC", DS);
                            return View("ViewGenerateDRC");
                        }

                        var inv = db.Set<DocumentReleasedCnfNew>().FromSqlRaw("EXEC SP_DRCGenerate  @Search1 ='" + DS.DRCUTRNo + "',@Search2='" + DS.DRCVirtualAcc + "',@Flag=1").ToList();

                        if (inv.Count > 0)
                        {
                            WriteConfirmationOrder(inv[0].UTR_No, inv[0].Virtual_Account);
                            var destinationPath = "";
                            if (inv[0].UTR_No != "" || inv[0].Virtual_Account != "")
                            {


                                var currentdatefolder = DateTime.Now.ToString("yyyyMMddHHmmss");
                                // var files = HttpContext.Request.Form.Files;
                                var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Document_Release");
                                if (files.Any())
                                {
                                    foreach (var file in files)
                                    {
                                        if (file.Length > 0)
                                        {
                                            //Getting FileName
                                            var fileName = Path.GetFileName(file);
                                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ZipsUpload", currentdatefolder);
                                            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ZipsUpload",
                                                currentdatefolder)))
                                            {
                                                System.IO.Directory.CreateDirectory(path);
                                            }

                                            var storingfilePath = $"{new PhysicalFileProvider(path).Root}{$@"{fileName}"}";
                                            using FileStream fs = System.IO.File.Create(storingfilePath);

                                            //file.CopyToAsync(fs);
                                            fs.Flush();

                                        }
                                    }


                                    var zippath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ZipsUpload", currentdatefolder);
                                    var sourcezipPath = new PhysicalFileProvider(zippath).Root;
                                    var zipname = $"File_{currentdatefolder}.zip";
                                    destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CreatedZip", zipname);
                                    ZipFile.CreateFromDirectory(sourcezipPath, destinationPath);


                                }

                                var configMail = _configuration.GetValue<string>("EmailSettings:Confirmation_Mail");
                                var frmEmailID = _configuration.GetValue<string>("EmailSettings:Email_FromID");
                                if (DS.DRCUTRCm == "DRC Generate")
                                {
                                    //SendEmail(frmEmailID, "", "", destinationPath, "Document Release Confirmation(" + inv[0].Virtual_Account + ")", Body_Line.ToString());
                                    SendEmail(frmEmailID, "", "", destinationPath, "Document Release Confirmation(" + inv[0].Virtual_Account + ")", Body_Line.ToString());
                                }
                                else if (DS.DRCUTRCm == "DRC Generate and Mail")
                                {
                                    if (DS.IsSelect == true && configMail.Length > 0)
                                    {
                                        SendEmail(frmEmailID + ";" + configMail, "", "", destinationPath, "Document Release Confirmation(" + inv[0].Virtual_Account + ")", "");
                                    }
                                    else
                                    {
                                        SendEmail(frmEmailID, "", "", destinationPath, "Document Release Confirmation(" + inv[0].Virtual_Account + ")", "");
                                    }
                                }




                                var file1 = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\Document_Release").GetFiles("*.*");
                                if (!Directory.Exists(_configuration.GetValue<string>("MSILSettings:BackupFilePath"))) 
                                {
                                    System.IO.Directory.CreateDirectory(_configuration.GetValue<string>("MSILSettings:BackupFilePath"));
                                }


                                if (file1.Any())
                                {
                                    foreach (FileInfo file in file1)
                                    {
                                        file.MoveTo($@"{_configuration.GetValue<string>("MSILSettings:BackupFilePath") + "\\" + file.Name}");

                                    }
                                }



                                TempData["alertMessage"] = "File Generated Successfully";
                            }
                            else
                            {
                                TempData["alertMessage"] = "Invalid Utr No./Virtual Ac No.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString() + " - DocumentReleasedCnf;GenerateDRC");
                }


                return View("ViewGenerateDRC");
            }
        }


        private void WriteConfirmationOrder(string UtrNo, string VANo)
        {
            //string Read_Line = "", Header_Line_Temp = "", Header_Line = "";
            //string Body_Line_Temp = "", Body_Line = "";
            //string Footer_Line_Temp = "", Footer_Line = "";



            try
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Document_Release") == false)
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Document_Release");
                }
                
                if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Document Release Confirmation.txt"))
                {
                    TempData["alertMessage"] = "Document Release Confirmation Template Not Found";
                    _logger.LogError("Document Release Confirmation Template Not Found" + " - DocumentReleasedCnf;Show");
                    return;
                }
                else
                {
                    StreamReader StRead = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Document Release Confirmation.txt");

                    while ((Read_Line = StRead.ReadLine()) != null)
                    {

                        if (Read_Line.ToString().ToUpper() == "<Header>".ToUpper())
                        {
                            for (int cnt = 1; cnt <= 1000; cnt++)
                            {
                                Header_Line_Temp = StRead.ReadLine();
                                if (Header_Line_Temp.ToString().ToUpper() == "</Header>".ToUpper())
                                {
                                    break;
                                }
                                else
                                {
                                    Header_Line = Header_Line.AppendLine(Header_Line_Temp);
                                }
                            }
                        }
                        else if (Read_Line.ToString().ToUpper() == "<Body>".ToUpper())
                        {
                            for (int cnt = 1; cnt <= 1000; cnt++)
                            {
                                Body_Line_Temp = StRead.ReadLine();
                                if (Body_Line_Temp.ToString().ToUpper() == "</Body>".ToUpper())
                                {
                                    break;
                                }
                                else
                                {
                                   if( Body_Line_Temp == "") 
                                    { 
                                        Body_Line = Body_Line.AppendLine("");
                                    }
                                    else
                                    {
                                        Body_Line = Body_Line.AppendLine(Body_Line_Temp);
                                    }
                                }
                            }
                        }
                        else if (Read_Line.ToString().ToUpper() == "<Footer>".ToUpper())
                        {
                            for (int cnt = 1; cnt <= 1000; cnt++)
                            {
                                Footer_Line_Temp = StRead.ReadLine();
                                if (Footer_Line_Temp.ToString().ToUpper() == "</Footer>".ToUpper())
                                {
                                    break;
                                }
                                else
                                {
                                    if (Footer_Line_Temp == "")
                                    {
                                        Footer_Line = Footer_Line.AppendLine("");
                                    }
                                    else
                                    {
                                        Footer_Line = Footer_Line.AppendLine(Footer_Line_Temp);
                                    }
                                    
                                }
                            }
                        }


                    }

                    StRead.Close();


                    DataTable dtInv = service.getDetails("Get_InvoiceSum", UtrNo, VANo, "", "", "", "", "");
                    if (dtInv.Rows.Count > 0)
                    {
                        Dealer_Code = dtInv.Rows[0]["Dealer_Name"].ToString() + "-" + dtInv.Rows[0]["Dealer_Code"].ToString();
                        Transporter_Name = dtInv.Rows[0]["Transporter_Name"].ToString();
                        Payment_Amt = dtInv.Rows[0]["PaymentAmt"].ToString();
                    }
                    DataTable dtDT = service.getDetails("Get_DealerTransporter", UtrNo, VANo, "", "", "", "", "");
                    if (dtDT.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtDT.Rows.Count; i++)
                        {
                            Flag_Transporter_Name1 = false;
                            Spl_Transporter_Name1 = Transporter_Name1.Split(",");
                            for (Cnt_Transporter_Name1 = 0; Cnt_Transporter_Name1 < Spl_Transporter_Name1.Length; Cnt_Transporter_Name1++)
                            {
                                if (dtDT.Rows[i]["Transporter_Name"].ToString().ToUpper() == Spl_Transporter_Name1[Cnt_Transporter_Name1].ToUpper())
                                    Flag_Transporter_Name1 = true;
                            }
                            if (Flag_Transporter_Name1 == false)
                            {
                                if (Transporter_Name1 == "")
                                    Transporter_Name1 = dtDT.Rows[i]["Transporter_Name"].ToString();
                                else

                                    Transporter_Name1 = Transporter_Name1 + "," + dtDT.Rows[i]["Transporter_Name"].ToString();
                            }
                        }

                    }

                    Split_String = Transporter_Name1.Split();
                    STp = "";
                    STp_Full = "";
                    for (LInt = 0; LInt < Transporter_Name1.Length; LInt++)
                    {
                        if (STp.Length == 100)
                        {
                            if (STp_Full == "")
                                STp_Full = STp;
                            else
                                STp_Full = STp_Full + System.Environment.NewLine + STp;
                            STp = "";
                        }
                        STp = STp + Transporter_Name1.Substring(LInt, 1);

                    }
                    if (STp_Full == "")
                        STp_Full = STp;
                    else
                        STp_Full = STp_Full + System.Environment.NewLine + STp;
                    Transporter_Name1 = STp_Full;


                    string FileName_Pdf = "Document_Release_Confirmation_" + System.DateTime.Now.ToString("ddmmyyyyHHMMSS") + ".pdf";
                    generatepdfTesting(FileName_Pdf, UtrNo,VANo);

                }

                

               


                ///////Zip file create
               // string Zippath = "";

                //if (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                //{
                //    string startPath = @".\start";
                //    Zippath = @".\result.zip";

                //    ZipFile.CreateFromDirectory(startPath, Zippath);

                //}

                //if (comboBox1.SelectedIndex == 2)
                //{
                //    if (checkBox1.Checked == true)
                //    {
                //        if (sttg.Confirmation_Mail.ToString().Trim().Length > 0)
                //        {
                //            SendEmail(sttg.Email_FromID + ";" + sttg.Confirmation_Mail, "", "BCC", Zippath, "Document Release Confirmation(" + VANo + ")", Body_Line);
                //        }
                //        else
                //        {
                //            SendEmail(sttg.Email_FromID, "", "BCC", Zippath, "Document Release Confirmation(" + VANo + ")", Body_Line);
                //        }
                //    }
                //    else
                //    {
                //        SendEmail(sttg.Email_FromID, "", "", Zippath, "Document Release Confirmation(" + VANo + ")", Body_Line);
                //    }
                //}

                //if (comboBox1.SelectedIndex != 1)
                //{
                //    if (File.Exists(Zippath))
                //    {
                //        File.Delete(Zippath);
                //    }
                //}


            }

            catch (Exception ex)
            {
               _logger.LogError(ex.ToString() + "WriteConfirmationOrder");
            }

        }

        public void SendEmail(string SendTo, string SendCC, string SendBCC, string FileAttach, string subject, string Message)
        {
            try
            {
                // **************************************************************************
                //Advanced Properties, change only if you have a good reason to do so.
                //**************************************************************************
                // .ConnectTimeout = 10                      ' Optional, default = 10
                // .ConnectRetry = 5                         ' Optional, default = 5
                //.MessageTimeout = 60                      ' Optional, default = 60
                //.PersistentSettings = True                ' Optional, default = TRUE
                //Optional, default = 25
                MailMessage mail = new MailMessage();
                //Settings sttg = new Settings();
                //ReadWriteAppSettings readWriteAppSettings = new ReadWriteAppSettings();
                //sttg = readWriteAppSettings.ReadGetSectionAppSettings();
                SmtpClient SmtpServer = new SmtpClient(_configuration.GetValue<string>("EmailSetting:SMTP_HOST"), Convert.ToInt32(_configuration.GetValue<string>("EmailSetting:Port")));
                mail.From = new MailAddress(_configuration.GetValue<string>("EmailSetting:Email_FromID"));

                Attachment attachment;

                string[] ArrSendTo;
                ArrSendTo = SendTo.ToString().Trim().Split(";");
                for (int i = 0; i <= ArrSendTo.Length - 1; i++)
                {
                    mail.To.Add(new MailAddress(ArrSendTo[i]));
                }
                if (SendCC != "")
                {
                    ArrSendTo = SendCC.ToString().Trim().Split(";");
                    for (int i = 0; i <= ArrSendTo.Length - 1; i++)
                    {
                        mail.CC.Add(new MailAddress(ArrSendTo[i]));
                    }
                    //mail.CC.Add(SendCC);
                }
                ArrSendTo = SendBCC.ToString().Trim().Split(";");
                if (SendBCC != "")
                {
                    ArrSendTo = SendBCC.ToString().Trim().Split(";");
                    for (int i = 0; i <= ArrSendTo.Length - 1; i++)
                    {
                        mail.Bcc.Add(new MailAddress(ArrSendTo[i]));
                    }
                    //mail.CC.Add(SendCC);
                }
                mail.Subject = subject;
                // string strMailBody;
                mail.IsBodyHtml = true;
                StringBuilder sb = new StringBuilder();
                sb.Append("Respected,");
                sb.Append("<br/>");
                sb.Append("<br/>");
                string fmsg = Message.Replace("\r\n", Environment.NewLine);
                sb.Append(fmsg);
                sb.Append("<br/>");
                sb.Append("<br/>");
                if (FileAttach != "")
                {
                    attachment = new Attachment(FileAttach);
                    mail.Attachments.Add(attachment);
                    sb.Append(" Please find attachment of " + subject);
                }
                sb.Append("<br/>");
                sb.Append("<br/>");

                sb.Append("********This is system Generated Mail. Do not reply.********");
                sb.Append("<br/>");
                sb.Append("<br/>");
                sb.Append("Thanks And Regards,");
                sb.Append("<br/>");
                sb.Append("     HDFC Bank     ");


                mail.Body = sb.ToString();

                SmtpServer.Credentials = new System.Net.NetworkCredential(_configuration.GetValue<string>("EmailSetting:Email_FromID"), _configuration.GetValue<string>("EmailSetting:Password"));
                SmtpServer.Send(mail);
                mail.Dispose();
                SmtpServer.Dispose();
                //System.GC.Collect();
                //System.GC.WaitForPendingFinalizers();
                //clserr.LogEntry("Exiting WriteConfirmationOrder Function", false);
                //clserr.LogEntry("Exiting WriteConfirmationOrder Function", false);
            }
            catch (Exception ex)
            {
                //clserr.WriteErrorToTxtFile(ex.Message + "", "SendEmail", FileAttach);
                //clserr.Handle_Error(ex, "SendEmail", "SendEmail");
            }



        }

        public void SendEmailOld(string ToEmail, string SendCC, string SendBCC, string AttachmentFile, string subject, string Matter)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //SmtpClient smtp = new SmtpClient();

                SmtpClient smtp = new SmtpClient(_configuration.GetValue<string>("EmailSetting:smtp.rediffmailpro.com"), Convert.ToInt32(_configuration.GetValue<string>("EmailSetting:Port")));
                //mail.From = new MailAddress(sttg.Email_FromID);
                //smtp.Port = Convert.ToInt32(sttg.Port);
                //smtp.Host = sttg.SMTP_HOST;

                mail.From = new MailAddress(_configuration.GetValue<string>("EmailSetting:Email_FromID"));
                //smtp.Port = Convert.ToInt32(_configuration.GetValue<string>("EmailSetting:Port"));
                //smtp.Host = _configuration.GetValue<string>("EmailSetting:smtp.rediffmailpro.com");

                

                string[] AryEmail = ToEmail.ToString().Split(';');
                for (int E = 0; E < AryEmail.Length; E++)
                {
                    mail.To.Add(AryEmail[E]);
                }

                mail.CC.Add(SendCC);
                mail.Bcc.Add(SendBCC);

                mail.Subject = subject;

                string strMailBody = "";
                mail.IsBodyHtml = true;
                if (AttachmentFile.ToString().Trim().Length > 0)
                {
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(AttachmentFile);
                    mail.Attachments.Add(attachment);
                }

                strMailBody = "<HTML><body><br><br>";
                strMailBody = strMailBody + Matter;
                mail.Body = strMailBody;

                //AlternateView htmlView = new AlternateView();

                //htmlView = AlternateView.CreateAlternateViewFromString(strMailBody,null,"text/html");
                //mail.AlternateViews(htmlView);

                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = new System.Net.NetworkCredential(sttg.UserID.ToString(), sttg.Password);
                smtp.Credentials = new System.Net.NetworkCredential(_configuration.GetValue<string>("EmailSetting:UserID"), _configuration.GetValue<string>("EmailSetting:Password"));
                smtp.Send(mail);

                //clserr.LogEntry(ToEmail + " Email Sent Successfully", false);

                //DateTime dtStartDate1 = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss").ToString().Replace("-", "/").Replace(".", "/"), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture); //Current Date
                //clserr.LogEntry("No invoice record found " + dtStartDate1, false);

            }
            catch (Exception ex)
            {
                //clserr.WriteErrorToTxtFile(ex.Message, "FrmDocumentRelConfir", "SendEmail");
                
                _logger.LogError(ex.ToString() + " - DocumentReleasedCnf;SendEmail");
            }

        }

        public void generatepdfTesting(string appPhysicalPath, string UtrNo, string VANo)
        {
            var filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Document_Release" + "\\" + appPhysicalPath;

            var doc1 = new Document();
            var streamOBJ = new System.IO.FileStream(filepath, System.IO.FileMode.CreateNew);
            PdfWriter.GetInstance(doc1, streamOBJ);
            doc1.Open();

            //doc1.Add(new Paragraph("Document Release Confirmation"));

            var docTitle = new Paragraph(Header_Line.ToString());
            var titleFont = FontFactory.GetFont("FONT_ARIAL", 12, Font.NORMAL, Color.BLACK);
            docTitle.Alignment = Element.ALIGN_CENTER;
            doc1.Add(docTitle);

            //Logo
            string strImageLogoPath = AppDomain.CurrentDomain.BaseDirectory +  "\\" + "hdfc logo.png";
            Image img = Image.GetInstance(strImageLogoPath);
            img.ScaleToFit(110.0F, 125.0F);
            img.Alignment = Image.ALIGN_RIGHT;
            doc1.Add(img);

            var blnlie = new Paragraph("                                 ");          
            doc1.Add(blnlie);

            var blnlie1 = new Paragraph("                                 ");          
            doc1.Add(blnlie1);


            string fBody_Line = Body_Line.ToString();
            fBody_Line = fBody_Line.Replace("<Transporter Name from Inv table>", Transporter_Name1);
            fBody_Line = fBody_Line.Replace("<Dealer Name from Inv table>", Dealer_Code);           
            var body = new Paragraph(fBody_Line.ToString());
            doc1.Add(body);


            PdfPTable DtTitle = new PdfPTable(8);
            int[] TitleWidth = new int[8] { 70, 30, 25, 30, 35, 30, 60, 45 };
            //DtTitle.Border = Rectangle.NO_BORDER;
            //DtTitle.DefaultCellBorder = 1,
            DtTitle.HorizontalAlignment  = Element.ALIGN_CENTER;
            DtTitle.SetWidths(TitleWidth);
            DtTitle.TotalWidth = 300;
            //DtTitle.Cellpadding = 2;


            PdfPCell Cell_Title1 = new PdfPCell(new Phrase("Order No.".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD,Color.BLACK)));
            Cell_Title1.BackgroundColor = Color.CYAN;
            //Cell Cell_Title1 = new Cell();
            //Cell_Title1.HorizontalAlignment = Element.ALIGN_LEFT
            //Cell_Title1.Add(new Paragraph("Order No.".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));

            PdfPCell Cell_Title2 = new PdfPCell(new Paragraph("Invoice No".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD,Color.BLACK)));
            Cell_Title2.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title3 = new PdfPCell(new Paragraph("Invoice Date".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title3.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title4 = new PdfPCell(new Paragraph("Amount Received (INR)".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title4.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title5 = new PdfPCell(new Paragraph("Amount Received Date".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title5.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title6 = new PdfPCell(new Paragraph("Goods Receipt/Lorry Receipt No.".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title6.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title7 = new PdfPCell(new Paragraph("Vehicle Identification Number".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title7.BackgroundColor = Color.CYAN;
            PdfPCell Cell_Title8 = new PdfPCell(new Paragraph("Financer Name".ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, Color.BLACK)));
            Cell_Title8.BackgroundColor = Color.CYAN;

            DtTitle.AddCell(Cell_Title1);
            DtTitle.AddCell(Cell_Title2);
            DtTitle.AddCell(Cell_Title3);
            DtTitle.AddCell(Cell_Title4);
            DtTitle.AddCell(Cell_Title5);
            DtTitle.AddCell(Cell_Title6);
            DtTitle.AddCell(Cell_Title7);
            DtTitle.AddCell(Cell_Title8);


            DataTable dtDetails = service.getDetails("Get_DealerDetails", UtrNo, VANo, "", "", "", "", "");
            if (dtDetails.Rows.Count > 0)
            {
                for (int k = 0; k < dtDetails.Rows.Count; k++)
                {
                    //Table records
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    OrderNo = dtDetails.Rows[k]["DO_number"].ToString();
                    InvoiceNo = "" + dtDetails.Rows[k]["Invoice_Number"].ToString();
                    Invoice_date = dtDetails.Rows[k]["Transport_Date"].ToString(); //"" + DateTime.ParseExact(dtDetails.Rows[k]["Transport_Date"].ToString(), "000000", provider);
                    AmtRecvd = "" + dtDetails.Rows[k]["Order_Inv_Amount"].ToString();
                    string date = Convert.ToDateTime(dtDetails.Rows[k]["payment_received_date"]).ToString("dd/MM/yyyy").Replace('-', '/');
                    AmtRecvddate = Convert.ToDateTime(dtDetails.Rows[k]["payment_received_date"]).ToString("dd/MM/yyyy").Replace('-', '/');
                    Goodlorry = "" + dtDetails.Rows[k]["Transporter_Code"].ToString();
                    vehicleNo = "" + dtDetails.Rows[k]["Vehical_ID"].ToString();
                    FinancerName = "" + dtDetails.Rows[k]["FNCR_name"].ToString();
                    AllEmail = dtDetails.Rows[k]["Email_IDs"].ToString().Replace(((Char)34).ToString(), "");  //or use "\""
                    AllEmail = AllEmail.Replace(",", ";");
                    EmailArr = AllEmail.Split(";");
                    BCCEmails = "";
                    ToEmail = "";

                    for (CnEmail = 0; CnEmail < EmailArr.Length; CnEmail++)
                    {
                        if (EmailArr[CnEmail].ToString().ToUpper().Contains("@maruti.co.in".ToUpper())) //Means "@maruti.co.in" found then BCC.MSIL Ids in BCC
                            BCCEmails = BCCEmails + ";" + EmailArr[CnEmail];
                        else
                            ToEmail = ToEmail + ";" + EmailArr[CnEmail];
                    }

                    PdfPCell Cell_OrderNo = new PdfPCell(new Phrase(OrderNo, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_InvoiceNo = new PdfPCell(new Paragraph(InvoiceNo, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_InvoiceDate = new PdfPCell(new Paragraph(Invoice_date, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_AmtRcv = new PdfPCell(new Paragraph(AmtRecvd, FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_AmtRcvDT = new PdfPCell(new Paragraph(AmtRecvddate.ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_GoodsRcpt = new PdfPCell(new Paragraph(Goodlorry.ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_VIN = new PdfPCell(new Paragraph(vehicleNo.ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));
                    PdfPCell Cell_FinancerName = new PdfPCell(new Paragraph(FinancerName.ToString().Trim(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD)));

                    DtTitle.AddCell(Cell_OrderNo);
                    DtTitle.AddCell(Cell_InvoiceNo);
                    DtTitle.AddCell(Cell_InvoiceDate);
                    DtTitle.AddCell(Cell_AmtRcv);
                    DtTitle.AddCell(Cell_AmtRcvDT);
                    DtTitle.AddCell(Cell_GoodsRcpt);
                    DtTitle.AddCell(Cell_VIN);
                    DtTitle.AddCell(Cell_FinancerName);

                    Inv_Amt = Inv_Amt + Convert.ToDouble("" + dtDetails.Rows[k]["Order_Inv_Amount"].ToString());
                }
            }

            doc1.Add(DtTitle);

            doc1.Add(blnlie);
            doc1.Add(blnlie);
            //doc1.Add(blnlie);
            //doc1.Add(blnlie);

            string fFooter_Line = Footer_Line.ToString();
            fFooter_Line = fFooter_Line.ToString().Replace("<dealer name invoice table>", Dealer_Code);
            fFooter_Line = fFooter_Line.ToString().Replace("<Payment amount received>", Inv_Amt.ToString("0.00"));
            var footer = new Paragraph(fFooter_Line.ToString());
            doc1.Add(footer);

            doc1.Close();

        }

        public void ReadFormatMail(out StringBuilder Header, out StringBuilder Body, out StringBuilder Footer)
        {
            Header = new StringBuilder();
            Body = new StringBuilder();
            Footer = new StringBuilder();
            try
            {
                StreamReader objStrmReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\Document Release Confirmation.txt", Encoding.UTF8);
                string Read_Line = "";

                string Header_Line_Temp = "";
                while (!objStrmReader.EndOfStream)
                {
                ReadL: Read_Line = objStrmReader.ReadLine();
                    if ("<Header>".ToUpper() == Read_Line.ToUpper())
                    {
                    ReadNextHeader: Header_Line_Temp = objStrmReader.ReadLine();
                        if (Header_Line_Temp != null)
                        {
                            if ("</Header>".ToUpper() == Header_Line_Temp.ToUpper())
                                goto ReadL;
                            else if ("</Header>".ToUpper() != Header_Line_Temp.ToUpper())
                            {
                                Header.Append("<br/>");
                                Header.Append(Header_Line_Temp);
                                goto ReadNextHeader;
                                //Header_Line = Header_Line + Header_Line.Append("<br/>")+ Header_Line_Temp;
                            }
                        }
                    }
                    if ("<Body>".ToUpper() == Read_Line.ToUpper())
                    {
                    ReadNextBody: Header_Line_Temp = objStrmReader.ReadLine();
                        if (Header_Line_Temp != null)
                        {
                            if ("</Body>".ToUpper() == Header_Line_Temp.ToUpper())
                                goto ReadL;
                            else if ("</Body>".ToUpper() != Header_Line_Temp.ToUpper())
                            {
                                if (Header_Line_Temp.Contains("Goods Receipt Note �"))
                                    Header_Line_Temp = Header_Line_Temp.Replace("�", "-");
                                if (Header_Line_Temp.Contains("�We hereby certify "))
                                    Header_Line_Temp = Header_Line_Temp.Replace("�We hereby certify ", "\"We hereby certify ");
                                if (Header_Line_Temp.Contains("this invoice�"))
                                    Header_Line_Temp = Header_Line_Temp.Replace("this invoice�", "this invoice\"");
                                Body.Append("<br/>");
                                Body.Append(Header_Line_Temp);


                                goto ReadNextBody;
                                //Header_Line = Header_Line + Header_Line.Append("<br/>")+ Header_Line_Temp;
                            }
                        }
                    }
                    if ("<Footer>".ToUpper() == Read_Line.ToUpper())
                    {
                    ReadNextFooter: Header_Line_Temp = objStrmReader.ReadLine();
                        if (Header_Line_Temp != null)
                        {
                            if ("</Footer>".ToUpper() == Header_Line_Temp.ToUpper())
                                goto ReadL;
                            else if ("</Footer>".ToUpper() != Header_Line_Temp.ToUpper())
                            {
                                Footer.Append("<br/>");
                                Footer.Append(Header_Line_Temp);
                                goto ReadNextFooter;
                                //Header_Line = Header_Line + Header_Line.Append("<br/>")+ Header_Line_Temp;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }

        }
        public void WriteConfirmationOrderTEST(string UTRNO,string VIRTUALACCNO)
        {
            int cnt;
           // string Read_Line;
            string Header_Line_Temp;
            string Header_Line = "";
            string Body_Line_Temp;
            string Body_Line ="";
            string Footer_Line_Temp;
            string Footer_Line;
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            if (!Directory.Exists(currentDirectory + "\\Document_Release"))
            {
                Directory.CreateDirectory(currentDirectory + "\\Document_Release");
            }
            string path = currentDirectory + "\\" + "Document Release Confirmation.txt";

            if (!System.IO.File.Exists(path))
            {
                TempData["alertMessage"] = "Document Release Confirmation Template Not Found";
                _logger.LogError("Document Release Confirmation Template Not Found" + " - DocumentReleasedCnf;Show");
                //return NotFound();
            }
            
            using (StreamReader reader = new StreamReader(currentDirectory + "\\" + "Document Release Confirmation.txt"))
            {
                string line;
                // Read line by line
                while ((line = reader.ReadLine()) != null)
                {
                    if (("<Header>").ToUpper() == line.ToUpper())
                    {
                        for (cnt = 1;cnt <=1000;cnt++)
                        {
                            Header_Line_Temp = reader.ReadLine();
                            if (("</Header>").ToUpper() == Header_Line_Temp.ToUpper())
                            {
                                goto NextLine;
                            }
                            else
                            {
                                Header_Line = Header_Line + Header_Line_Temp;
                            }
                        }
                    }
                    else if(("<Body>").ToUpper() == line.ToUpper())
                    {
                        for (cnt = 1; cnt <= 1000; cnt++)
                        {
                            Body_Line_Temp = reader.ReadLine();
                            if (("</Body>").ToUpper() == Body_Line_Temp.ToUpper())
                            {
                                goto NextLine;
                            }
                            else
                            {
                                Body_Line = Body_Line + Body_Line_Temp;
                            }
                        }
                    }
                    else if (("<Footer>").ToUpper() == line.ToUpper())
                    {
                        for (cnt = 1; cnt <= 1000; cnt++)
                        {
                            Footer_Line_Temp = reader.ReadLine();
                            if (("</Footer>").ToUpper() == Footer_Line_Temp.ToUpper())
                            {
                                goto NextLine;
                            }
                            else
                            {
                                Footer_Line = Body_Line + Footer_Line_Temp;
                            }
                        }
                    }
                    Console.WriteLine(line);
                NextLine:;
                }
            }



            string FileName_Pdf;
            FileName_Pdf = "Document_Release_Confirmation_" + DateTime.Now.ToString("ddmmyyyyHHMMSS") + ".pdf";

            

            //return NotFound();
        }
    }

}

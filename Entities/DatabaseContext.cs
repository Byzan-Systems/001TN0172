
using HDFCMSILWebMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HDFCMSILWebMVC.Entities
{
    public class DatabaseContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration["connectionStrings:DefaultConnection"]);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisterNew>().HasNoKey();
            modelBuilder.Entity<ShowUserMaster>().HasNoKey();
            modelBuilder.Entity<UserMaster>().HasNoKey();
            modelBuilder.Entity<ShowFinancerMaster>().HasNoKey();
            modelBuilder.Entity<FinancerMaster>().HasNoKey();
            modelBuilder.Entity<FinancerModel>().HasNoKey();
            modelBuilder.Entity<StockyardModel>().HasNoKey();
            modelBuilder.Entity<ShowStockyardMaster>().HasNoKey();
            modelBuilder.Entity<StockyardMaster>().HasNoKey();
            
            modelBuilder.Entity<DetailsUserMaster>().HasNoKey();
            modelBuilder.Entity<showAddMISControlPoint>().HasNoKey();
            modelBuilder.Entity<DownloadFillInvoice>().HasNoKey();

            modelBuilder.Entity<AddMISControlPoint_INVDate>().HasNoKey();
            modelBuilder.Entity<AddMISControlPoint_Order_Rec>().HasNoKey();
            modelBuilder.Entity<AddMISControlPoint_PHyInvRec>().HasNoKey();
            modelBuilder.Entity<AddMISControlPoint_Payment_Recev>().HasNoKey();
            modelBuilder.Entity<MISLiquidation>().HasNoKey();
            modelBuilder.Entity<ShowMISLiquidation>().HasNoKey();

            modelBuilder.Entity<ShowMISLiquidationDetailInvoice>().HasNoKey();
            modelBuilder.Entity<MISLiquidationDetailInvoice>().HasNoKey();
            modelBuilder.Entity<ShowPaymentReportSelect>().HasNoKey();
            modelBuilder.Entity<ShowPending_Report>().HasNoKey();
            modelBuilder.Entity<ControlDOLiquidation>().HasNoKey();
            modelBuilder.Entity<AccountDetails>().HasNoKey();
            modelBuilder.Entity<DBAccountDetails>().HasNoKey();
            modelBuilder.Entity<TallyBookingFCC>().HasNoKey();
            modelBuilder.Entity<TallyBookingFCCData>().HasNoKey();
            modelBuilder.Entity<ShowTradeRefNo>().HasNoKey();
            modelBuilder.Entity<TradeRefNo>().HasNoKey();
            modelBuilder.Entity<AuthorizInvDORetainInv>().HasNoKey();
            modelBuilder.Entity<AuthorizoderNoInv>().HasNoKey();
            modelBuilder.Entity<AuthorizInvoicesonly>().HasNoKey();
            modelBuilder.Entity<ShowCashOps_Upload>().HasNoKey();

            modelBuilder.Entity<PendingRet_InvoiceLevel>().HasNoKey();
            modelBuilder.Entity<PendingRet_PendingOrder>().HasNoKey();
            modelBuilder.Entity<PendingRet_PendingPayment>().HasNoKey();
            modelBuilder.Entity<PendingRet_FinancerName>().HasNoKey();
            modelBuilder.Entity<PendingRet_NONFinancerName>().HasNoKey();
            modelBuilder.Entity<PendingRet_BothFinancerName>().HasNoKey();
            modelBuilder.Entity<PendingRet_DO_CancellationAndRetainInvoiceReport>().HasNoKey();
            modelBuilder.Entity<PendingRet_DOAndInvoiceCancellationReport>().HasNoKey();
            modelBuilder.Entity<PendingRet_InvoiceCancellationReport>().HasNoKey();
            modelBuilder.Entity<Gefu>().HasNoKey();
     

            modelBuilder.Entity<Order_Des>().HasNoKey();

            modelBuilder.Entity<invoice_cancel_desc>().HasNoKey();
            modelBuilder.Entity<Order_Invoice_Order_Des>().HasNoKey();
            modelBuilder.Entity<DocumentReleasedCnfNew>().HasNoKey();
            modelBuilder.Entity<AuthOrder_Des>().HasNoKey();
            modelBuilder.Entity<cancellationInvDORetainInv>().HasNoKey();
            modelBuilder.Entity<cancellationInvoicesonly>().HasNoKey();
            modelBuilder.Entity<cancellationoderNoInv>().HasNoKey();
            modelBuilder.Entity<DocumentReleasedCnfNew>().HasNoKey();
            modelBuilder.Entity<invoice>().HasNoKey();
            modelBuilder.Entity<Order_Invoice_Order_DesForCanfrm>().HasNoKey(); ////Added on 17-12-2023
            modelBuilder.Entity<InvoiceDetails>().HasNoKey();
            modelBuilder.Entity<OrderDetails>().HasNoKey();
            modelBuilder.Entity<CashOPSDetails>().HasNoKey();
            modelBuilder.Entity<PaymentDetails>().HasNoKey();
            modelBuilder.Entity<UAM_LoginLogout>().HasNoKey();
            modelBuilder.Entity<UAM_LoginLogoutExist>().HasNoKey();
            modelBuilder.Entity<user_mst_temp>().HasNoKey();
        }
        public DbSet<LoginMST> LoginMSTs { get; set; }
        public DbSet<user_mst_temp> user_mst_tempDB { get; set; }
        public DbSet<FinancerModel> FinancerMST { get; set; }
        public DbSet<StockyardModel> StockyardMST { get; set; }
        public DbSet<RegisterNew> RegisterNews { get; set; }
        public DbSet<UserMaster> UserMasters { get; set; }
        public DbSet<ShowUserMaster> ShowUserMasters { get; set; }
        public DbSet<DownloadFillInvoice> DownloadFillInvoices { get; set; }
        public DbSet<DetailsUserMaster> DetailsUserMasters { get; set; }
        public DbSet<showAddMISControlPoint> showAddMISControlPoints { get; set; }
        public DbSet<AddMISControlPoint_INVDate> AddMISControlPoint_INVDates { get; set; }
        public DbSet<AddMISControlPoint_PHyInvRec> AddMISControlPoint_PHyInvRecs { get; set; }
        public DbSet<AddMISControlPoint_Order_Rec> AddMISControlPoint_Order_Recs { get; set; }
        public DbSet<AddMISControlPoint_Payment_Recev> AddMISControlPoint_Payment_Recevs { get; set; }
        public DbSet<AccountDetails> Account_Details { get; set; }
        public DbSet<MISLiquidation> MISLiquidations { get; set; }
        public DbSet<ShowMISLiquidation> ShowMISLiquidations { get; set; }
        public DbSet<ShowMISLiquidationDetailInvoice> ShowMISLiquidationDetailInvoices { get; set; }
        public DbSet<ShowPaymentReportSelect> ShowPaymentReports { get; set; }
        public DbSet<ShowPending_Report> ShowPending_Reports { get; set; }
        public DbSet<ControlDOLiquidation> ControlDOLiquidations { get; set; }
        public DbSet<DBAccountDetails> DBAccountDetailss { get; set; }
        public DbSet<TallyBookingFCC> TallyBookingFCCs { get; set; }
        public DbSet<TallyBookingFCCData> TallyBookingFCCDatas { get; set; }
        public DbSet<ShowTradeRefNo> ShowTradeRefNo { get; set; }
        public DbSet<TradeRefNo> TradeRefNos { get; set; }
        public DbSet<MISLiquidationDetailInvoice> MISLiquidationDetailInvoices { get; set; }
        public DbSet<AuthorizInvDORetainInv> AuthorizInvDORetainInvs { get; set; }
        public DbSet<AuthorizoderNoInv> AuthorizoderNoInvs { get; set; }
        public DbSet<AuthorizInvoicesonly> AuthorizInvoicesonlys { get; set; }
        public DbSet<ShowCashOps_Upload> ShowCashOps_Uploads { get; set; }
        public DbSet<PendingRet_InvoiceLevel> pendingRet_InvoiceLevels { get; set; }
        public DbSet<PendingRet_PendingOrder> PendingRet_PendingOrders { get; set; }
        public DbSet<PendingRet_PendingPayment> PendingRet_PendingPayments { get; set; }
        public DbSet<PendingRet_FinancerName> PendingRet_FinancerNames { get; set; }
        public DbSet<PendingRet_NONFinancerName> PendingRet_NONFinancerNames { get; set; }
        public DbSet<PendingRet_BothFinancerName> PendingRet_BothFinancerNames { get; set; }
        public DbSet<PendingRet_DO_CancellationAndRetainInvoiceReport> PendingRet_DO_CancellationAndRetainInvoiceReports { get; set; }
        public DbSet<PendingRet_DOAndInvoiceCancellationReport> PendingRet_DOAndInvoiceCancellationReports { get; set; }
        public DbSet<PendingRet_InvoiceCancellationReport> PendingRet_InvoiceCancellationReports { get; set; }
        public DbSet<Gefu> Gefus { get; set; }

        public DbSet<Order_Des> Order_Dess { get; set; }

        public DbSet<invoice_cancel_desc> invoice_cancel_descs { get; set; }

        public DbSet<Order_Invoice_Order_Des> Order_Invoice_Order_Dess { get; set; }

        public DbSet<DocumentReleasedCnfNew> DocumentReleasedCnfNews { get; set; }
        public DbSet<AuthOrder_Des> AuthOrder_DesS { get; set; }
        public DbSet<invoice> invoices { get; set; }
        public DbSet<cancellationInvDORetainInv> cancellationInvDORetainInvs { get; set; }
        public DbSet<cancellationInvoicesonly> cancellationInvoicesonlys { get; set; }
        public DbSet<cancellationoderNoInv> cancellationoderNoInvs { get; set; }

        public DbSet<Order_Invoice_Order_DesForCanfrm> Order_Invoice_Order_DesForCanfrms { get; set; } ////Added on 17-12-2023
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<CashOPSDetails> CashOPSDetails { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<UAM_LoginLogout> UAM_LoginLogout { get; set; }

    }
//    public class DatabaseContextUAM : DbContext
//    {
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
//            var configuration = builder.Build();
//            optionsBuilder.UseSqlServer(configuration["ConnectionStringsUAM:DefaultConnection"]);
//        }
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {      
//            modelBuilder.Entity<UAM_LoginLogout>().HasNoKey();
//            modelBuilder.Entity<UAM_LoginLogoutExist>().HasNoKey();
//        }
//         public DbSet<UAM_LoginLogout> UAM_LoginLogout { get; set; }

//    }
}

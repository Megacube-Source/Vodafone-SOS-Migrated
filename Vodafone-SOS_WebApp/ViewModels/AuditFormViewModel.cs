using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class AuditFormViewModel
    {
        [Required]
        [Display(Name="Control")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string Entity { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public Nullable<DateTime> StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public Nullable<DateTime> EndDate { get; set; }
        //RK Added for claim report filters
        #region AColumns
        [Display(Name = "A01 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A01Filter { get; set; }

        [RestrictSpecialChar]
        public string A01 { get; set; }

        [Display(Name = "A02 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A02Filter { get; set; }
        [RestrictSpecialChar]
        public string A02 { get; set; }

        [Display(Name = "A03 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A03Filter { get; set; }
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [Display(Name = "A04 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A04Filter { get; set; }
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [Display(Name = "A05 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A05Filter { get; set; }
        [RestrictSpecialChar]
        public string A05 { get; set; }

        [Display(Name = "A06 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A06Filter { get; set; }
        [RestrictSpecialChar]
        public string A06 { get; set; }

        [Display(Name = "A07 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A07Filter { get; set; }
        [RestrictSpecialChar]
        public string A07 { get; set; }


        [Display(Name = "A08 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A08Filter { get; set; }
        [RestrictSpecialChar]
        public string A08 { get; set; }

        [Display(Name = "A09 ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A09Filter { get; set; }
        [RestrictSpecialChar]
        public string A09 { get; set; }

        [Display(Name = "A10 ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string A10Filter { get; set; }
        [RestrictSpecialChar]
        public string A10 { get; set; }

        #endregion
        //ActivityTypeId
        [Display(Name = "Allocation Date")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string AllocationDateFilter { get; set; }
        public DateTime AllocationDateFrom { get; set; }
        public DateTime AllocationDateTo { get; set; }

        
        

        [Display(Name = "Already Paid Date")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string AlreadyPaidDateFilter { get; set; }
        public DateTime AlreadyPaidDateFrom { get; set; }
        public DateTime AlreadyPaidDateTo { get; set; }

        [Display(Name = "Connection Date")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ConnectionDateFilter { get; set; }
        public DateTime ConnectionDateFrom { get; set; }
        public DateTime ConnectionDateTo { get; set; }

        [Display(Name = "Last Recliam Date")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LastReclaimDateFilter { get; set; }
        public DateTime LastReclaimDateFrom { get; set; }
        public DateTime LastReclaimDateTo { get; set; }

        [Display(Name = "Order Date")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string OrderDateFilter { get; set; }
        public DateTime OrderDateFrom { get; set; }
        public DateTime OrderDateTo { get; set; }

        [Display(Name = "Already Paid Amount")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string AlreadyPaidAmountFilter { get; set; }
        [Range(typeof(Decimal), "-100000", "99999999", ErrorMessage = "Invalid value")]
        public decimal AlreadyPaidAmountFrom { get; set; }
        [Range(typeof(Decimal), "-100000", "99999999", ErrorMessage = "Invalid value")]
        public decimal AlreadyPaidAmountTo { get; set; }

        [Display(Name = "Clawback Amount")]
        public string ClawbackAmountFilter { get; set; }
        [Range(typeof(Decimal), "-100000", "99999999", ErrorMessage = "Invalid value")]
        public decimal ClawbackAmountFrom { get; set; }
        [Range(typeof(Decimal), "-100000", "99999999", ErrorMessage = "Invalid value")]
        public decimal ClawbackAmountTo { get; set; }

        [Display(Name = "Expected Commission Amount ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ExpectedCommissionAmountFilter { get; set; }
        public decimal ExpectedCommissionAmountFrom { get; set; }
        public decimal ExpectedCommissionAmountTo { get; set; }

        [Display(Name = "Payment Amount ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string PaymentAmountFilter { get; set; }
        public decimal PaymentAmountFrom { get; set; }
        public decimal PaymentAmountTo { get; set; }

        [Display(Name = "BAN ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string BANFilter { get; set; }
        public decimal BAN { get; set; }

        [Display(Name = "Customer Name ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string CustomerNameFilter { get; set; }
        public decimal CustomerName { get; set; }

        [Display(Name = "IMEI ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string IMEIFilter { get; set; }
        public decimal IMEI { get; set; }

        [Display(Name = "MSISDN ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string MSISDNFilter { get; set; }
        public decimal MSISDN { get; set; }

        [Display(Name = "Order Number ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string OrderNumberFilter { get; set; }
        public decimal OrderNumber { get; set; }

        [Display(Name = "Payment Batch Number ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string PaymentBatchNumberFilter { get; set; }
        public decimal PaymentBatchNumber { get; set; }

        [Display(Name = "Reason Non AutoPayment ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ReasonNonAutoPaymentFilter { get; set; }
        public decimal ReasonNonAutoPayment { get; set; }

        [Display(Name = "Claim Batch Number ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ClaimBatchNumberFilter { get; set; }
        public decimal ClaimBatchNumber { get; set; }

        [Display(Name = "Clawback Payee Code ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ClawbackPayeeCodeFilter { get; set; }
        public decimal ClawbackPayeeCode { get; set; }

        [Display(Name = "Brand ")]
        [RestrictSpecialChar]
        public string BrandIds { get; set; }

        [Display(Name = "Commission Type ")]
        [RestrictSpecialChar]
        public string CommissionTypeIds { get; set; }

        [Display(Name = "Device Type ")]
        [RestrictSpecialChar]
        public string DeviceTypeIds { get; set; }

        [Display(Name = "Payment Commission Type ")]
        [RestrictSpecialChar]
        public string PaymentCommissionTypeIds { get; set; }

        [Display(Name = "Product Code ")]
        [RestrictSpecialChar]
        public string ProductCodeIds { get; set; }

        [Display(Name = "Status ")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string StatusFilter { get; set; }

        [RestrictSpecialChar]
        public string Status { get; set; }

        [Display(Name = "Created By ")]
        [RestrictSpecialChar]
        public string CreatedByIds { get; set; }

        [Display(Name = "Activity Type ")]
        [RestrictSpecialChar]
        public string ActivityTypeIds { get; set; }

        [Display(Name = "Rejected Reason ")]
        [RestrictSpecialChar]
        public string RejectedReasonIds { get; set; }

        [Display(Name = "Already Paid Dealer")]
        public Boolean AlreadyPaidDealer { get; set; }
    }
}
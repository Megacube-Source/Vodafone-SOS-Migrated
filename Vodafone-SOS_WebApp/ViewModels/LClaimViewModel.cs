using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LClaimViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Company Id")]
        //[Required(ErrorMessage = "Company Id is required")]
        public int LcCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "The Created By Id can be maximum 200 characters")]
        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string LcCreatedById { get; set; }

        [Display(Name = "Brand Id")]
        public Nullable<int> LcBrandId { get; set; }

        [Display(Name = "Activity Type")]
        public Nullable<int> LcActivityTypeId { get; set; }

        [Display(Name = "Commission Type ")]
        public Nullable<int> LcCommissionTypeId { get; set; }

        [RestrictSpecialChar]
        public string LcAllocatedToId { get; set; }

        [RestrictSpecialChar]
        public string LcAllocatedById { get; set; }
        public Nullable<int> LcRejectionReasonId { get; set; }
        public Nullable<int> LcPaymentCommissionTypeId { get; set; }

        [RestrictSpecialChar]
        public string LcApprovedById { get; set; }

        [RestrictSpecialChar]
        public string LcSentForApprovalById { get; set; }

        [RestrictSpecialChar]
        public string LcWithdrawnById { get; set; }

        [RestrictSpecialChar]
        public string LcRejectedById { get; set; }

        [Display(Name = "Device Type")]
        public Nullable<int> LcDeviceTypeId { get; set; }

       // [Required(ErrorMessage = "Claim Id is required")]
        [Display(Name = "Claim Id")]
        public long LcClaimId { get; set; }

        [Display(Name = "Connection Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcConnectionDate { get; set; }

        [Display(Name = "Order Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcOrderDate { get; set; }

        [Display(Name = "MSISDN")]
        [MaxLength(20, ErrorMessage = "MSISDN Can be maximum 20 characters")]
       // [Required(ErrorMessage = "MSISDN is required")]
        [RestrictSpecialChar]
        public string LcMSISDN { get; set; }

        [Display(Name = "Billing/Customer Account")]
        [MaxLength(20, ErrorMessage = "Ban Can be maximum 20 characters")]
        [RestrictSpecialChar]
        public string LcBAN { get; set; }

        [Display(Name = "Order Number")]
        [MaxLength(20, ErrorMessage = "Order Number Can be maximum 20 Length")]
        [RestrictSpecialChar]
        public string LcOrderNumber { get; set; }

        [MaxLength(100, ErrorMessage = "Customer Name Can be maximum 100 characters")]
        [Display(Name = "Customer Name")]
        [RestrictSpecialChar]
        public string LcCustomerName { get; set; }

        [Display(Name = "Product Code")]
        public Nullable<int> LcProductCodeId { get; set; }

        [Display(Name = "Expected Commision Amount ")]
       // [Required(ErrorMessage = "Expected Conission Amount  is required")]
        //[Range(0.1,double.MaxValue,ErrorMessage= "Expected Conission Amount should be greater than zero")] RK R2.1.5
        public decimal LcExpectedCommissionAmount { get; set; }

        [MaxLength(45)]
        [Display(Name = "IMEI")]
        [RestrictSpecialChar]
        public string LcIMEI { get; set; }

        [Display(Name = "Duplicate Claim")]
        public bool LcIsDuplicateClaim { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Duplicate Claim Details")]
        [RestrictSpecialChar]
        public string LcDuplicateClaimDetails { get; set; }

        [Display(Name = "Allocation Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcAllocationDate { get; set; }

        [Display(Name = "Reclaim")]
     //   [Required(ErrorMessage = "Reclaim is required")]
        public bool LcIsReclaim { get; set; }

        [Display(Name = "Already Paid Amount")]
        public Nullable<decimal> LcAlreadyPaidAmount { get; set; }

        [Display(Name = "Already Paid Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcAlreadyPaidDate { get; set; }

        [Display(Name = "Already Paid Dealer")]
        [MaxLength(200)]
        [RestrictSpecialChar]
        public string LcAlreadyPaidDealer { get; set; }

        [MaxLength(4000)]
        [Display(Name = "Reason For Non Auto Payement")]
        [RestrictSpecialChar]
        public string LcReasonNonAutoPayment { get; set; }

        [MaxLength(20)]
        [Display(Name = "Clawback Payee Code")]
        [RestrictSpecialChar]
        public string LcClawbackPayeeCode { get; set; }

        [Display(Name = "Clawback Amount")]
        public Nullable<decimal> LcClawbackAmount { get; set; }

        [Display(Name = "Sent For Approval Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcSentForApprovalDate { get; set; }

        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcApprovalDate { get; set; }

        [Display(Name = "Payement Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcPaymentDate { get; set; }

        [Display(Name = "Last Reclaim Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcLastReclaimDate { get; set; }

        [Display(Name = "Withdrawn Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> LcWithdrawnDate { get; set; }

        [Display(Name = "Rejection Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LcRejectionDate { get; set; }

        [Display(Name = "To Pay Amount")]
        public Nullable<decimal> LcPaymentAmount { get; set; }

        [MaxLength(255)]
        [Display(Name = "A01")]
        [RestrictSpecialChar]
        public string A01 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A02")]
        [RestrictSpecialChar]
        public string A02 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A03")]
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [RestrictSpecialChar]
        public string LcscTooltip { get; set; }

        [MaxLength(255)]
        [Display(Name = "A04")]
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A05")]
        [RestrictSpecialChar]
        public string A05 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A06")]
        [RestrictSpecialChar]
        public string A06 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A07")]
        [RestrictSpecialChar]
        public string A07 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A08")]
        [RestrictSpecialChar]
        public string A08 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A09")]
        [RestrictSpecialChar]
        public string A09 { get; set; }

        [MaxLength(255)]
        [Display(Name = "A10")]
        [RestrictSpecialChar]
        public string A10 { get; set; }

       
        public Nullable<int> LcPaymentBatchNumber { get; set; }
        public Nullable<int> LcClaimBatchNumber { get; set; }
        public Nullable<System.DateTime> LcCreatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }
        public int WFCompanyId { get; set; }

        [Display(Name = "Existing Comments")]
        [RestrictSpecialChar]
        public string WFComments { get; set; }

        [Required(ErrorMessage = "Payee is required")]
        [Display(Name = "Payee")]
        public int LcPayeeId { get; set; }

        [RestrictSpecialChar]
        public string LcPayeeCode { get; set; }

        [RestrictSpecialChar]
        public string ErrorMessage { get; set; }//to get error message

        [RestrictSpecialChar]
        //object added by shubham to get payee comments
        public string Comments { get; set; }

        // [RestrictSpecialChar]
        // public string RrrRejectionReason { get; set; }


        [RestrictSpecialChar]
        public string FileName { get; set; }//This variable stores file names attached with claims

        [Display(Name = "Commission Period")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LcCommissionPeriod { get; set; }
        [Display(Name = "Parent Payee")]
        public int LcParentPayeeId { get; set; }

        public string SubmitClicked { get; set; }

        public string RChannel { get; set; }

        public string Type { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Forword To")]

        public string ForwordTo { get; set; }


    }
}
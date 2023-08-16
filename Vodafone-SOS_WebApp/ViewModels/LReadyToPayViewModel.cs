using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LReadyToPayViewModel
    {
        #region OldProp
        //public int Id { get; set; }
        //public int Identifier { get; set; }
        //public Boolean Select { get; set; }
        //public int BatchNumber { get; set; }
        //public string BatchName { get; set; }
        //public string LbCommissionPeriod { get; set; }
        //public Int64 ClaimNumber { get; set; }

        //public string PayeeName { get; set; }
        //public Nullable<decimal> PaymentAmount { get; set; }
        //public string RejectedReason { get; set; }
        //[MaxLength(200, ErrorMessage = "Payment Batch Name can be maximum 200 characters")]
        //[Required(ErrorMessage = "Payment Batch Name is required")]
        //[Display(Name = "Payment Batch Name ")]
        //[RestrictSpecialChar]
        //public string PaymentBatchName { get; set; }
        //[MaxLength(200, ErrorMessage = "Commission Period can be maximum 200 characters")]
        //[Required(ErrorMessage = "Commission Period is required")]
        //[Display(Name = "Commission Period ")]
        //[RestrictSpecialChar]
        //public string PaymentBatchCommissionPeriod { get; set; }
        //public int PaymentBatchId { get; set; }
        //public DateTime CreatedDateTime { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime UpdatedDateTime { get; set; }
        //public string UpdatedBy { get; set; }
        //public string ReadyToPayStatus { get; set; }
        ////public string SelectedValues { get; set; }
        #endregion
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "The Payment Batch Name can be maximum 100 characters")]
        [Required(ErrorMessage = "Payment Batch Name is required")]
        [Display(Name = "Payment Batch Name")]
        [RestrictSpecialChar]
        public string PaymentBatchName { get; set; }
        public int PaymentBatchNo { get; set; }
        
        [Required(ErrorMessage = "Period is required")]
        [Display(Name = "Period")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string PeriodName { get; set; }

        [RestrictSpecialChar]
        public string Status { get; set; }

        [RestrictSpecialChar]
        public string UpdatedByID { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        [RestrictSpecialChar]
        public string CreatedByID { get; set; }
        public DateTime CreatedDateTime { get; set; }

        //RTP Details Props
        public int Identifier { get; set; }
        public Nullable<Boolean> Select { get; set; }
        public int BatchNumber { get; set; }
        [RestrictSpecialChar]
        public string BatchName { get; set; }

        [RestrictSpecialChar]
        public string LbCommissionPeriod { get; set; }
        public Int64 ClaimNumber { get; set; }

        [RestrictSpecialChar]
        public string PayeeName { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        [RestrictSpecialChar]
        public string RejectedReason { get; set; }
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string PayPublishEmailIds { get; set; }
        [Display(Name = "Publish Payee Documents in MyDocuments")]
        public bool SendPayeeDocuments { get; set; }
        [Display(Name = "Email Payee Documents to Payees")]
        public bool EmailDocuments { get; set; }

        [RestrictSpecialChar]
        public string CountPayment { get; set; }

        [RestrictSpecialChar]
        public string DocList { get; set; }

        //SG-06feb2020 - Added for displaying 4 new columns
        public string CommissionPeriod { get; set; }
        public string FirstName { get; set; }
        public string PayeeCode { get; set; }
        public string CommissionType { get; set; }
    }
}
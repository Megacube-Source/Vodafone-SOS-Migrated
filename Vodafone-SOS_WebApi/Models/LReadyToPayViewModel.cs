using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
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
        public string PaymentBatchName { get; set; }
        public int PaymentBatchNo { get; set; }
        public string PeriodName { get; set; }
        public string Status { get; set; }
        public string UpdatedByID { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string CreatedByID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        //RTP Details Props
        public int Identifier { get; set; }
        public Boolean Select { get; set; }
        public int BatchNumber { get; set; }
        public string BatchName { get; set; }
        public string LbCommissionPeriod { get; set; }
        public Int64 ClaimNumber { get; set; }
        public string PayeeName { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public string RejectedReason { get; set; }
    }
}
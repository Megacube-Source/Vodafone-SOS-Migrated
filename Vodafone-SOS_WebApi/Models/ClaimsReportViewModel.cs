using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class ClaimsReportViewModel
    {


        public string A01 { get; set; }
        public string A02 { get; set; }
        public string A03 { get; set; }
        public string A04 { get; set; }
        public string A05 { get; set; }
        public string A06 { get; set; }
        public string A07 { get; set; }
        public string A08 { get; set; }
        public string A09 { get; set; }
        public string A10 { get; set; }
        public string ActivityTypeId { get; set; }
        public string AllocationDate { get; set; }
        public Nullable<decimal> AlreadyPaidAmount { get; set; }
        public string AlreadyPaidDate { get; set; }
        public string AlreadyPaidDealer { get; set; }
        public string Ban { get; set; }
        public string  BrandId { get; set; }
        public Nullable<int> ClaimBatchNumber { get; set; }
        public Nullable<decimal> ClawbackAmount { get; set; }
        public string ClawbackPayeeCode { get; set; }
        public string CommissionTypeId { get; set; }
        public string ConnectionDate { get; set; }
        public string CreatedById { get; set; }
        public string CustomerName { get; set; }
        public string DeviceTypeId { get; set; }
        public Nullable<decimal> ExpectedCommissionAmount { get; set; }
        public string IMEI { get; set; }
        public string LastReclaimDate { get; set; }
        public string MSISDN { get; set; }
        public string OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public Nullable<int> PayeeId { get; set; }
        public string PayeeName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<int> PaymentBatchNumber { get; set; }
        public string PaymentCommissionTypeId { get; set; }
       // public Nullable<int> ProductCode { get; set; }
        public string ReasonNonAutoPayment { get; set; }
        public string Status { get; set; }
        public string RejectedReasonId { get; set; }

        public Boolean Reclaim { get; set; }
        public Int64 ClaimNumber { get; set; }

        public string Payeefirstname { get; set; }
        public string PayeeLastname { get; set; }

        public string CurrentOwner { get; set; }

        public string CreatedDateTime { get; set; }

        public string ParentPayeeId { get; set; }

        public string CommissionPeriod { get; set; }
        public string Comments { get; set; }

    }
}
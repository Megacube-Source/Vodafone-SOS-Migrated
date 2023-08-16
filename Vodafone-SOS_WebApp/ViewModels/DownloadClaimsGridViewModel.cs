using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class DownloadClaimsGridViewModel
    {
        //RK Adding the special character restric attribute as we are not sure at the moment that are the set is required here or not
        [RestrictSpecialChar]
        public string A01 { get; set; }


        [RestrictSpecialChar]
        public string A02 { get; set; }


        [RestrictSpecialChar]
        public string A03 { get; set; }


        [RestrictSpecialChar]
        public string A04 { get; set; }


        [RestrictSpecialChar]
        public string A05 { get; set; }


        [RestrictSpecialChar]
        public string A06 { get; set; }


        [RestrictSpecialChar]
        public string A07 { get; set; }


        [RestrictSpecialChar]
        public string A08 { get; set; }


        [RestrictSpecialChar]
        public string A09 { get; set; }


        [RestrictSpecialChar]
        public string A10 { get; set; }

        [RestrictSpecialChar]
        public string ActivityTypeId { get; set; }

        public string AllocationDate { get; set; }


        public Nullable<decimal> AlreadyPaidAmount { get; set; }
        public string AlreadyPaidDate { get; set; }

        [RestrictSpecialChar]
        public string AlreadyPaidDealer { get; set; }

        [RestrictSpecialChar]
        public string Ban { get; set; }

        [RestrictSpecialChar]
        public string BrandId { get; set; }


        public Nullable<int> ClaimBatchNumber { get; set; }


        public Nullable<decimal> ClawbackAmount { get; set; }

        [RestrictSpecialChar]
        public string ClawbackPayeeCode { get; set; }

        [RestrictSpecialChar]
        public string CommissionTypeId { get; set; }
        public string ConnectionDate { get; set; }

        [RestrictSpecialChar]
        public string CreatedById { get; set; }

        [RestrictSpecialChar]
        public string CustomerName { get; set; }

        [RestrictSpecialChar]
        public string DeviceTypeId { get; set; }

        public Nullable<decimal> ExpectedCommissionAmount { get; set; }

        [RestrictSpecialChar]
        public string IMEI { get; set; }

        [RestrictSpecialChar]
        public string LastReclaimDate { get; set; }

        [RestrictSpecialChar]
        public string MSISDN { get; set; }
        public string OrderDate { get; set; }

        [RestrictSpecialChar]
        public string OrderNumber { get; set; }

        [RestrictSpecialChar]
        public string ParentCode { get; set; }

        [RestrictSpecialChar]
        public string ParentName { get; set; }

        [RestrictSpecialChar]
        public string PayeeName { get; set; }

        [RestrictSpecialChar]
        public string PayeeCode { get; set; }

        [RestrictSpecialChar]
        public string CreatedBy { get; set; }


        public Nullable<int> PaymentAmount { get; set; }


        public Nullable<int> PaymentBatchNumber { get; set; }

        [RestrictSpecialChar]
        public string PaymentCommissionTypeId { get; set; }

        [RestrictSpecialChar]
        public string ProductCode { get; set; }

        [RestrictSpecialChar]
        public string ReasonNonAutoPayment { get; set; }

        [RestrictSpecialChar]
        public string Status { get; set; }

        public Boolean Reclaim { get; set; }
        public Int64 ClaimNumber { get; set; }

        public string Payeefirstname { get; set; }
        public string PayeeLastname { get; set; }

        public string CurrentOwner { get; set; }

        public string CreatedDateTime { get; set; }


        public Nullable<int> PayeeId { get; set; }
        public string ParentPayeeId { get; set; }

        public string CommissionPeriod { get; set; }
        public string Comments { get; set; }


    }
}
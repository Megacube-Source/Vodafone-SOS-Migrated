using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LClaimReportFilterViewModel
    {
        [Display(Name = "Expected Commission Amount Between ")]
        public decimal ExpectedCommissionAmountFrom { get; set; }
        public decimal ExpectedCommissionAmountTo { get; set; }
        
        //ExpectedCommissionAmount
        //////PayeeId
        //A01
        //A02
        //A03
        //A04
        //A05
        //A06
        //A07
        //A08
        //A09
        //A10
        //ActivityTypeId
        //AllocationDate
        //AlreadyPaidAmount
        //AlreadyPaidDate
        //AlreadyPaidDealer
        //BAN
        //BrandId
        //ClaimBatchNumber
        //ClawbackAmount
        //ClawbackPayeeCode
        //CommissionTypeId
        //ConnectionDate
        //CreatedById
        //CreatedDateTime
        //CustomerName
        //DeviceTypeId
        //IMEI
        //LastReclaimDate
        //MSISDN
        //OrderDate
        //OrderNumber
        //PaymentAmount
        //PaymentBatchNumber
        //PaymentCommissionTypeId
        //ProductCodeId
        //ReasonNonAutoPayment
        //Status
    }
}
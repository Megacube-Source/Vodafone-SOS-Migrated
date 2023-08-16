using System;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LCalcRowCountsViewModel
    {
        public int RowCounts { get; set; }
    }

    public partial class LCalcViewModel
    {
        public int Id { get; set; }
        public string LcOpCoCode { get; set; }
        public string LcSource { get; set; }
        public string LcAdjustmenCode { get; set; }
        public Nullable<int> LcSOSBatchNumber { get; set; }
        public Nullable<long> LcAlteryxTransactionNumber { get; set; }
        public string LcPrimaryChannel { get; set; }
        public string LcPayee { get; set; }
        public string LcParentPayee { get; set; }
        public Nullable<System.DateTime> LcOrderDate { get; set; }
        public Nullable<System.DateTime> LcConnectionDate { get; set; }
        public Nullable<System.DateTime> LcTerminationDate { get; set; }
        public string LcSubscriberNumber { get; set; }
        public string LcBAN { get; set; }
        public string LcActivityType { get; set; }
        public string LcPlanDescrition { get; set; }
        public string LcProductCode { get; set; }
        public string LcUpgradeCode { get; set; }
        public string LcIMEI { get; set; }
        public string LcDevieCode { get; set; }
        public string LcDeviceType { get; set; }
        public string LcCommType { get; set; }
        public Nullable<int> LcContractDuration { get; set; }
        public string LcContractId { get; set; }
        public Nullable<decimal> LcCommAmtExTax { get; set; }
        public Nullable<decimal> LcTax { get; set; }
        public Nullable<decimal> LcCommAmtIncTax { get; set; }
        public string LcComments { get; set; }
        public Nullable<bool> LcIsPayeeAccepted { get; set; }
        public Nullable<int> LcPayeeAttachmentId { get; set; }
        public Nullable<System.DateTime> LcAcceptedDateTime { get; set; }
        public string LcAcceptedById { get; set; }
        //added for Payee Summary
        public string LbCommissionPeriod { get; set; }

        public string WFStatus { get; set; }
    }
}
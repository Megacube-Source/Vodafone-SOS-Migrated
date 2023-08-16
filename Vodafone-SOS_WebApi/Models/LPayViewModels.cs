using System;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LPayRowCountsViewModel
    {
        public int RowCounts { get; set; }
    }
    public partial class DownloadLPayViewModel
    {
        public string OpCoCode { get; set; }
        public string Source { get; set; }
        public string AdjustmenCode { get; set; }
        public Nullable<int> SOSBatchNumber { get; set; }
        public Nullable<long> AlteryxTransactionNumber { get; set; }
        public string PrimaryChannel { get; set; }
        public string Payee { get; set; }
        public string ParentPayee { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> ConnectionDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public string SubscriberNumber { get; set; }
        public string BAN { get; set; }
        public string ActivityType { get; set; }
        public string PlanDescrition { get; set; }
        public string ProductCode { get; set; }
        public string UpgradeCode { get; set; }
        public string IMEI { get; set; }
        public string DevieCode { get; set; }
        public string DeviceType { get; set; }
        public string CommType { get; set; }
        public Nullable<int> ContractDuration { get; set; }
        public string ContractId { get; set; }
        public Nullable<decimal> CommAmtExTax { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> CommAmtIncTax { get; set; }
        public string Comments { get; set; }
    }

    public partial class LPayViewModel
    {
        public Nullable<int> Id { get; set; }
        public string LpOpCoCode { get; set; }
        public string LpSource { get; set; }
        public string LpAdjustmenCode { get; set; }
        public Nullable<int> LpSOSBatchNumber { get; set; }
        public Nullable<long> LpAlteryxTransactionNumber { get; set; }
        public string LpPrimaryChannel { get; set; }
        public string LpPayee { get; set; }
        public string LpParentPayee { get; set; }
        public Nullable<System.DateTime> LpOrderDate { get; set; }
        public Nullable<System.DateTime> LpConnectionDate { get; set; }
        public Nullable<System.DateTime> LpTerminationDate { get; set; }
        public string LpSubscriberNumber { get; set; }
        public string LpBAN { get; set; }
        public string LpActivityType { get; set; }
        public string LpPlanDescrition { get; set; }
        public string LpProductCode { get; set; }
        public string LpUpgradeCode { get; set; }
        public string LpIMEI { get; set; }
        public string LpDevieCode { get; set; }
        public string LpDeviceType { get; set; }
        public string LpCommType { get; set; }
        public Nullable<int> LpContractDuration { get; set; }
        public string LpContractId { get; set; }
        public Nullable<decimal> LpCommAmtExTax { get; set; }
        public Nullable<decimal> LpTax { get; set; }
        public Nullable<decimal> LpCommAmtIncTax { get; set; }
        public string LpComments { get; set; }
    }
}
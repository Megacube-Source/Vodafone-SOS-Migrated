using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class DownloadLCalcViewModel
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
        public Nullable<decimal> Comments { get; set; }
    }
}
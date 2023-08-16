using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class PostUploadManualAdjustMentViewModel
    {
        public int XAlteryxTransactionNumber { get; set; }
        //public string LcOpCoCode { get; set; }
        public string XSource { get; set; }
        public string XAdjustmenCode { get; set; }
        //[RestrictSpecialChar]
        //public string LcPrimaryChannel { get; set; }
        public string XPayee { get; set; }
        public string XParentPayee { get; set; }
        //Commented below three propery for validate the data in api
        //public Nullable<System.DateTime> XOrderDate { get; set; }
        //public Nullable<System.DateTime> XConnectionDate { get; set; }
        //public Nullable<System.DateTime> XTerminationDate { get; set; }

        public string XOrderDate { get; set; }
        public string XConnectionDate { get; set; }
        public string XTerminationDate { get; set; }

        public string XSubscriberNumber { get; set; }
        public string XBAN { get; set; }
        public string XActivityType { get; set; }
        public string XPlanDescrition { get; set; }
        public string XProductCode { get; set; }
        public string XUpgradeCode { get; set; }
        public string XIMEI { get; set; }
        public string XDevieCode { get; set; }
        public string XDeviceType { get; set; }
        public string XCommType { get; set; }
        public Nullable<int> XContractDuration { get; set; }
        public string XContractId { get; set; }
        //public Nullable<decimal> XCommAmtExTax { get; set; }
        //public Nullable<decimal> XTax { get; set; }
        //public Nullable<decimal> XCommAmtIncTax { get; set; }

        public string XCommAmtExTax { get; set; }
        public string XTax { get; set; }
        public string XCommAmtIncTax { get; set; }


        public string XComments { get; set; }
        //public string XBatchName { get; set; }
        //public string XCommissionPeriod { get; set; }
        //public Nullable<int> XPeriodCode { get; set; }
        //public string SalesComments { get; set; }
       
        public string ManualAdjustmentCode { get; set; }
        public string XPayeeName { get; set; }
        public string XParentPayeeName { get; set; }
       public string XChannel { get; set; }
        public string ErrorMessage { get; set; }
        //public string LcSource { get; set; }
        //public string LcAdjustmenCode { get; set; }
        //public Nullable<long> LcAlteryxTransactionNumber { get; set; }
        //public string LcPayee { get; set; }
        //public string LcParentPayee { get; set; }
        //public Nullable<System.DateTime> LcOrderDate { get; set; }
        //public Nullable<System.DateTime> LcConnectionDate { get; set; }
        //public Nullable<System.DateTime> LcTerminationDate { get; set; }
        //public string LcSubscriberNumber { get; set; }
        //public string LcBAN { get; set; }
        //public string LcActivityType { get; set; }
        //public string LcPlanDescrition { get; set; }
        //public string LcProductCode { get; set; }
        //public string LcUpgradeCode { get; set; }
        //public string LcIMEI { get; set; }
        //public string LcDevieCode { get; set; }
        //public string LcDeviceType { get; set; }
        //public string LcCommType { get; set; }
        //public Nullable<int> LcContractDuration { get; set; }
        //public Nullable<decimal> LcCommAmtExTax { get; set; }
        //public Nullable<decimal> LcTax { get; set; }
        //public Nullable<decimal> LcCommAmtIncTax { get; set; }
        //public string LcComments { get; set; }
        //public string ErrorMessage { get; set; }
        public string ParameterCarrier { get; set; }
    }
}
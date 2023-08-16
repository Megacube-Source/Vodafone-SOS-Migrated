using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class UploadManualAdjustmentViewModel
    {
        public int XAlteryxTransactionNumber { get; set; }
        //public int Id { get; set; }
        //[RestrictSpecialChar]
        //public string LcOpCoCode { get; set; }

        [RestrictSpecialChar]
        public string XChannel { get; set; }
        [RestrictSpecialChar]
        public string XSource { get; set; }

        [RestrictSpecialChar]
        public string XAdjustmenCode { get; set; }

        //[RestrictSpecialChar]
        //public string LcPrimaryChannel { get; set; }

        [RestrictSpecialChar]
        public string XPayee { get; set; }

        [RestrictSpecialChar]
        public string XParentPayee { get; set; }
        //public Nullable<System.DateTime> XOrderDate { get; set; }
        //public Nullable<System.DateTime> XConnectionDate { get; set; }
        //public Nullable<System.DateTime> XTerminationDate { get; set; }

        [RestrictSpecialChar]
        public string XOrderDate { get; set; }

        [RestrictSpecialChar]
        public string XConnectionDate { get; set; }

        [RestrictSpecialChar]
        public string XTerminationDate { get; set; }

        [RestrictSpecialChar]
        public string XSubscriberNumber { get; set; }

        [RestrictSpecialChar]
        public string XBAN { get; set; }

        [RestrictSpecialChar]
        public string XActivityType { get; set; }

        [RestrictSpecialChar]
        public string XPlanDescrition { get; set; }

        [RestrictSpecialChar]
        public string XProductCode { get; set; }

        [RestrictSpecialChar]
        public string XUpgradeCode { get; set; }

        [RestrictSpecialChar]
        public string XIMEI { get; set; }

        [RestrictSpecialChar]
        public string XDevieCode { get; set; }

        [RestrictSpecialChar]
        public string XDeviceType { get; set; }

        [RestrictSpecialChar]
        public string XCommType { get; set; }
        public Nullable<int> XContractDuration { get; set; }

        [RestrictSpecialChar]
        public string XContractId { get; set; }
        //public Nullable<decimal> XCommAmtExTax { get; set; }
        //public Nullable<decimal> XTax { get; set; }
        //public Nullable<decimal> XCommAmtIncTax { get; set; }


        [RestrictSpecialChar]
        public string XCommAmtExTax { get; set; }

        [RestrictSpecialChar]
        public string XTax { get; set; }

        [RestrictSpecialChar]
        public string XCommAmtIncTax { get; set; }

        [RestrictSpecialChar]
        public string XComments { get; set; }
        //public string XBatchName { get; set; }
       // public string XCommissionPeriod { get; set; }
        //public Nullable<int> XPeriodCode { get; set; }

        //[RestrictSpecialChar]
        //public string SalesComments { get; set; }
        [RestrictSpecialChar]
        public string ManualAdjustmentCode { get; set; }
        [RestrictSpecialChar]
        public string XPayeeName { get; set; }
        [RestrictSpecialChar]
        public string XParentPayeeName { get; set; }

        [RestrictSpecialChar]
        public string ErrorMessage { get; set; }

        public string ParameterCarrier { get; set; }
        //  public string AtachedFiles { get; set; }
    }
}
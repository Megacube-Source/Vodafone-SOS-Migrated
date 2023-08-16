using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class XRawData_BI_PREPAID_ACTIVATION_REPORTViewModel
    {
        [RestrictSpecialChar]
        public string XStatus { get; set; }

        [RestrictSpecialChar]
        public string XComment { get; set; }

        [RestrictSpecialChar]
        public string XErrorMessage { get; set; }

        [RestrictSpecialChar]
        public string XResolution { get; set; }
        public int XBatchNumber { get; set; }

        [RestrictSpecialChar]
        public string XCreatedBy { get; set; }
        public Nullable<System.DateTime> XCreateDateTime { get; set; }

        [RestrictSpecialChar]
        public string XUpdatedBy { get; set; }
        public Nullable<System.DateTime> XUpdateDateTime { get; set; }
        public Nullable<System.DateTime> XCommissionPeriod { get; set; }
        public int XTransactionNumber { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public Nullable<System.DateTime> SUREPAY_FIRST_CALL_DATE { get; set; }
        public Nullable<System.DateTime> FIRST_RECHARGE_DATE { get; set; }
        public Nullable<int> CONNECTIONS { get; set; }
        public Nullable<int> FIRST_RECHARGE_AMOUNT { get; set; }

        [RestrictSpecialChar]
        public string MSISDN { get; set; }

        [RestrictSpecialChar]
        public string PLAN_GROUP { get; set; }

        [RestrictSpecialChar]
        public string ICCID { get; set; }

        [RestrictSpecialChar]
        public string DAY_OF_WEEK { get; set; }

        [RestrictSpecialChar]
        public string CUSTOMER_TYPE { get; set; }

        [RestrictSpecialChar]
        public string TEST_MSISDN_FLAG { get; set; }

        [RestrictSpecialChar]
        public string MOVEMENT { get; set; }

        [RestrictSpecialChar]
        public string MOVEMENT_CATEGORY { get; set; }

        [RestrictSpecialChar]
        public string MOVEMENT_MINOR_CATEGORY { get; set; }

        [RestrictSpecialChar]
        public string ACTIVATION_SALES_ZONE { get; set; }

        [RestrictSpecialChar]
        public string DISTRIBUTED_TO { get; set; }

        [RestrictSpecialChar]
        public string SALES_CHANNEL { get; set; }

        [RestrictSpecialChar]
        public string CUSTOMER_ID_TYPE { get; set; }

        [RestrictSpecialChar]
        public string CUSTOMER_ID { get; set; }

        [RestrictSpecialChar]
        public string REMARKS { get; set; }

        [RestrictSpecialChar]
        public string RECHARGE_PRODUCT { get; set; }

        [RestrictSpecialChar]
        public string CUSTOMER_ID_CHECK { get; set; }

        [RestrictSpecialChar]
        public string FIRST_RECHARGE_CHECK { get; set; }

        [RestrictSpecialChar]
        public string COMMENTS { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class XBatchViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string XCompanyCode { get; set; }

        [RestrictSpecialChar]
        public string XUpdatedBy { get; set; }
        public int XBatchNumber { get; set; }

        [RestrictSpecialChar]
        public string XBatchType { get; set; }

        [RestrictSpecialChar]
        public string XRawDataType { get; set; }

        [RestrictSpecialChar]
        public string XStatus { get; set; }
        public System.DateTime XUploadStartDateTime { get; set; }
        public Nullable<System.DateTime> XUploadFinishDateTime { get; set; }
        public Nullable<int> XRecordCount { get; set; }
        public Nullable<int> XAlteryxBatchNumber { get; set; }

        [RestrictSpecialChar]
        public string XComments { get; set; }

        [RestrictSpecialChar]
        public string XCommissionPeriod { get; set; }

        [RestrictSpecialChar]
        public string XPrimaryChannel { get; set; }

        [RestrictSpecialChar]
        public string XBusinessUnit { get; set; }

        [RestrictSpecialChar]
        public string XSubChannel { get; set; }

    }
}
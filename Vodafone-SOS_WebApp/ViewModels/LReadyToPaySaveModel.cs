using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LReadyToPaySaveModel
    {
        public int iRTPID { get; set; }
        public int iCompanyID { get; set; }
        public Boolean blnIsBatchList { get; set; }
        public string strType { get; set; }
        public string strRTPStatus { get; set; }
        public string strAction { get; set; }
        public string strPayBatchName { get; set; }
        public string strBatchCommPeriod { get; set; }
        public string strRTPData { get; set; }
        public string strCreatedBy { get; set; }
        public string strUpdatedBy { get; set; }
        public string strPortfolios { get; set; }
        public bool EmailDocuments { get; set; }
        public bool SendPayeeDocuments { get; set; }
        public string PayPublishEmailIds { get; set; }
        public string UserRole { get; set; }

        public Boolean IsClaimChanged { get; set; }
        public Boolean IsCalChanged { get; set; }
        public Boolean isMAChanged { get; set; }
    }
}
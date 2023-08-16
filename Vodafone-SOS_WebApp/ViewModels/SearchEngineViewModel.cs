using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class SearchEngineViewModel
    {
        public bool ChkSubscriberNumber { get; set; }
        public string SubscriberNumber { get; set; }
        public bool ChkCustomerSegment { get; set; }
        public string CustomerSegment { get; set; }
        public bool ChkActivityType { get; set; }
        public string ActivityType { get; set; }
        public bool ChkActivationOrder { get; set; }
        public string ActivationOrder { get; set; }
        public bool ChkCommType { get; set; }
        public string CommissionType { get; set; }
        public bool ChkChannel { get; set; }
        public string Channel { get; set; }
        public bool ChkParentPayee { get; set; }
        public string PayeeParent { get; set; }
        public bool ChkSubChannel { get; set; }
        public string SubChannel { get; set; }
        public bool ChkPayee { get; set; }
        public string Payees { get; set; }
        public bool ChkPeriod { get; set; }
        public string Period { get; set; }
        public bool ChkBatchStatus { get; set; }
        public  string BatchStatus { get; set; }
        public bool ChkMSDIN { get; set; }
        public string MSDIN { get; set; }
        public int CountryID { get; set; }
        public string UserName { get; set; }
        public string Workflow { get; set; }
        public string LoggedInUserId { get; set; }
        public string UserRole { get; set; }

        public string CompanyCode { get; set; }

        public string SelectedTab { get; set; }

        public string LoggedinUserName { get; set; }
    }
}
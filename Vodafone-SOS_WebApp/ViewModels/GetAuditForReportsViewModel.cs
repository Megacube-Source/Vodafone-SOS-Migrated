using System;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class GetLAuditForReports
    {
        public Int32 row { get; set; }

        [RestrictSpecialChar]
        public string LaSOSProcessName { get; set; }

        [RestrictSpecialChar]
        public string LaL3ProcessName { get; set; }

        [RestrictSpecialChar]
        public string LaControlCode { get; set; }

        [RestrictSpecialChar]
        public string LaControlDescription { get; set; }

        [RestrictSpecialChar]
        public string LaAction { get; set; }
        //public string LaActionType { get; set; }

        [RestrictSpecialChar]
        public string LaActionedById { get; set; }
        public System.DateTime LaActioDateTime { get; set; }

        [RestrictSpecialChar]
        public string LaOldStatus { get; set; }

        [RestrictSpecialChar]
        public string LaNewStatus { get; set; }

        [RestrictSpecialChar]
        public string LaEntityType { get; set; }
        public Int32 LaEntityId { get; set; }

        [RestrictSpecialChar]
        public string LaEntityName { get; set; }

        [RestrictSpecialChar]
        // public Nullable<int> LaWorkflowId { get; set; }
        public string GcCompanyName { get; set; }

        [RestrictSpecialChar]
        public string LaRoleId { get; set; }
        //Adding extra columns used in case of Payee CR and User CR LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName

        [RestrictSpecialChar]
        public string LcrOldValue { get; set; }

        [RestrictSpecialChar]
        public string LcrNewValue { get; set; }

        [RestrictSpecialChar]
        public string LcrColumnLabel { get; set; }

        [RestrictSpecialChar]
        public string LcrColumnName { get; set; }
        [RestrictSpecialChar]
        public string laPeriod { get; set; }

        public int counts { get; set; }

        [RestrictSpecialChar]
        public string GcCode { get; set; }
        public int workflowId { get; set; }

        public string PayeeCode { get; set; }
    }
}
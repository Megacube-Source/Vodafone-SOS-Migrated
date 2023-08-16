using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class GetLAuditForReports
    {
        public Int64 row { get; set; }
        //public int Id { get; set; }
        public string LaSOSProcessName { get; set; }
        public string LaL3ProcessName { get; set; }
        public string LaControlCode { get; set; }
        public string LaControlDescription { get; set; }
        public string LaAction { get; set; }
        //public string LaActionType { get; set; }
        public string LaActionedById { get; set; }
        public System.DateTime LaActioDateTime { get; set; }
        public string LaOldStatus { get; set; }
        public string LaNewStatus { get; set; }
        public string LaEntityType { get; set; }
        public Int32 LaEntityId { get; set; }
        public string LaEntityName { get; set; }
       // public Nullable<int> LaWorkflowId { get; set; }
        public string GcCompanyName { get; set; }
        public string LaRoleId { get; set; }

        //Adding extra columns used in case of Payee CR and User CR LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName
        public string LcrOldValue { get; set; }
        public string LcrNewValue { get; set; }
        public string LcrColumnLabel { get; set; }
        public string LcrColumnName { get; set; }
        public string laPeriod { get; set; }
        public int counts {get;set;}
        public string GcCode { get; set; }
        public int workflowId { get; set; }
        
        public string PayeeCode { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LChangeRequestViewModel
    {
        public int Id { get; set; }
        public string LcrEntityName { get; set; }
        public Nullable<int> LcrRowId { get; set; }
        public string LcrColumnName { get; set; }
        public string LcrColumnLabel { get; set; }
        public string LcrOldValue { get; set; }
        public string LcrNewValue { get; set; }
        public System.DateTime LcrCreatedDateTime { get; set; }
        public Nullable<System.DateTime> LcrUpdatedDateTime { get; set; }
        public string LcrCreatedById { get; set; }
        public string LcrUpdatedById { get; set; }
        public string LcrAction { get; set; }
        public int LcrCompanyId { get; set; }
        public Nullable<System.DateTime> LcrEffectiveStartDate { get; set; }
        public string LcrNewId { get; set; }
        public string LcrOldId { get; set; }
        public string WFRequesterId { get; set; }
        public string WFAnalystId { get; set; }
        public string WFManagerId { get; set; }
        public string WFCurrentOwnerId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public string WFRequesterRoleId { get; set; }
        public int WFCompanyId { get; set; }
        public string WFComments { get; set; }
               
        //RK added
        public string LpPayeeCode { get; set; }
        public bool LcrCreatedByForm { get; set; }
        public string EmailId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LDocumentSetsViewModel
    {
        public int Id { get; set; }
        public string LdsName { get; set; }
        public string LdsDescription { get; set; }
        public string LdsCommissionPeriod { get; set; }
        public bool LdsSendEmail { get; set; }
        public string LdsCreatedById { get; set; }
        public string LdsUpdatedById { get; set; }
        public System.DateTime LdsCreatedDateTime { get; set; }
        public System.DateTime LdsUpdatedDateTime { get; set; }
        public string WFRequesterId { get; set; }
        public string WFAnalystId { get; set; }
        public string WFManagerId { get; set; }
        public string WFCurrentOwnerId { get; set; }
        public string WFRequesterRoleId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }
        public string WFComments { get; set; }
        public string LdsPayeeList { get; set; }
        public string LdsDocumentList { get; set; }
        public string ParameterCarrier { get; set; }
        public string AttachedFiles { get; set; }
        public string SupportingDocumentFiles { get; set; }
        public string PayeeSelection { get; set; }
        public string PayeeListCarrier { get; set; }
    }
}
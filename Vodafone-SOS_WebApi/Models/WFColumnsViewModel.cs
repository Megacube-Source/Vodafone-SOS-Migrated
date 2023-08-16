using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class WFColumnsViewModel
    {
        public string WFRequesterId { get; set; }
        public string WFAnalystId { get; set; }
        public string WFManagerId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFCurrentOwnerId { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public string WFRequesterRoleId { get; set; }
        public Nullable<int> WFCompanyId { get; set; }
        public string WFComments { get; set; }
    }
}
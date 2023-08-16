using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class RWorkFlowViewModel
    {
        public string GcCode { get; set; }
        public int LBatches { get; set; }
        public int LPayees { get; set; }
        public int LClaims { get; set; }
        public int LUsers { get; set; }
        public int LChangeRequests { get; set; }
        public int LRefFiles { get; set; }
        public int LSchemes { get; set; }

        public int LAccruals { get; set; }

        public int LDocumentSets { get; set; }
    }
}
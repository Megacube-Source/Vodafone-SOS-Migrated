using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LCommissionPeriodsViewModel
    {
        public int Id { get; set; }
        public int LcpCompanyId { get; set; }
        public string LcpPeriodName { get; set; }
        public string LcpCreatedById { get; set; }
        public DateTime LcpCreatedDateTime { get; set; }
        public string LcpUpdatedById { get; set; }
        public DateTime LcpUpdatedDateTime { get; set; }
        public string LcpStatus { get; set; }
    }
}
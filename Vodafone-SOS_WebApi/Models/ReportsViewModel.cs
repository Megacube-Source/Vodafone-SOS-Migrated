using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class ReportsViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string XCommissionPeriod { get; set; }
        public string XDisplayLocation { get; set; }
        public string XName { get; set; }
    }
}
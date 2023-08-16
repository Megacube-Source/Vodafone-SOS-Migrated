using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LPortfolioViewModel
    {
        public int Id { get; set; }

        
        public int LpCompanyId { get; set; }

        
        public string LpBusinessUnit { get; set; }

        public int LpChannelId { get; set; }

        public string LpStatus { get; set; }

        
        public string RcName { get; set; }//object defined to get channel name
        
        public string RcPrimaryChannel { get; set; }
        public bool Select { get; set; }
    }
}
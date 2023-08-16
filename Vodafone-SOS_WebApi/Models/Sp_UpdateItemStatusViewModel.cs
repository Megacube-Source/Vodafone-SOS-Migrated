using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class Sp_UpdateItemStatusViewModel
    {
        public string ItemList { get; set; }
        public string Comments { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
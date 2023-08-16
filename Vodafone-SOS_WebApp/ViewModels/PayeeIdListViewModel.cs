using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Models
{
    public partial class PayeeIdListViewModel
    {
        [RestrictSpecialChar]
        public string PayeeIdList { get; set; }
    }
}
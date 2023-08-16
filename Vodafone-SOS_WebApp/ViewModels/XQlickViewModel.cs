using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class XQlickViewModel
    {
        [RestrictSpecialChar]
        public string XRole { get; set; }

        [RestrictSpecialChar]
        public string XURL { get; set; }
    }
}
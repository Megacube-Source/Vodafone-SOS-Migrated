using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class XPayeeDocumentsViewModel
    {
        [RestrictSpecialChar]
        public string XFileName { get; set; }
        [RestrictSpecialChar]
        public string XFileLocation { get; set; }
        [RestrictSpecialChar]
        public string XPayeeCode { get; set; }
    }
}
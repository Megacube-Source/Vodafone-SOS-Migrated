using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LDropDownViewModel
    {
        public int Id { get; set; }
        public int LdCompanyId { get; set; }

        [RestrictSpecialChar]
        public string LdName { get; set; }

        [RestrictSpecialChar]
        public string LdDescription { get; set; }

    }
}
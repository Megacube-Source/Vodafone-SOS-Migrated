using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LDropDownValueViewModel
    {
        public int Id { get; set; }
        public int LdvDropDownId { get; set; }

        [RestrictSpecialChar]
        public string LdvDescription { get; set; }

        [RestrictSpecialChar]
        public string LdvValue { get; set; }
    }
}
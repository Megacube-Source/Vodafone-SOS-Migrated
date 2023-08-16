using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class PortfolioListViewModel
    {
        //test  changes
        [RestrictSpecialChar]
        public string PortfolioList { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GridDataViewModel
    {
        [RestrictSpecialChar]
        public string GridData { get; set; }
        public int CompanyId { get; set; }
    }
}
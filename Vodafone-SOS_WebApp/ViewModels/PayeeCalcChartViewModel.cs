using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class PayeeCalcChartViewModel
    {
        [RestrictSpecialChar]
        public string CommissionPeriod { get; set; }
        public decimal PrelimCount { get; set; }
        public decimal CompletedCount { get; set; }
        public int RowNumber { get; set; }
    }
}